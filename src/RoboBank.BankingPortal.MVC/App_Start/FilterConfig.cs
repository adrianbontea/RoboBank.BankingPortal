﻿using RoboBank.BankingPortal.MVC.Custom;
using System.Web.Mvc;

namespace RoboBank.BankingPortal.MVC
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new AuthorizeAttribute());
            filters.Add(new AIHandleErrorAttribute());
        }
    }
}
