using System.Web;
using System.Web.Mvc;

namespace RoboBank.BankingPortal.MVC.Controllers
{
    public class UserController : Controller
    {
        [Authorize]
        public ActionResult Logout()
        {
            Request.GetOwinContext().Authentication.SignOut();
            return Redirect("/");
        }
    }
}