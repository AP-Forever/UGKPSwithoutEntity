using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace UGKPSwithoutEntity.Attributes
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        public string UserRole { get; set; }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var b = HttpContext.Current.Session["UserID"] != null ? true : false;
            if ((UserRole != null) && (UserRole == "Admin"))
            {
                if (HttpContext.Current.Session["IsAdmin"] != null)
                {
                    return (bool)HttpContext.Current.Session["IsAdmin"];
                }
                else
                {
                    return false;
                }
                
            }
            return b;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (HttpContext.Current.Session["UserID"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                            new RouteValueDictionary(new { controller = "Account", action = "Login" })
                    );
            }
            else
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
            
        }
    }
}