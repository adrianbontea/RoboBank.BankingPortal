using System.Collections.Generic;

namespace RoboBank.BankingPortal.MVC.Models
{
    public class HomeViewModel
    {
        public string UserFirstName { get; set; }

        public IEnumerable<string> FromCurrencies { get; set; }

        public IEnumerable<string> ToCurrencies { get; set; }
    }
}