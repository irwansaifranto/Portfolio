using Business.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUI.Infrastructure;
using WebUI.Infrastructure.Concrete;

namespace WebUI.Controllers
{
    public class HomeController : MyController
    {
        public HomeController(ILogRepository repoLog)
            : base(repoLog)
        {
        }

        //
        // GET: /Dashboard/

        public ActionResult Index()
        {
            CustomPrincipal user = User as CustomPrincipal;

            if (user.HasAccess("Booking"))
            {
                return RedirectToAction("Index", "Booking");
            }

            return View();
        }

        [AuthorizeUser(ModuleName = "Dashboard")]
        public string TestAuthorize()
        {
            return "authorized";
        }
    }
}
