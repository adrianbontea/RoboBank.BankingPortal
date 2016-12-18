using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RoboBank.BankingPortal.Application.Adapters
{
    public class IBANService : IIBANService
    {
        public async Task<IEnumerable<IBAN>> GetCustomerIBANsAsync(string id)
        {
            var httpClient = new HttpClient();
            var accountServiceEndpoint = ConfigurationManager.AppSettings["AccountServiceEndpoint"];
            var response = await httpClient.GetAsync($"{accountServiceEndpoint}/customers/{id}/accounts");

            var result = await response.Content.ReadAsAsync<IEnumerable<AccountModel>>();
            return result.Select(acc => new IBAN { Value = acc.Id }).ToList();
        }
    }
}
