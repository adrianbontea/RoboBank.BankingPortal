using System.Collections.Generic;
using System.Threading.Tasks;

namespace RoboBank.BankingPortal.Application
{
    public interface IIBANService
    {
        Task<IEnumerable<IBAN>> GetCustomerIBANsAsync(string id);
    }
}