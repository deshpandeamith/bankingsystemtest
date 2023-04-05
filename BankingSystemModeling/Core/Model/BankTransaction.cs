using System;
using BankingSystemModeling.Core.Enum;

namespace BankingSystemModeling.Core.Model
{
    public class BankTransaction
    {
        public long TransactionId { get; set; }
        public long UserId { get; set; }
        public long AccountId { get; set; }
        public Decimal TransactionAmount { get; set; }
        public BankTransactionType TransType { get; set; } = BankTransactionType.None;
        public BankTransactionStatus Status { get; set; } = BankTransactionStatus.None;
    }
}

