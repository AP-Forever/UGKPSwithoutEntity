using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace UGKPSwithoutEntity
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_BeginRequest()
        {
            var ci = CultureInfo.GetCultureInfo("en-US");

            if (Thread.CurrentThread.CurrentCulture.DisplayName == ci.DisplayName)
            {
                ci = CultureInfo.CreateSpecificCulture("en-US");
                ci.NumberFormat.CurrencyNegativePattern = 1;
                Thread.CurrentThread.CurrentCulture = ci;
                Thread.CurrentThread.CurrentUICulture = ci;
            }
        }
    }
}
