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
using WebUI.Models.CarModel;
using WebUI.Models.CarBrand;

namespace WebUI.Controllers
{
    [AuthorizeUser(ModuleName = "Car Model")]
    public class CarModelController : MyController
    {
		private ICarModelRepository RepoCarModel;
		private ICarBrandRepository RepoCarBrand;
		private ILogRepository RepoLog;

        public CarModelController(ICarModelRepository repoCarModel, ILogRepository repoLog, ICarBrandRepository repoCarBrand)
			: base(repoLog)
        {
            RepoCarModel = repoCarModel;
			RepoCarBrand = repoCarBrand;
        }

		[MvcSiteMapNode(Title = "Car Model", ParentKey = "Dashboard",Key="IndexCarModel")]
		[SiteMapTitle("Breadcrumb")]
        public ActionResult Index()
        {
            var model = new CarModelFormStub();
            model.FillCarBrandOptions(RepoCarBrand.FindAll());            
            ViewBag.ListBrand = new JavaScriptSerializer().Serialize(model.CarBrandOptions);

            return View();
        }

        public string Binding()
        {
            //kamus
            GridRequestParameters param = GridRequestParameters.Current;
            Business.Infrastructure.FilterInfo filters = param.Filters;
            List<car_model> items = RepoCarModel.FindAll(param.Skip, param.Take, (param.Sortings != null ? param.Sortings.ToList() : null), param.Filters);
            int total = RepoCarModel.Count(param.Filters);

            return new JavaScriptSerializer().Serialize(new { total = total, data = new CarModelPresentationStub().MapList(items) });
        }

		[MvcSiteMapNode(Title = "Create", ParentKey = "IndexCarModel")]
		[SiteMapTitle("Breadcrumb")]
        public ActionResult Create()
        {
			List<Business.Entities.car_brand> listCarBrand = RepoCarBrand.FindAll();
			
            CarModelFormStub formStub = new CarModelFormStub(listCarBrand);

            return View("Form", formStub);
        }

        [HttpPost]
		[SiteMapTitle("Breadcrumb")]
        public ActionResult Create(CarModelFormStub model)
        {
            //bool isNameExist = RepoCarModel.Find().Where(p => p.name == model.Name).Count() > 0;
            
            if (ModelState.IsValid)
            {
                car_model dbItem = new car_model();
                dbItem = model.GetDbObject(dbItem);

                try
                {
                    RepoCarModel.Save(dbItem);
                }
                catch (Exception e)
                {
				model.FillCarBrandOptions(RepoCarBrand.FindAll());
                    return View("Form", model);
                }

                //message
                string template = HttpContext.GetGlobalResourceObject("MyGlobalMessage", "CreateSuccess").ToString();
                this.SetMessage(model.Name, template);

                return RedirectToAction("Index");
            }
            else
            {
				model.FillCarBrandOptions(RepoCarBrand.FindAll());
                return View("Form", model);
            }
        }

		[MvcSiteMapNode(Title = "Edit", ParentKey = "IndexCarModel", Key = "EditCarModel", PreservedRouteParameters = "id")]
		[SiteMapTitle("Breadcrumb")]
        public ActionResult Edit(Guid id)
        {
            car_model car_model = RepoCarModel.FindByPk(id);
			List<Business.Entities.car_brand> listCarBrand = RepoCarBrand.FindAll();
            CarModelFormStub formStub = new CarModelFormStub(car_model,listCarBrand);

            ViewBag.name = car_model.name;
            return View("Form", formStub);
        }

        [HttpPost]
		[SiteMapTitle("Breadcrumb")]
        public ActionResult Edit(CarModelFormStub model)
        {
            //bool isNameExist = RepoKompetitor.Find().Where(p => p.name == model.Name && p.id != model.Id).Count() > 0;

            if (ModelState.IsValid)
            {
                car_model dbItem = RepoCarModel.FindByPk(model.Id);
                dbItem = model.GetDbObject(dbItem);

                try
                {
                    RepoCarModel.Save(dbItem);
                }
                catch (Exception e)
                { 
				model.FillCarBrandOptions(RepoCarBrand.FindAll());
                    return View("Form", model);
                }

                //message
                string template = HttpContext.GetGlobalResourceObject("MyGlobalMessage", "CreateSuccess").ToString();
                this.SetMessage(model.Name, template);

                return RedirectToAction("Index");
            }
            else
            {
                car_model car_model = RepoCarModel.FindByPk(model.Id);
                ViewBag.name = car_model.name;
				model.FillCarBrandOptions(RepoCarBrand.FindAll());
                return View("Form", model);
            }
        }

        public string BindingBrand()
        {
            List<car_brand> items = RepoCarBrand.FindAll(null, null, null, null);
            int total = items.Count();

            return new JavaScriptSerializer().Serialize(new { total = total, data = new CarBrandPresentationStub().MapList(items).OrderBy(x=>x.Name) });
        }

        //[HttpPost]
        //public JsonResult Delete(int id)
        //{
        //    string template = "";
        //    ResponseModel response = new ResponseModel(true);
        //    car_model dbItem = RepoCarModel.FindByPk(id);

        //    RepoCarModel.Delete(dbItem);

        //    return Json(response);
        //}

	}
}

