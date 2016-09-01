using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUI.Models;

namespace WebUI.Binders
{
    public class UserLoginModelBinder : IModelBinder
    {
        private const string SessionKey = "userLogin";

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            UserLogin userLogin = (UserLogin)controllerContext.HttpContext.Session[SessionKey];
            if (userLogin == null)
            {
                string userId = (string)controllerContext.HttpContext.Session["userId"];
                string username = (string)controllerContext.HttpContext.Session["username"];
                string[] rolesArr = (string[])controllerContext.HttpContext.Session["roles"];
                List<string> roles = rolesArr.ToList();

                userLogin = new UserLogin { MobidigUserId = userId, Username = username, Roles = roles };
            }

            return userLogin;
        }
    }
}