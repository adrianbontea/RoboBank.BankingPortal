using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace RoboBank.BankingPortal.Application.Adapters
{
    public class CustomerService : ICustomerService
    {
        public async Task<Customer> GetCustomerByExternalIdAsync(string externalId)
        {
            var httpClient = new HttpClient();
            var customerServiceEndpoint = ConfigurationManager.AppSettings["CustomerServiceEndpoint"];
            var response = await httpClient.GetAsync($"{customerServiceEndpoint}/customers/externalId/{externalId}");

            return await response.Content.ReadAsAsync<Customer>();
        }
    }
}
