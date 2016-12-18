using System.Threading.Tasks;

namespace RoboBank.BankingPortal.Application
{
    public interface ICustomerService
    {
        Task<Customer> GetCustomerByExternalIdAsync(string externalId);
    }
}