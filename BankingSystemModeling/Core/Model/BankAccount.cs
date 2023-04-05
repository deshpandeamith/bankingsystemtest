using BankingSystemModeling.Core.Enum;

namespace BankingSystemModeling.Core.Model;

public record BankAccount
{
    public long AccountId { get; set; }
    public long UserId { get; set; }
    public long AccountNumber { get; set; }
    public AccountType AccountType { get; set; } = AccountType.Checking;
    public DateTime CreatedOn { get; set; }
    public Decimal CurrentBalance { get; set; } = 0;
    public DateTime DeletedOn { get; set; }
}

