using RoboBank.BankingPortal.MVC.Models;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;

namespace RoboBank.BankingPortal.MVC.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var userFirstName = string.Empty;
            var claimsPrincipal = User as ClaimsPrincipal;

            if(claimsPrincipal != null)
            {
                var firstNameClaim = claimsPrincipal.FindFirst(ClaimTypes.GivenName);

                if(firstNameClaim != null)
                {
                    userFirstName = firstNameClaim.Value;
                }
            }
            return View(new HomeViewModel
            {
                UserFirstName = userFirstName,
                FromCurrencies = new[] { "EUR", "USD", "GBP", "RON" },
                ToCurrencies = new[] { "EUR", "USD", "GBP", "RON" }
            });
        }
    }
}