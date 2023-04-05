using Moq;
using BankingSystemModeling.Core.Model;
using BankingSystemModeling.Controllers;
using FluentAssertions;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using BankingSystemModeling.Core.Enum;

namespace BankingSystemModelingUnitTesting;

public class BankingSystemUnitTests
{
    Mock<IBankigSystemDB> bankingSystemDb = new Mock<IBankigSystemDB>();


    [Fact]
    public void BankingSystemTest_Get_ShouldReturn200OKStatus()
    {
        // Arrange
        String userName = "djohn";
        bankingSystemDb.Setup(p => p.GetUser(It.IsAny<String>())).Returns(MockData.GetBankUser());
        var sut = new BankingSystemTestController(bankingSystemDb.Object);
        // Act
        var result = (OkObjectResult) sut.GetUser(userName);
        // Assert
        result.StatusCode.Should().Be(200);
    }
    [Fact]
    public void BankingSystemTest_Get_ShouldReturn404NotFoundStatus()
    {
        // Arrange
        String userName = "djohnaa";
        var sut = new BankingSystemTestController(bankingSystemDb.Object);
        // Act
        var result = sut.GetUser(userName);
        // Assert
        result.GetType().Should().Be(typeof(NotFoundResult));
    }
    [Fact]
    public void BankingSystemTest_Get_ShouldCallDataSourceOnce()
    {
        // Arrange
        String userName = "djohn";
        bankingSystemDb.Setup(p => p.GetUser(It.IsAny<String>())).Returns(MockData.GetBankUser());
        var sut = new BankingSystemTestController(bankingSystemDb.Object);
        // Act
        var result = (OkObjectResult)sut.GetUser(userName);
        // Assert
        bankingSystemDb.Verify(a => a.GetUser(It.IsAny<String>()), Times.Once);
    }


    // get accounts
    [Fact]
    public void GetAccounts_ValidAccount_ReturnsEmptyAccountsList()
    {
        // Arrange
        long userId = 1;
        bankingSystemDb.Setup(p => p.GetUserAccounts(It.IsAny<long>())).Returns(() => {
            return new List<BankAccount>();
        });
        var sut = new BankingSystemTestController(bankingSystemDb.Object);
        // Act
        var result = (OkObjectResult)sut.GetAccounts(userId);
        // Assert
        result.StatusCode.Should().Be(200);
    }
    [Fact]
    public void GetAccounts_InvalidAccount_Returns404NotFoundStatus()
    {
        // Arrange
        long userId = 5;
        var sut = new BankingSystemTestController(bankingSystemDb.Object);
        // Act
        var result = sut.GetAccounts(userId);
        // Assert
        result.GetType().Should().Be(typeof(NotFoundResult));
        //result.StatusCode.Should().Be(200);
    }

    // create account // CreateAccount(long userId, AccountType accountType)
    [Fact]
    public void CreateAccount_ValidUser_AccountReturnedWith200OKStatus()
    {
        // Arrange
        long userId = 1;
        bankingSystemDb.Setup(p => p.CreateAccount(It.IsAny<long>(),It.IsAny<AccountType>())).Returns(() => {
            return new BankAccount()
            {
                AccountId = 1,
                AccountNumber = 11,
                AccountType = AccountType.Checking,
                CurrentBalance = 0,
                UserId = 1,
                CreatedOn = DateTime.UtcNow
            };
        });
        var sut = new BankingSystemTestController(bankingSystemDb.Object);
        // Act
        var result = (OkObjectResult)sut.CreateAccount(userId, AccountType.Checking);
        // Assert
        result.StatusCode.Should().Be(200);
    }

    [Fact]
    public void CreateAccount_InvalidUser_Returns404NotFoundStatus()
    {
        // Arrange
        long userId = 1;
        //bankingSystemDb.Setup(p => p.CreateAccount(It.IsAny<long>(), It.IsAny<AccountType>())).Returns(MockData.GetNewAccount());
        var sut = new BankingSystemTestController(bankingSystemDb.Object);
        // Act
        var result = sut.CreateAccount(userId, AccountType.Checking);
        // Assert
        result.GetType().Should().Be(typeof(NotFoundResult));
    }

    // deposit amount // DepositStatus Deposit(long userId, long accountId, Decimal depositAmount, BankTransactionType bankTransactionType = BankTransactionType.Deposit);

    [Fact]
    public void Deposit_ValidUser_ValidAccount_Returns200OKStatus()
    {
        // Arrange
        long userId = 1;
        bankingSystemDb.Setup(p => p.Deposit(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<Decimal>(), It.IsAny<BankTransactionType>())).Returns(() => { return DepositStatus.Success; });
        var sut = new BankingSystemTestController(bankingSystemDb.Object);
        // Act
        var result = (OkObjectResult)sut.Deposit(userId, 1, 100);
        // Assert
        result.StatusCode.Should().Be(200);
    }

