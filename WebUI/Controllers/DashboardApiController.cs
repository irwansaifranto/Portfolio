using Business.Abstract;
using Business.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebUI.Infrastructure;
using WebUI.Infrastructure.Concrete;
using WebUI.Models.ApiRent;

namespace WebUI.Controllers
{
    [AuthorizeUser(ModuleName = "Dashboard API")]
    public class DashboardApiController : Controller
    {
        //
        // GET: /DashboardApiRent/
        private IApiRentRepository RepoRentApi;

        public DashboardApiController(IApiRentRepository repoRentApi)
        {
            RepoRentApi = repoRentApi;
        }

        public ActionResult Index()
        {
            var model = new ApiRentPresentationStub();
            model.FillStatusOptions();
            model.FillCancelOptions();

            ViewBag.ListStatus = model.GetStatusOptionsAsJson();
            ViewBag.CancelStatus = model.GetCancelOptionsAsJson();

            return View();
        }

        public string Binding()
        {
            //kamus
            GridRequestParameters param = GridRequestParameters.Current;
            int total = 0;
            //penanganan filter null
            if (param.Filters.Filters == null)
            {
                param.Filters.Filters = new List<Business.Infrastructure.FilterInfo>();
                param.Filters.Logic = "and";
            }
            
            List<api_rent> items = RepoRentApi.FindAll(param.Skip, param.Take, (param.Sortings != null ? param.Sortings.ToList() : null), param.Filters);
            total = RepoRentApi.Count(param.Filters);

            return new JavaScriptSerializer().Serialize(new { total = total, data = new ApiRentPresentationStub().MapList(items) });
        }

    }
}
