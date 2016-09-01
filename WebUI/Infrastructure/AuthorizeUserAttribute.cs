using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using WebUI.Binders;
using WebUI.Controllers;
using WebUI.Infrastructure.Concrete;
using WebUI.Models;

namespace WebUI.Infrastructure
{
    public class AuthorizeUserAttribute : AuthorizeAttribute
    {
        // list of module names, separated by ',' 
        // i.e. "Technology Management Home, Technology Mapping", user with module access Technology Management Home or Technology Mapping can access page
        public string ModuleName { get; set; } 

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            if (filterContext.HttpContext.Request.IsAuthenticated)
            {
                //kamus
                List<ModuleAction> moduleAccess;
                List<string> moduleNameList;
                bool hasAccess = true;

                //algoritma
                moduleAccess = (HttpContext.Current.User as CustomPrincipal).Modules;

                if (ModuleName != null)
                {
                    moduleNameList = ModuleName.Split(',').Select(m => m.Trim()).ToList();
                    if (moduleAccess.Where(m => moduleNameList.Contains(m.ModuleName)).Count() == 0)
                    {
                        hasAccess = false;
                    }
                }

                if (!hasAccess)
                {
                   filterContext.Result = new RedirectToRouteResult(new
                        RouteValueDictionary(new { controller = "Error", action = "Http401", area = "", url = filterContext.HttpContext.Request.Url.OriginalString }));
                }
            }
        }
    }
}