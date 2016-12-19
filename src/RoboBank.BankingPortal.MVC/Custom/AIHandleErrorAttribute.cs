using Microsoft.ApplicationInsights;
using System;
using System.Web.Mvc;

namespace RoboBank.BankingPortal.MVC.Custom
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class AIHandleErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext != null && filterContext.HttpContext != null && filterContext.Exception != null)
            {
                //If customError is Off, then AI HTTPModule will report the exception
                if (filterContext.HttpContext.IsCustomErrorEnabled)
                {   
                    var telemetryClient = new TelemetryClient();
                    telemetryClient.TrackException(filterContext.Exception);
                }
            }
            base.OnException(filterContext);
        }
    }
}