    [Theory]
    [InlineData(DepositStatus.InvalidAccount)]
    [InlineData(DepositStatus.InvalidData)]
    [InlineData(DepositStatus.InvalidDepositAmount)]
    public void Deposit_ValidUser_ValidAccount_InvalidDepositAmount_Returns400BadRequestStatus(DepositStatus depositStatus)
    {
        // Arrange
        bankingSystemDb.Setup(p => p.Deposit(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<Decimal>(), It.IsAny<BankTransactionType>())).Returns(() => { return depositStatus; });
        var sut = new BankingSystemTestController(bankingSystemDb.Object);
        // Act
        var result = sut.Deposit(1, 1, 100);
        // Assert
        result.GetType().Should().Be(typeof(BadRequestObjectResult));
    }

    // withdraw amount // WithdrawalStatus Withdraw(long userId, long accountId, Decimal withdrawAmount);

    [Fact]
    public void Withdraw_ValidUser_ValidAccount_ValidWithdrawAmount_Returns200OKStatus()
    {
        // Arrange
        bankingSystemDb.Setup(p => p.Withdraw(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<Decimal>())).Returns(() => { return WithdrawalStatus.Success; });
        var sut = new BankingSystemTestController(bankingSystemDb.Object);
        // Act
        var result = (OkObjectResult)sut.Withdraw(1, 1, 100);
        // Assert
        result.StatusCode.Should().Be(200);
    }

    [Theory]
    [InlineData(WithdrawalStatus.InvalidAccount)]
    [InlineData(WithdrawalStatus.InvalidAmount)]
    public void Withdraw_ValidUser_ValidAccount_InvalidDepositAmount_Returns400BadRequestStatus(WithdrawalStatus withdrawalStatus)
    {
        // Arrange
        bankingSystemDb.Setup(p => p.Withdraw(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<Decimal>())).Returns(() => { return withdrawalStatus; });
        var sut = new BankingSystemTestController(bankingSystemDb.Object);
        // Act
        var result = sut.Deposit(1, 1, 100);
        // Assert
        result.GetType().Should().Be(typeof(BadRequestObjectResult));
    }


    // delete account  //BankTransactionStatus DeleteAccount(long userId, long accountId, long transferToAccountId = 0);

    [Fact]
    public void DeleteAccount_ValidUser_ValidAccount_AnyBalanceAmount_Returns200OKStatus()
    {
        // Arrange
        bankingSystemDb.Setup(p => p.DeleteAccount(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<long>())).Returns(() => { return BankTransactionStatus.Success; });
        var sut = new BankingSystemTestController(bankingSystemDb.Object);
        // Act
        var result = (OkObjectResult)sut.DeleteAccount(1, 1, 100);
        // Assert
        result.StatusCode.Should().Be(200);
    }

    [Theory]
    [InlineData(BankTransactionStatus.NonZeroBalance)]
    [InlineData(BankTransactionStatus.InvalidAccount)]
    public void DeleteAccount_AnyUser_AnyAccount_NonZeroBalanceInAccountToBeDeleted_Returns400BadRequestStatus(BankTransactionStatus bankTransactionStatus)
    {
        // Arrange
        bankingSystemDb.Setup(p => p.DeleteAccount(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<long>())).Returns(() => { return bankTransactionStatus; });
        var sut = new BankingSystemTestController(bankingSystemDb.Object);
        // Act
        var result = sut.DeleteAccount(1, 1, 100);
        // Assert
        result.GetType().Should().Be(typeof(BadRequestObjectResult));
    }



}
public class MockData
{

    public static BankUser GetBankUser()
    {
        return new BankUser
        {
            UserId = 1,
            UserName = "djohn",
            FirstName = "John",
            LastName = "Doe",
            IsActive = true
        };
    }

    public static List<BankAccount> GetAccounts()
    {
        return new List<BankAccount>()
        {
            new BankAccount()
            {
                AccountId = 1,
                AccountNumber = 11,
                AccountType = AccountType.Checking,
                CurrentBalance = 0,
                UserId = 1,
                CreatedOn = DateTime.UtcNow
            },
            new BankAccount()
            {
                AccountId = 2,
                AccountNumber = 22,
                AccountType = AccountType.Savings,
                CurrentBalance = 0,
                UserId = 1,
                CreatedOn = DateTime.UtcNow
            }
        };
    }

}