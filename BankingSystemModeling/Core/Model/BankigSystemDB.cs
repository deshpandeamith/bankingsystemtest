using BankingSystemModeling.Core.Enum;

namespace BankingSystemModeling.Core.Model;


public class BankigSystemDB : IBankigSystemDB
{

    public Dictionary<String, BankUser> Users = new();
    public Dictionary<long, Dictionary<long, BankAccount>> Accounts = new();
    private long AccountId = 100000;
    private long AccountNumber = 1000000000;
    private long TransactionId = 1000000000;
    private Object AccountIdlockObj = new Object();
    private Object AccountNumberlockObj = new Object();
    private Object TransactionlockObj = new Object();
    private Object lockObj = new Object();
    private readonly ILogger _logger;

    public BankigSystemDB()
    {
        _logger = LoggerFactory.Create(config =>
        {
            config.AddConsole();
        }).CreateLogger(typeof(BankigSystemDB));

        // initializing with a set of users
        Users.Add("djohn", new BankUser
        {
            UserId = 1, 
            UserName = "djohn",
            FirstName = "John",
            LastName = "Doe",
            IsActive = true
        });
        Users.Add("rsteven", new BankUser
        {
            UserId = 2, 
            UserName = "rsteven",
            FirstName = "Steven",
            LastName = "Roth",
            IsActive = true
        });
        Accounts.Add(1, new());
        Accounts.Add(2, new());
    }

    /// <summary>
    /// Generates a new Account Id
    /// </summary>
    /// <returns></returns>
    private long NextAccountId()
    {
        lock (AccountIdlockObj)
        {
            log($"NextAccountId AccountId:{AccountId}");
            return ++AccountId;
        }
    }
    /// <summary>
    /// Generates a new account number
    /// </summary>
    /// <returns></returns>
    private long NextAccountNumber()
    {
        lock (AccountNumberlockObj)
        {
            log($"NextAccountNumber AccountNumber:{AccountNumber}");
            return ++AccountNumber;
        }
    }
    /// <summary>
    /// Generates a new transaction id
    /// </summary>
    /// <returns></returns>
    private long NextTransactionId()
    {
        lock (TransactionlockObj)
        {
            log($"NextTransactionId TransactionId:{TransactionId}");
            return ++TransactionId;
        }
    }

    /// <summary>
    /// Retrieve user information using username
    /// </summary>
    /// <param name="userName">UserName of the </param>
    /// <returns>BankUser object with the userinformation for the passed username else returns null</returns>
    public BankUser? GetUser(String userName)
    {
        log($"GetUser userName:{userName}");
        if (this.Users.ContainsKey(userName))
        {
            return Users[userName];
        }
        return null;
    }

    /// <summary>
    /// Retrieves all the Bank Accounts for the passed userName of the User
    /// </summary>
    /// <param name="userName">userName of the user whose accounts need to be retrievec</param>
    /// <returns>List of BankAccount objects for the User and Null if user with name not found</returns>
    public List<BankAccount>? GetUserAccounts(long userId)
    {
        log($"GetUserAccounts userid:{userId}");
        if (this.Accounts.ContainsKey(userId))
        {
            return this.Accounts[userId].Values.ToList<BankAccount>();
        }
        return null;
    }

    /// <summary>
    /// System will allow a valid user to create a new account with zero balance
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="accountType"></param>
    /// <returns></returns>
    public BankAccount CreateAccount(long userId, AccountType accountType)
    {
        log($"CreateAccount userid:{userId} accounttype:{accountType}");
        BankAccount bankAccount = new BankAccount()
        {
            AccountId = NextAccountId(),
            AccountNumber = NextAccountNumber(),
            AccountType = accountType,
            CurrentBalance = 0,
            UserId = userId,
            CreatedOn = DateTime.UtcNow
        };

        Dictionary<long, BankAccount> accounts = this.Accounts.ContainsKey(userId) ? this.Accounts[userId]:new();
        accounts.Add(bankAccount.AccountId, bankAccount);
        this.Accounts[userId]= accounts;

        RecordBankTransaction(bankAccount.AccountId, bankAccount.CurrentBalance, userId, BankTransactionType.NewAccount);

        return bankAccount;
    }

