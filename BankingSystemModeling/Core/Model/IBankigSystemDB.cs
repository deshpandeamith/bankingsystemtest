using System;
using BankingSystemModeling.Core.Enum;

namespace BankingSystemModeling.Core.Model
{
	public interface IBankigSystemDB
	{
        public BankUser? GetUser(String userName);
        public List<BankAccount>? GetUserAccounts(long userId);
        public BankAccount CreateAccount(long userId, AccountType accountType);
        public DepositStatus Deposit(long userId, long accountId, Decimal depositAmount, BankTransactionType bankTransactionType = BankTransactionType.Deposit);
        public WithdrawalStatus Withdraw(long userId, long accountId, Decimal withdrawAmount);
        public BankTransactionStatus DeleteAccount(long userId, long accountId, long transferToAccountId = 0);

    }
}

