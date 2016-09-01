using Business.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebUI.Models;

namespace WebUI.Controllers
{
    public class ErrorController : MyController
    {
        public ErrorController(ILogRepository repoLog)
            : base(repoLog)
        {
        }

        public ActionResult Http404(string url)
        {
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            var model = new ErrorPageViewModel();
            // If the url is relative ('NotFound' route) then replace with Requested path
            model.RequestedUrl = Request.Url.OriginalString.Contains(url) & Request.Url.OriginalString != url ?
                Request.Url.OriginalString : url;
            // Dont get the user stuck in a 'retry loop' by
            // allowing the Referrer to be the same as the Request
            model.ReferrerUrl = Request.UrlReferrer != null &&
                Request.UrlReferrer.OriginalString != model.RequestedUrl ?
                Request.UrlReferrer.OriginalString : null;

            // TODO: insert ILogger here

            return View("NotFound", model);
        }

        public ActionResult Http403(string url)
        {
            ErrorPageViewModel model = new ErrorPageViewModel();

            Response.StatusCode = (int)HttpStatusCode.Forbidden;
            model.RequestedUrl = Request.Url.OriginalString.Contains(url) & Request.Url.OriginalString != url ?
                Request.Url.OriginalString : url;
            model.ReferrerUrl = Request.UrlReferrer != null &&
                Request.UrlReferrer.OriginalString != model.RequestedUrl ?
                Request.UrlReferrer.OriginalString : null;

            return View("Forbidden", model);
        }

        #region access denied

        public ActionResult Http401(string url)
        {
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            var model = new ErrorPageViewModel();
            // If the url is relative ('NotFound' route) then replace with Requested path
            model.RequestedUrl = Request.Url.OriginalString.Contains(url) & Request.Url.OriginalString != url ?
                Request.Url.OriginalString : url;
            // Dont get the user stuck in a 'retry loop' by
            // allowing the Referrer to be the same as the Request
            model.ReferrerUrl = Request.UrlReferrer != null &&
                Request.UrlReferrer.OriginalString != model.RequestedUrl ?
                Request.UrlReferrer.OriginalString : null;

            // TODO: insert ILogger here

            return View("AccessDenied", model);
        }

        #endregion
    }
}
