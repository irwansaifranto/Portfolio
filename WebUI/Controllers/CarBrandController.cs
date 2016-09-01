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
using WebUI.Models.CarBrand;

namespace WebUI.Controllers
{
    [AuthorizeUser(ModuleName = "Car Brand")]
    public class CarBrandController : MyController
    {
		private ICarBrandRepository RepoCarBrand;
		private ILogRepository RepoLog;

        public CarBrandController(ICarBrandRepository repoCarBrand, ILogRepository repoLog)
			: base(repoLog)
        {
            RepoCarBrand = repoCarBrand;
        }

		[MvcSiteMapNode(Title = "Car Brand", ParentKey = "Dashboard",Key="IndexCarBrand")]
		[SiteMapTitle("Breadcrumb")]
        public ActionResult Index()
        {
            return View();
        }

        public string Binding()
        {
            //kamus
            GridRequestParameters param = GridRequestParameters.Current;

            List<car_brand> items = RepoCarBrand.FindAll(param.Skip, param.Take, (param.Sortings != null ? param.Sortings.ToList() : null), param.Filters);
            int total = RepoCarBrand.Count(param.Filters);

            return new JavaScriptSerializer().Serialize(new { total = total, data = new CarBrandPresentationStub().MapList(items) });
        }

		[MvcSiteMapNode(Title = "Create", ParentKey = "IndexCarBrand")]
		[SiteMapTitle("Breadcrumb")]
        public ActionResult Create()
        {
			
            CarBrandFormStub formStub = new CarBrandFormStub();

            return View("Form", formStub);
        }

        [HttpPost]
		[SiteMapTitle("Breadcrumb")]
        public ActionResult Create(CarBrandFormStub model)
        {
            //bool isNameExist = RepoCarBrand.Find().Where(p => p.name == model.Name).Count() > 0;
            
            if (ModelState.IsValid)
            {
                car_brand dbItem = new car_brand();
                dbItem = model.GetDbObject(dbItem);

                    RepoCarBrand.Save(dbItem);


                //message
                string template = HttpContext.GetGlobalResourceObject("MyGlobalMessage", "CreateSuccess").ToString();
                this.SetMessage(model.Name, template);

                return RedirectToAction("Index");
            }
            else
            {
                return View("Form", model);
            }
        }

		[MvcSiteMapNode(Title = "Edit", ParentKey = "IndexCarBrand", Key = "EditCarBrand", PreservedRouteParameters = "id")]
		[SiteMapTitle("Breadcrumb")]
        public ActionResult Edit(System.Guid id)
        {
            car_brand car_brand = RepoCarBrand.FindByPk(id);
            ViewBag.name = car_brand.name;
            CarBrandFormStub formStub = new CarBrandFormStub(car_brand);
            return View("Form", formStub);
        }

        [HttpPost]
		[SiteMapTitle("Breadcrumb")]
        public ActionResult Edit(CarBrandFormStub model)
        {
            //bool isNameExist = RepoKompetitor.Find().Where(p => p.name == model.Name && p.id != model.Id).Count() > 0;

            if (ModelState.IsValid)
            {
                car_brand dbItem = RepoCarBrand.FindByPk(model.Id);
                dbItem = model.GetDbObject(dbItem);

                try
                {
                    RepoCarBrand.Save(dbItem);
                }
                catch (Exception e)
                { 
                    return View("Form", model);
                }

                //message
                string template = HttpContext.GetGlobalResourceObject("MyGlobalMessage", "CreateSuccess").ToString();
                this.SetMessage(model.Name, template);

                return RedirectToAction("Index");
            }
            else
            {
                car_brand car_brand = RepoCarBrand.FindByPk(model.Id);
                ViewBag.name = car_brand.name;
                return View("Form", model);
            }
        }

        //[HttpPost]
        //public JsonResult Delete(int id)
        //{
        //    string template = "";
        //    ResponseModel response = new ResponseModel(true);
        //    car_brand dbItem = RepoCarBrand.FindByPk(id);

        //    RepoCarBrand.Delete(dbItem);

        //    return Json(response);
        //}

	}
}

