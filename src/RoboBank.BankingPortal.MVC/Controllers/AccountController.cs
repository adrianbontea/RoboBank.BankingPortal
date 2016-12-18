using RoboBank.BankingPortal.Application;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace RoboBank.BankingPortal.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationService _applicationService;

        public AccountController (ApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        public async Task<ActionResult> MyAccounts()
        {
            return View(await _applicationService.GetCustomerIBANsAsync(User.Identity.Name));
        }


        public async Task<ActionResult> AccountDetails(string iban)
        {
            return PartialView("_AccountDetails", await _applicationService.GetAccountAsync(User.Identity.Name, iban));
        }

        [HttpGet]
        public async Task<ActionResult> Transfer()
        {
            return View(await _applicationService.GetCustomerIBANsAsync(User.Identity.Name));
        }

        [HttpPost]
        public async Task<ActionResult> Transfer(TransferInfo transferInfo)
        {
            await _applicationService.TansferAsync(User.Identity.Name, transferInfo);
            return RedirectToAction("Transfer");
        }
    }
}