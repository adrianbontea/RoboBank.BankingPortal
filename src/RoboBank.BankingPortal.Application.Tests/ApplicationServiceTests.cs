using Moq;
using Ploeh.AutoFixture.Xunit2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace RoboBank.BankingPortal.Application.Tests
{
    public class ApplicationServiceTests
    {
        [Theory]
        [AutoMoqData]
        public void GetCustomerIBANSShouldThrowExceptionForNotFoundCustomer([Frozen]Mock<ICustomerService> customerServiceMock, ApplicationService sut, string externalId)
        {
            // Arrange
            customerServiceMock.Setup(m => m.GetCustomerByExternalIdAsync(externalId)).Returns(Task.FromResult<Customer> (null));

            // Act
            var ex = Record.ExceptionAsync(() => sut.GetCustomerIBANsAsync(externalId)).Result;

            // Assert
            Assert.NotNull(ex);
            Assert.IsType<NotFoundException>(ex);
        }

        [Theory]
        [AutoMoqData]
        public void GetCustomerIBANsShouldReturnEmptyIfCustomerDoesntHaveAnyAccount([Frozen]Mock<ICustomerService> customerServiceMock, [Frozen]Mock<IIBANService> IBANServiceMock, ApplicationService sut, string externalId, Customer customer)
        {
            // Arrange
            customerServiceMock.Setup(m => m.GetCustomerByExternalIdAsync(externalId)).Returns(Task.FromResult(customer));
            IBANServiceMock.Setup(m => m.GetCustomerIBANsAsync(customer.Id)).Returns(Task.FromResult<IEnumerable<IBAN>>(new List<IBAN>()));

            // Act
            var result = sut.GetCustomerIBANsAsync(externalId).Result;

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Theory]
        [AutoMoqData]
        public void GetCustomerIBANSShouldReturnOneIBANIfCustomerHasOneAccount([Frozen]Mock<ICustomerService> customerServiceMock, [Frozen]Mock<IIBANService> IBANServiceMock, ApplicationService sut, string externalId, Customer customer, IBAN iban)
        {
            // Arrange
            customerServiceMock.Setup(m => m.GetCustomerByExternalIdAsync(externalId)).Returns(Task.FromResult(customer));
            IBANServiceMock.Setup(m => m.GetCustomerIBANsAsync(customer.Id)).Returns(Task.FromResult<IEnumerable<IBAN>>(new[] {iban}));

            // Act
            var result = sut.GetCustomerIBANsAsync(externalId).Result;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(iban, result.First());
        }

        [Theory]
        [AutoMoqData]
        public void GetCustomerIBANsShouldReturnAllIBANsIfCustomerHasMoreAccounts([Frozen]Mock<ICustomerService> customerServiceMock, [Frozen]Mock<IIBANService> IBANServiceMock, ApplicationService sut, string externalId, Customer customer, IBAN iban1, IBAN iban2)
        {
            // Arrange
            customerServiceMock.Setup(m => m.GetCustomerByExternalIdAsync(externalId)).Returns(Task.FromResult(customer));
            IBANServiceMock.Setup(m => m.GetCustomerIBANsAsync(customer.Id)).Returns(Task.FromResult<IEnumerable<IBAN>>(new[] { iban1, iban2 }));

            // Act
            var result = sut.GetCustomerIBANsAsync(externalId).Result;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(iban1, result.First());
            Assert.Equal(iban2, result.Last());
        }

        [Theory]
        [AutoMoqData]
        public void GetCustomerIBANsShouldReturnOneIBANWithValidValueIfCustomerHasOneAccount([Frozen]Mock<ICustomerService> customerServiceMock, [Frozen]Mock<IIBANService> IBANServiceMock, ApplicationService sut, string externalId, Customer customer, IBAN iban)
        {
            // Arrange
            customerServiceMock.Setup(m => m.GetCustomerByExternalIdAsync(externalId)).Returns(Task.FromResult(customer));
            IBANServiceMock.Setup(m => m.GetCustomerIBANsAsync(customer.Id)).Returns(Task.FromResult<IEnumerable<IBAN>>(new[] { iban }));

            // Act
            var result = sut.GetCustomerIBANsAsync(externalId).Result;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(iban.Value, result.First().Value);
        }

        [Theory]
        [AutoMoqData]
        public void GetAccountShouldThrowNotFoundExceptionIfRequestingCustomerDoesntExist([Frozen]Mock<ICustomerService> customerServiceMock, ApplicationService sut, string externalId, string iban)
        {
            // Arrange
            customerServiceMock.Setup(m => m.GetCustomerByExternalIdAsync(externalId)).Returns(Task.FromResult<Customer>(null));

            // Act
            var ex = Record.ExceptionAsync(() => sut.GetAccountAsync(externalId, iban)).Result;

            // Assert
            Assert.NotNull(ex);
            Assert.IsType<NotFoundException>(ex);
        }

        [Theory]
        [AutoMoqData]
        public void GetAccountShouldThrowUnauthorizedAccessExceptionIfRequestingCustomerDoesntOwnAccount([Frozen]Mock<ICustomerService> customerServiceMock, [Frozen]Mock<IIBANService> IBANServiceMock, ApplicationService sut, string externalId, Customer customer, IBAN iban, string requestIban)
        {
            // Arrange
            customerServiceMock.Setup(m => m.GetCustomerByExternalIdAsync(externalId)).Returns(Task.FromResult(customer));
            IBANServiceMock.Setup(m => m.GetCustomerIBANsAsync(customer.Id)).Returns(Task.FromResult<IEnumerable<IBAN>>(new[] { iban }));

            // Act
            var ex = Record.ExceptionAsync(() => sut.GetAccountAsync(externalId, requestIban)).Result;

            // Assert
            Assert.NotNull(ex);
            Assert.IsType<UnauthorizedAccessException>(ex);
        }

        [Theory]
        [AutoMoqData]
        public void GetAccountShouldReturnAccountIfRequestingCustomerOwnsAccount([Frozen]Mock<ICustomerService> customerServiceMock, [Frozen]Mock<IIBANService> IBANServiceMock, [Frozen]Mock<IAccountService> accountServiceMock, ApplicationService sut, string externalId, Customer customer, Account account,  string iban)
        {
            // Arrange
            customerServiceMock.Setup(m => m.GetCustomerByExternalIdAsync(externalId)).Returns(Task.FromResult(customer));
            IBANServiceMock.Setup(m => m.GetCustomerIBANsAsync(customer.Id)).Returns(Task.FromResult<IEnumerable<IBAN>>(new[] { new IBAN { Value = iban} }));
            accountServiceMock.Setup(m => m.GetAccountByIdAsync(iban)).Returns(Task.FromResult(account));

            // Act
            var result = sut.GetAccountAsync(externalId, iban).Result;

            // Assert
            Assert.Equal(account, result);
        }

        [Theory]
        [AutoMoqData]
        public void GetAccountShouldReturnAccountWithIbanCurrencyAndBalanceIfRequestingCustomerOwnsAccount([Frozen]Mock<ICustomerService> customerServiceMock, [Frozen]Mock<IIBANService> IBANServiceMock, [Frozen]Mock<IAccountService> accountServiceMock, ApplicationService sut, string externalId, Customer customer, Account account, string iban)
        {
            // Arrange
            customerServiceMock.Setup(m => m.GetCustomerByExternalIdAsync(externalId)).Returns(Task.FromResult(customer));
            IBANServiceMock.Setup(m => m.GetCustomerIBANsAsync(customer.Id)).Returns(Task.FromResult<IEnumerable<IBAN>>(new[] { new IBAN { Value = iban } }));
            accountServiceMock.Setup(m => m.GetAccountByIdAsync(iban)).Returns(Task.FromResult(account));

            // Act
            var result = sut.GetAccountAsync(externalId, iban).Result;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(account.Id, result.Id);
            Assert.Equal(account.Currency, result.Currency);
            Assert.Equal(account.Balance, result.Balance);
        }

        [Theory]
        [AutoMoqData]
        public void TransferShouldThrowNotFoundExceptionIfSourceAccountIsNotFound([Frozen]Mock<IAccountService> accountServiceMock, ApplicationService sut, TransferInfo transferInfo)
        {
            // Arrange
            accountServiceMock.Setup(m => m.GetAccountByIdAsync(transferInfo.SourceAccountId)).Returns(Task.FromResult<Account>(null));

            // Act
            var ex = Record.ExceptionAsync(() => sut.TansferAsync(null, transferInfo)).Result;

            // Assert
            Assert.NotNull(ex);
            Assert.IsType<NotFoundException>(ex);
            Assert.Contains("source", ex.Message);
        }

        [Theory]
        [AutoMoqData]
        public void TransferShouldThrowNotFoundExceptionIfTargetAccountIsNotFound([Frozen]Mock<IAccountService> accountServiceMock, ApplicationService sut, TransferInfo transferInfo)
        {
            // Arrange
            accountServiceMock.Setup(m => m.GetAccountByIdAsync(transferInfo.SourceAccountId)).Returns(Task.FromResult(new Account()));
            accountServiceMock.Setup(m => m.GetAccountByIdAsync(transferInfo.TargetAccountId)).Returns(Task.FromResult<Account>(null));

            // Act
            var ex = Record.ExceptionAsync(() => sut.TansferAsync(null, transferInfo)).Result;

            // Assert
            Assert.NotNull(ex);
            Assert.IsType<NotFoundException>(ex);
            Assert.Contains("target", ex.Message);
        }

        [Theory]
        [AutoMoqData]
        public void TransferShouldThrowNotFoundExceptionIfCustomerIsNotFound([Frozen]Mock<IAccountService> accountServiceMock, [Frozen]Mock<ICustomerService> customerServiceMock, ApplicationService sut, TransferInfo transferInfo, string externalId)
        {
            // Arrange
            accountServiceMock.Setup(m => m.GetAccountByIdAsync(transferInfo.SourceAccountId)).Returns(Task.FromResult(new Account()));
            accountServiceMock.Setup(m => m.GetAccountByIdAsync(transferInfo.TargetAccountId)).Returns(Task.FromResult(new Account()));
            customerServiceMock.Setup(m => m.GetCustomerByExternalIdAsync(externalId)).Returns(Task.FromResult<Customer>(null));

            // Act
            var ex = Record.ExceptionAsync(() => sut.TansferAsync(externalId, transferInfo)).Result;

            // Assert
            Assert.NotNull(ex);
            Assert.IsType<NotFoundException>(ex);
            Assert.Contains("customer", ex.Message);
        }

        [Theory]
        [AutoMoqData]
        public void TransferShouldThrowUnauthorizedAccessExceptionIfCustomerDoesntOwnSourceAccount([Frozen]Mock<IAccountService> accountServiceMock, [Frozen]Mock<ICustomerService> customerServiceMock, ApplicationService sut, TransferInfo transferInfo, string externalId)
        {
            // Arrange
            accountServiceMock.Setup(m => m.GetAccountByIdAsync(transferInfo.SourceAccountId)).Returns(Task.FromResult(new Account { Id = transferInfo.SourceAccountId, CustomerId = "X" }));
            accountServiceMock.Setup(m => m.GetAccountByIdAsync(transferInfo.TargetAccountId)).Returns(Task.FromResult(new Account()));
            customerServiceMock.Setup(m => m.GetCustomerByExternalIdAsync(externalId)).Returns(Task.FromResult(new Customer { Id = "One" }));

            // Act
            var ex = Record.ExceptionAsync(() => sut.TansferAsync(externalId, transferInfo)).Result;

            // Assert
            Assert.NotNull(ex);
            Assert.IsType<UnauthorizedAccessException>(ex);
        }

        [Theory]
        [AutoMoqData]
        public void TransferShouldThrowInvalidOperationExceptionIfTargetAccountCurrencyDoesntMatchTransferCurrency([Frozen]Mock<IAccountService> accountServiceMock, [Frozen]Mock<ICustomerService> customerServiceMock, ApplicationService sut, TransferInfo transferInfo, string externalId)
        {
            // Arrange
            accountServiceMock.Setup(m => m.GetAccountByIdAsync(transferInfo.SourceAccountId)).Returns(Task.FromResult(new Account { Id = transferInfo.SourceAccountId, CustomerId = "one" }));
            accountServiceMock.Setup(m => m.GetAccountByIdAsync(transferInfo.TargetAccountId)).Returns(Task.FromResult(new Account { Currency = "USD" }));
            customerServiceMock.Setup(m => m.GetCustomerByExternalIdAsync(externalId)).Returns(Task.FromResult(new Customer { Id = "one" }));

            // Act
            var ex = Record.ExceptionAsync(() => sut.TansferAsync(externalId, transferInfo)).Result;

            // Assert
            Assert.NotNull(ex);
            Assert.IsType<InvalidOperationException>(ex);
            Assert.Contains("currency", ex.Message);
        }

        [Theory]
        [AutoMoqData]
        public void TransferShouldThrowInvalidOperationExceptionIfAmountIsLessOrEqualToZero([Frozen]Mock<IAccountService> accountServiceMock, [Frozen]Mock<ICustomerService> customerServiceMock, ApplicationService sut, TransferInfo transferInfo, string externalId)
        {
            // Arrange
            accountServiceMock.Setup(m => m.GetAccountByIdAsync(transferInfo.SourceAccountId)).Returns(Task.FromResult(new Account { Id = transferInfo.SourceAccountId, CustomerId = "one" }));
            accountServiceMock.Setup(m => m.GetAccountByIdAsync(transferInfo.TargetAccountId)).Returns(Task.FromResult(new Account { Currency = transferInfo.Currency }));
            customerServiceMock.Setup(m => m.GetCustomerByExternalIdAsync(externalId)).Returns(Task.FromResult(new Customer { Id = "one" }));
            transferInfo.Amount = -1;

            // Act
            var ex = Record.ExceptionAsync(() => sut.TansferAsync(externalId, transferInfo)).Result;

            // Assert
            Assert.NotNull(ex);
            Assert.IsType<InvalidOperationException>(ex);
            Assert.Contains("amount", ex.Message);
        }

        [Theory]
        [AutoMoqData]
        public void TransferShouldSendTransferInfoToAccountService([Frozen]Mock<IAccountService> accountServiceMock, [Frozen]Mock<ICustomerService> customerServiceMock, ApplicationService sut, TransferInfo transferInfo, string externalId)
        {
            // Arrange
            accountServiceMock.Setup(m => m.GetAccountByIdAsync(transferInfo.SourceAccountId)).Returns(Task.FromResult(new Account { Id = transferInfo.SourceAccountId, CustomerId = "one" }));
            accountServiceMock.Setup(m => m.GetAccountByIdAsync(transferInfo.TargetAccountId)).Returns(Task.FromResult(new Account { Currency = transferInfo.Currency }));
            accountServiceMock.Setup(m => m.TransferAsync(It.IsAny<TransferInfo>())).Returns(Task.CompletedTask);
            customerServiceMock.Setup(m => m.GetCustomerByExternalIdAsync(externalId)).Returns(Task.FromResult(new Customer { Id = "one" }));
            transferInfo.Amount = 100;

            // Act
            sut.TansferAsync(externalId, transferInfo).Wait();

            // Assert
            accountServiceMock.Verify(m => m.TransferAsync(transferInfo));
        }
    }
}
