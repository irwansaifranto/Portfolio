using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Business.Abstract;
using WebUI.Controllers;
using Business.Entities;

namespace WebUI.Infrastructure
{
    public class LogActionFilter : ActionFilterAttribute
    {
        /// <summary>
        /// uncomment to use log functionality
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //kamus
            //MyController controller = filterContext.Controller as MyController;
            //ILogRepository logRepo;
            //log log;

            ////algoritma
            //if (controller == null)
            //{
            //    throw new InvalidOperationException("There is no YourProperty !!!");
            //}
            //else
            //{
            //    logRepo = controller.RepoLog;

            //    log = GetLog(filterContext.RouteData, filterContext.HttpContext);
            //    logRepo.Save(log);
            //}
        }

        private log GetLog(RouteData routeData, HttpContextBase httpContext)
        {
            string application = "Minibank";
            string username = httpContext.User.Identity.Name;
            string controllerName = routeData.Values["controller"].ToString();
            string actionName = routeData.Values["action"].ToString();
            //string areaName = routeData.Values["area"].ToString();

            string ipAddress = httpContext.Request.UserHostAddress;
            string url = httpContext.Request.Url.PathAndQuery;
            string requestBody = "";
            using (StreamReader reader = new StreamReader(httpContext.Request.InputStream))
            {
                try
                {
                    httpContext.Request.InputStream.Position = 0;
                    requestBody = reader.ReadToEnd();
                }
                catch (Exception ex)
                {
                    requestBody = string.Empty;
                    //log errors
                }
                finally
                {
                    httpContext.Request.InputStream.Position = 0;
                }

                if (requestBody.Contains("password"))
                {
                    requestBody = requestBody.Substring(0, requestBody.IndexOf("password"));
                }
            }

            log log = new log { action = url, application = application, data = requestBody, ip = ipAddress, timestamp = DateTime.Now, user = username };

            return log;
        }
    }
}