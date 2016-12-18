using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace RoboBank.BankingPortal.Application.Adapters
{
    public class AccountService : IAccountService
    {
        public async Task<Account> GetAccountByIdAsync(string id)
        {
            var httpClient = new HttpClient();
            var accountServiceEndpoint = ConfigurationManager.AppSettings["AccountServiceEndpoint"];
            var response = await httpClient.GetAsync($"{accountServiceEndpoint}/accounts/{id}");

            return await response.Content.ReadAsAsync<Account>();
        }

        public async Task TransferAsync(TransferInfo transferInfo)
        {
            var httpClient = new HttpClient();
            var accountServiceEndpoint = ConfigurationManager.AppSettings["AccountServiceEndpoint"];
            await httpClient.PostAsJsonAsync($"{accountServiceEndpoint}/transfers", transferInfo);
        }
    }
}