    /// <summary>
    /// An account is allowed to be deleted if the account balance is zero or a transferto account Id
    /// is provided to deposit the remaining balance into the transfer to account before deleting the account.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="accountId"></param>
    /// <param name="transferToAccountId"></param>
    /// <returns></returns>
    public BankTransactionStatus DeleteAccount(long userId, long accountId, long transferToAccountId=0)
    {
        log($"DeleteAccount userid:{userId} accountId:{accountId} transferToAccountId:{transferToAccountId}");
        if (this.Accounts.ContainsKey(userId) && this.Accounts[userId].ContainsKey(accountId))
        {
            BankAccount ba = this.Accounts[userId][accountId];
            if (ba.CurrentBalance > 0 && Deposit(userId, transferToAccountId,
                ba.CurrentBalance, BankTransactionType.Settlement) != DepositStatus.Success)
            {
                return BankTransactionStatus.NonZeroBalance;
            }
            RecordBankTransaction(accountId, ba.CurrentBalance, userId,
                BankTransactionType.DeleteAccount);

            this.Accounts[userId].Remove(accountId);
        }
        else
        {
            return BankTransactionStatus.InvalidAccount;
        }

        return BankTransactionStatus.Success;
    }

    /// <summary>
    /// Withdrawal is allowed when a the requested amount is non zero, will not
    /// make the account balance less than $100 and the requested amount is less
    /// than 90% of the current balance.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="accountId"></param>
    /// <param name="withdrawAmount"></param>
    /// <returns></returns>
    public WithdrawalStatus Withdraw(long userId, long accountId, Decimal withdrawAmount)
    {
        log($"Withdraw userid:{userId} accountId:{accountId} withdrawAmount:{withdrawAmount}");
        if (this.Accounts.ContainsKey(userId) && this.Accounts[userId].ContainsKey(accountId))
        {
            BankAccount ba = this.Accounts[userId][accountId];
            if (withdrawAmount <= 0 || (ba.CurrentBalance - withdrawAmount) < 100 || withdrawAmount >= Decimal.Multiply(new Decimal(0.9) , ba.CurrentBalance) )
            {
                RecordBankTransaction(accountId, withdrawAmount, userId, BankTransactionType.Withdrawal, BankTransactionStatus.InvalidAmount);
                return WithdrawalStatus.InvalidAmount;
            }
            ba.CurrentBalance -= withdrawAmount;
            this.Accounts[userId][accountId] = ba;
            RecordBankTransaction(accountId, withdrawAmount, userId, BankTransactionType.Withdrawal);
        }
        else
        {
            RecordBankTransaction(accountId, withdrawAmount, userId, BankTransactionType.Withdrawal, BankTransactionStatus.InvalidAccount);
            return WithdrawalStatus.InvalidAccount;
        }
        return WithdrawalStatus.Success;
    }

    /// <summary>
    /// This method allows bank user to deposit funds. If the passed funds are zero or more than 10000$
    /// System returns invalid deposit amount error.If the passed username or account number is not found
    /// then system throws invalid account error. System will allow exception for amount only when the
    /// deposit request is towards settlement when another account is being closed and balanced is being transfered
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="accountId"></param>
    /// <param name="depositAmount">depositAmount has to be greater than $0 and less than $10,000 </param>
    public DepositStatus Deposit(long userId, long accountId, Decimal depositAmount, BankTransactionType bankTransactionType = BankTransactionType.Deposit)
    {
        log($"Deposit userid:{userId} accountId:{accountId} depositAmount:{depositAmount} bankTransactionType:{bankTransactionType}");
        if (depositAmount<=0 || (bankTransactionType == BankTransactionType.Deposit && depositAmount > 10000)){
            RecordBankTransaction(accountId, depositAmount, userId, bankTransactionType, BankTransactionStatus.InvalidData);
            return DepositStatus.InvalidData;
        }
        if (this.Accounts.ContainsKey(userId) && this.Accounts[userId].ContainsKey(accountId))
        {
            BankAccount ba = this.Accounts[userId][accountId];
            ba.CurrentBalance += depositAmount;
            this.Accounts[userId][accountId] = ba;
            RecordBankTransaction(accountId, depositAmount, userId, bankTransactionType);
        }
        else
        {
            RecordBankTransaction(accountId, depositAmount, userId, bankTransactionType, BankTransactionStatus.InvalidAccount);
            return DepositStatus.InvalidAccount;
        }
        return DepositStatus.Success;
    }

    public void RecordBankTransaction(long accountId, Decimal transactionAmount,
        long userId, BankTransactionType transactionType, BankTransactionStatus transactionStatus = BankTransactionStatus.Success)
    {
        BankTransaction bankTransaction = new BankTransaction()
        {
            AccountId = accountId,
            TransactionAmount = transactionAmount,
            TransType = transactionType,
            UserId = userId,
            TransactionId = NextTransactionId(),
            Status = transactionStatus
        };
        
        log(System.Text.Json.JsonSerializer.Serialize(bankTransaction, typeof(BankTransaction)));
    }


    public void log(String mesg)
    {
        _logger.LogInformation(mesg);
        //Console.WriteLine(mesg);
    }
}
