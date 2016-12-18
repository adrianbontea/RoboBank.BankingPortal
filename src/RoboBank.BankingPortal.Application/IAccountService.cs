using System.Threading.Tasks;

namespace RoboBank.BankingPortal.Application
{
    public interface IAccountService
    {
        Task<Account> GetAccountByIdAsync(string id);

        Task TransferAsync(TransferInfo transferInfo);
    }
}