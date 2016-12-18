using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoboBank.BankingPortal.Application
{
    public class ApplicationService
    {
        private readonly ICustomerService _customerService;
        private readonly IIBANService _IBANService;
        private readonly IAccountService _accountService;

        public ApplicationService (ICustomerService customerService, IIBANService IBANService, IAccountService accountService)
        {
            _customerService = customerService;
            _IBANService = IBANService;
            _accountService = accountService;
        }

        public async Task<IEnumerable<IBAN>> GetCustomerIBANsAsync(string externalId)
        {
            var customer = await _customerService.GetCustomerByExternalIdAsync(externalId);

            if (customer == null)
            {
                throw new NotFoundException($"The customer with external id = {externalId} was not found.");
            }

            return await _IBANService.GetCustomerIBANsAsync(customer.Id);
        }

        public async Task<Account> GetAccountAsync(string externalId, string iban)
        {
            var customer = await _customerService.GetCustomerByExternalIdAsync(externalId);

            if (customer == null)
            {
                throw new NotFoundException($"The customer with external id = {externalId} was not found.");
            }

            var ibans = await _IBANService.GetCustomerIBANsAsync(customer.Id);

            var foundIban = ibans.FirstOrDefault(ibn => ibn.Value == iban);

            if (foundIban == null)
            {
                throw new UnauthorizedAccessException();
            }

            return await _accountService.GetAccountByIdAsync(foundIban.Value);
        }

        public async Task TansferAsync(string externalId, TransferInfo transferInfo)
        {
            var sourceAccount = await _accountService.GetAccountByIdAsync(transferInfo.SourceAccountId);

            if (sourceAccount == null)
            {
                throw new NotFoundException("source");
            }

            var targetAccount = await _accountService.GetAccountByIdAsync(transferInfo.TargetAccountId);

            if (targetAccount == null)
            {
                throw new NotFoundException("target");
            }

            var customer = await _customerService.GetCustomerByExternalIdAsync(externalId);

            if (customer == null)
            {
                throw new NotFoundException("customer");
            }

            if (sourceAccount.CustomerId != customer.Id)
            {
                throw new UnauthorizedAccessException();
            }

            if (transferInfo.Currency != targetAccount.Currency)
            {
                throw new InvalidOperationException("currency");
            }

            if (transferInfo.Amount <= 0)
            {
                throw new InvalidOperationException("amount");
            }

            await _accountService.TransferAsync(transferInfo);
        }
    }
}