using BankingSystem.Common;
using BankingSystem.Model;
using Microsoft.AspNetCore.SignalR;
using System.Security.Cryptography;

namespace BankingSystem.DAL
{

    public class DataAccessLayer
    {
        // initialize database
        // a collection for users
        // a collection for accounts
        // a collection for transactions
        private static readonly Object _lock = new();
        private static DataAccessLayer? _instance;

        private Dictionary<String, BankUser> Users = new();
        private Dictionary<String, Dictionary<int, BankAccount>> Accounts = new();
        private List<BankTransaction> Transactions = new();

        private DataAccessLayer()
        { }
        public static DataAccessLayer Instance { 
            get { 
                lock(_lock)
                {
                    if (_instance == null) {
                        _instance = new DataAccessLayer();
                        _instance.InitializeData();
                    } 
                }
                return _instance;
            } 
        }
        private void InitializeData()
        {
            Random random = Random.Shared;
            // Just initializing a set of users
            Users.Add("djohn", new BankUser
            {
                UserId = random.Next(1000000, 9999999),
                UserName = "djohn",
                FirstName = "John",
                LastName = "Doe",
                IsActive = true
            });
            Users.Add("rsteven", new BankUser
            {
                UserId = random.Next(1000000, 9999999),
                UserName = "rsteven",
                FirstName = "Steven",
                LastName = "Roth",
                IsActive = true
            });

        }


        /// <summary>
        /// Retrieve user information using username
        /// </summary>
        /// <param name="userName">UserName of the </param>
        /// <returns>BankUser object with the userinformation for the passed username else returns null</returns>
        public BankUser? GetUser(String userName)
        {
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
        public List<BankAccount>? GetUserAccounts(String userName)
        {
            if (this.Accounts.ContainsKey(userName))
            {
                return this.Accounts[userName].Values.ToList<BankAccount>();
            }
            return null;
        }
        // create account -- User need to provide minimum of 100$ for opening an account
        // -- The minimum deposit needed can be from cash
        // -- The minimum deposit needed can be from another account

        // delete account -- need the user to select another account to transfer the funds before deleting or 
        // -- if the account has funds, a withdrawal or transfer is allowed to zero the balance before delete
        // -- 
        
        // withdraw funds
        // -- a minimum of $100 is needed in the account
        // -- only a 90% of the balance is allowed to be withdrawn
        
        // deposit funds
        // -- user cannot deposit more than 10000$ in a single transaction
        /// <summary>
        /// This method allows bank user to deposit funds. If the passed funds are zero or more than 10000$
        /// System returns invalid deposit amount error.If the passed username or account number is not found
        /// then system throws invalid account error
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="accountNumber"></param>
        /// <param name="depositAmount">depositAmount has to be greater than $0 and less than $10,000 </param>
        public DepositStatus Deposit(String userName, int accountNumber, Decimal depositAmount)
        {
            // check to see if the deposit is one of the allowed values if not return corresponding status
            if (depositAmount==0 || depositAmount > 10000){
                return DepositStatus.INVALID_DATA;
            }
            if (this.Accounts.ContainsKey(userName) && this.Accounts[userName].ContainsKey(accountNumber))
            {
                BankAccount ba = this.Accounts[userName][accountNumber];
                ba.CurrentBalance += depositAmount;
                //this.Accounts[userName][accountNumber] = ba;
            }
            else
            {
                return DepositStatus.INVALID_ACCOUNT;
            }
            return DepositStatus.SUCCESS;
        }
    }
}
