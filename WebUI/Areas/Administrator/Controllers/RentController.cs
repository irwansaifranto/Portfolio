using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebUI.Models;
using WebUI.Controllers;
using WebUI.Infrastructure;
using WebUI.Extension;
using MvcSiteMapProvider;
using MvcSiteMapProvider.Web.Mvc.Filters;
using Business.Abstract;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System.IO;
using System.Threading;
using Business.Entities;
using Common.Enums;
using WebUI.Models.Booking;
using WebUI.Infrastructure.Concrete;
using WebUI.Models.Customer;
using WebUI.Models.Driver;
using WebUI.Models.CarModel;
using WebUI.Models.Profile;
using WebUI.Models.Car;
using WebUI.Areas.Administrator.Models;

namespace WebUI.Areas.Administrator.Controllers
{
    public class RentController : Controller
    {
        private IRentRepository RepoRent;
        private IOwnerRepository RepoOwner;

        public RentController(IRentRepository repoRent, IOwnerRepository repoOwner)
        {
            RepoRent = repoRent;
            RepoOwner = repoOwner;

        }

        [MvcSiteMapNode(Title = "Dashboard", ParentKey = "Dashboard", Key = "IndexRent")]
        public ActionResult Index()
        {
            //status
            EnumHelper eh = new EnumHelper();
            List<SelectListItem> listStatus = new List<SelectListItem>();

            List<RentStatus> enumList = eh.EnumToList<RentStatus>().ToList();
            foreach (RentStatus single in enumList)
            {
                listStatus.Add(new SelectListItem { Text = eh.GetEnumDescription(single), Value = single.ToString() });
            }

            //viewbag           
            ViewBag.ListStatus = new JavaScriptSerializer().Serialize(listStatus);
            return View("Index");
        }

        public string Binding()
        {
            //kamus
            GridRequestParameters param = GridRequestParameters.Current;
            Business.Infrastructure.FilterInfo filters = param.Filters;
            List<rent> items;
            List<RentPresentationStub> result = new List<RentPresentationStub>();
            int total;

            //algoritma           
            items = RepoRent.FindAll(param.Skip, param.Take, (param.Sortings != null ? param.Sortings.ToList() : null), filters);
            total = RepoRent.Count(param.Filters);
            result = new RentPresentationStub().MapList(items);

            return new JavaScriptSerializer().Serialize(new { total = total, data = result });
        }

    }
}
