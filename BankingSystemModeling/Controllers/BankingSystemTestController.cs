using Microsoft.AspNetCore.Mvc;
using BankingSystemModeling.Core.Model;
using BankingSystemModeling.Core.Enum;

namespace BankingSystemModeling.Controllers;

[Route("[controller]")]
[ApiController]
public class BankingSystemTestController : ControllerBase
{
    private readonly IBankigSystemDB bankigSystemDB;

    public BankingSystemTestController(IBankigSystemDB _bankigSystemDB)
    {
        bankigSystemDB = _bankigSystemDB;
    }

    /// <summary>
    /// Retrieve Bank User object for a given user whose usern name is available
    /// </summary>
    /// <param name="userName"></param>
    /// <returns>Returns BankUser object if a user with the passed username is
    /// found else returns null</returns>
    // GET: api/<BankingSystemTest>/djohn
    [HttpGet("{userName}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BankUser))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetUser(String userName)
    {
        var user = bankigSystemDB.GetUser(userName);
        return (user == null) ? NotFound() : Ok(user);
    }

    /// <summary>
    /// Retrieve list of Bank Accounts for the user whose user Id is passed
    /// </summary>
    /// <param name="userId">The Id of the bank user</param>
    /// <returns>Returns a list of Bank Accounts if a user with the passed userId is found</returns>
    [HttpGet("{userId}/Accounts")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<BankAccount>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetAccounts(long userId)
    {
        var bankAccounts = bankigSystemDB.GetUserAccounts(userId);
        return (bankAccounts == null) ? NotFound() : Ok(bankAccounts);
    }

    /// <summary>
    /// API to create a new bank account for a user. 
    /// </summary>
    /// <param name="userId">The Id of the bank user requesting to create account</param>
    /// <param name="accountType">Type of account to be created</param>
    /// <returns>Retruns the created account information if successful</returns>
    [HttpPut("{userId}/Accounts")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BankAccount))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult CreateAccount(long userId, AccountType accountType)
    {
        var bankAccount = bankigSystemDB.CreateAccount(userId, accountType);
        return (bankAccount == null) ? NotFound() : Ok(bankAccount);
    }

    /// <summary>
    /// API is used to deposit funds into the bank account. A non zero and an amount
    /// less than or equal to $10000 is allowed to be deposited in a single transaction
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="accountId"></param>
    /// <param name="depositAmount"></param>
    /// <returns></returns>
    [HttpPost("{userId}/Accounts/{accountId}/deposit")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DepositStatus))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DepositStatus))]
    public IActionResult Deposit(long userId, long accountId, Decimal depositAmount)
    {
        var depositStatus = bankigSystemDB.Deposit(userId, accountId, depositAmount);
        return (depositStatus == DepositStatus.Success) ? Ok(depositStatus):BadRequest(depositStatus);
    }

    /// <summary>
    /// API is used to withdraw funds from a bank account. A non zero amount that
    /// meets a minimum balance of 100$ in the account is allowed. Also the withdrawal
    /// amount has to be less than 90% of the total balance in the account.
    /// </summary>
    /// <param name="userId">The user Id of the owner of the bank account</param>
    /// <param name="accountId">The bank account Id</param>
    /// <param name="withdrawAmount">The amount to withdraw</param>
    /// <returns>Success if the withdrawal conditions are met else return bad request</returns>
    [HttpPost("{userId}/Accounts/{accountId}/withdraw")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WithdrawalStatus))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(WithdrawalStatus))]
    public IActionResult Withdraw(long userId, long accountId, Decimal withdrawAmount)
    {
        var withdrawalStatus = bankigSystemDB.Withdraw(userId, accountId, withdrawAmount);
        return (withdrawalStatus == WithdrawalStatus.Success) ? Ok(withdrawalStatus) : BadRequest(withdrawalStatus);
    }

    /// <summary>
    /// API is used to delete an account. A transfer to account Id is required if
    /// the account to be delete has a non zero balance. A account with non zero
    /// balance will not be allowed to delete if no transfer to account is available.
    /// </summary>
    /// <param name="userId">User Id of the bank user</param>
    /// <param name="accountId">Account Id which need to be deleted</param>
    /// <param name="transferToAccountId">Account to which the non zero balance
    /// from to be deleted account needs to be transferred to</param>
    /// <returns>If the account to be deleted has zero balance or a transfer to
    /// account id is provided to transfer the remaining balance, system will allow
    /// account deletion. Otherwise system will not allow deletion</returns>
    [HttpDelete("{userId}/Accounts/{accountId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BankTransactionStatus))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BankTransactionStatus))]
    public IActionResult DeleteAccount(long userId, long accountId, long transferToAccountId = 0)
    {
        var transactionStatus = bankigSystemDB.DeleteAccount(userId, accountId, transferToAccountId);
        return (transactionStatus == BankTransactionStatus.Success) ? Ok(transactionStatus) : BadRequest(transactionStatus);
    }

}
