namespace BankingSystemModeling.Core.Model;

public class BankUser
{
    public long UserId { get; set; }
    public String UserName { get; set; } = String.Empty;
    public String FirstName { get; set; } = String.Empty;
    public String LastName { get; set; } = String.Empty;
    public bool IsActive { get; set; }
}  






