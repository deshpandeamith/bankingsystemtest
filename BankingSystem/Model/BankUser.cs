namespace BankingSystem.Model;

public record BankUser
{
    public long UserId { get; set; }
    public String? UserName { get; set; }
    public String? FirstName { get; set; }
    public String? LastName { get; set; }
    public bool IsActive { get; set; }
}  

public record BankAccount 
{
    public long AccountId { get; set; }
    public long UserId { get; set; }
    public long AccountNumber { get; set; }
    public String? AccountNickName { get; set; }
    public String? AccountType { get; set; }
    public DateTime CreatedOn { get; set; }
    public Decimal CurrentBalance { get; set; }
    public bool IsActive { get; set; }
    public DateTime DeletedOn { get; set; }
}

public enum TransactionType
{
    Deposit,
    Withdrawal
}
public enum TransactionStatus
{
    Success,
    Failure
}
public record BankTransaction
{
    public long TransactionId { get; set; }
    public long UserId { get; set; }
    public long AccountId { get; set; }
    public Decimal TransactionAmount { get; set; }
    public TransactionType TransType { get; set; }
    public TransactionStatus Status { get; set; }
}

