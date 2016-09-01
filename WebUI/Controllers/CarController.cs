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
using WebUI.Models.Profile;
using WebUI.Models.CarBrand;
using WebUI.Models.CarModel;
using WebUI.Infrastructure.Concrete;
using WebUI.Models.Car;

namespace WebUI.Controllers
{
    [AuthorizeUser(ModuleName = "Car")]
    public class CarController : MyController
    {
		private ICarRepository RepoCar;
		private ICarModelRepository RepoCarModel;
        private ICarBrandRepository RepoCarBrand;
		private ILogRepository RepoLog;

        public CarController(ICarRepository repoCar, ILogRepository repoLog, ICarModelRepository repoCarModel, ICarBrandRepository repoCarBrand)
			: base(repoLog)
        {
            RepoCar = repoCar;
			RepoCarModel = repoCarModel;
            RepoCarBrand = repoCarBrand;
        }

        [MvcSiteMapNode(Title = "Kendaraan", ParentKey = "Dashboard", Key = "IndexCar")]
        [SiteMapTitle("Breadcrumb")]
        public ActionResult Index()
        {
            var model = new CarFormStub();

            model.FillCarModelOptions(RepoCarModel.FindAll());
            model.FillCarBrandOptions(RepoCarBrand.FindAll());

            ViewBag.ListBrand = new JavaScriptSerializer().Serialize(model.CarBrandOptions);
            ViewBag.ListModel = new JavaScriptSerializer().Serialize(model.CarModelOptions);
            ViewBag.ListTransmission = new JavaScriptSerializer().Serialize(model.CarTransmissionOptions);
            ViewBag.ListModelYear = new JavaScriptSerializer().Serialize(model.YearOptions);

            return View();
        }

        //adanya idCar bakal bikin aneh fungsi kalau dipanggil untuk kendo grid
        public string Binding(Guid? idCar)
        {
            //kamus
            GridRequestParameters param = GridRequestParameters.Current;
            car car;
            Business.Infrastructure.FilterInfo filters;
            Guid? idCarModel;

            //algoritma
            if (param.Filters.Filters == null)
            {
                param.Filters.Filters = new List<Business.Infrastructure.FilterInfo>();
                param.Filters.Logic = "and";
            }

            param.Filters.Filters.Add(new Business.Infrastructure.FilterInfo
            {
                Field = "id_owner",
                Operator = "eq",
                Value = GetOwnerId().ToString()
            });

            List<car> items = RepoCar.FindAll(param.Skip, param.Take, (param.Sortings != null ? param.Sortings.ToList() : null), param.Filters);
            int total = RepoCar.Count(param.Filters);

            if (idCar != null)
            {
                car = RepoCar.FindByPk(idCar.Value);
                if (car != null)
                {
                    filters = param.Filters.Filters.Where(m => m.Field == "id_car_model").FirstOrDefault();

                    if (filters == null) //menambahkan mobil jika car_model sama dengan param
                    {
                        items.Add(car);
                        ++total;
                    }
                    else
                    {
                        idCarModel = Guid.Parse(filters.Value);
                        if (car.id_car_model == idCarModel)
                        {
                            items.Add(car);
                            ++total;
                        }
                    }
                }
            }

            return new JavaScriptSerializer().Serialize(new { total = total, data = new CarPresentationStub().MapList(items) });
        }
        
		[MvcSiteMapNode(Title = "Tambah", ParentKey = "IndexCar", Key = "CreateCar")]
		[SiteMapTitle("Breadcrumb")]
        public ActionResult Create()
        {
			List<Business.Entities.car_model> listCarModel = RepoCarModel.FindAll();
			
            CarFormStub formStub = new CarFormStub();
            formStub.FillCarModelOptions(listCarModel);

            return View("Form", formStub);
        }

        [HttpPost]
        [MvcSiteMapNode(Title = "Tambah", ParentKey = "IndexCar", Key = "CreateCarPost")]
        [SiteMapTitle("Breadcrumb")]
        public ActionResult Create(CarFormStub model)
        {
            //bool isNameExist = RepoCar.Find().Where(p => p.name == model.Name).Count() > 0;

            if (ModelState.IsValid)
            {
                car dbItem = new car();
                Guid idOwner = (User as CustomPrincipal).IdOwner.Value;
                dbItem = model.GetDbObject(dbItem, idOwner);

                try
                {
                    RepoCar.Save(dbItem);
                }
                catch (Exception e)
                {
                    model.FillCarModelOptions(RepoCarModel.FindAll());
                    return View("Form", model);
                }

                //message
                string template = HttpContext.GetGlobalResourceObject("MyGlobalMessage", "CreateSuccess").ToString();
                this.SetMessage(dbItem.license_plate, template);

                return RedirectToAction("Index");
            }
            else
            {
                model.FillCarModelOptions(RepoCarModel.FindAll());
                return View("Form", model);
            }
        }

		[MvcSiteMapNode(Title = "Edit", ParentKey = "IndexCar", Key = "EditCar", PreservedRouteParameters = "id")]
		[SiteMapTitle("Breadcrumb")]
        public ActionResult Edit(Guid id)
        {
            car car = RepoCar.FindByPk(id);;

            CarFormStub formStub = new CarFormStub(car);
            formStub.FillCarModelOptions(RepoCarModel.FindAll());

            ViewBag.name = car.license_plate;
            //formStub.Color = null;

            return View("Form", formStub);
        }

        [HttpPost]
        [SiteMapTitle("Breadcrumb")]
        public ActionResult Edit(CarFormStub model)
        {
            //bool isNameExist = RepoKompetitor.Find().Where(p => p.name == model.Name && p.id != model.Id).Count() > 0;

            if (ModelState.IsValid)
            {
                car dbItem = RepoCar.FindByPk(model.Id);
                Guid idOwner = (User as CustomPrincipal).IdOwner.Value;
                dbItem = model.GetDbObject(dbItem, idOwner);

                try
                {
                    RepoCar.Save(dbItem);
                }
                catch (Exception e)
                {
                    model.FillCarModelOptions(RepoCarModel.FindAll());
                    return View("Form", model);
                }

                //message
                string template = HttpContext.GetGlobalResourceObject("MyGlobalMessage", "EditSuccess").ToString();
                this.SetMessage(dbItem.license_plate, template);

                return RedirectToAction("Index");
            }
            else
            {
                car car = RepoCar.FindByPk(model.Id);
                ViewBag.name = car.license_plate;
                model.FillCarModelOptions(RepoCarModel.FindAll());
                return View("Form", model);
            }
        }

        [MvcSiteMapNode(Title = "Detail", ParentKey = "IndexCar", Key = "DetailCar")]
        public ViewResult Detail(Guid id)
        {
            car dbItem = RepoCar.FindByPk(id);
            CarPresentationStub model = new CarPresentationStub(dbItem);

            return View(model);
        }

        //[HttpPost]
        //public JsonResult Delete(int id)
        //{
        //    string template = "";
        //    ResponseModel response = new ResponseModel(true);
        //    car dbItem = RepoCar.FindByPk(id);

        //    RepoCar.Delete(dbItem);

        //    return Json(response);
        //}

        public string BindingCarBrand()
        {
            List<car_brand> items = RepoCarBrand.FindAll(null, null, null, null);
            int total = items.Count();

            return new JavaScriptSerializer().Serialize(new { total = total, data = new CarBrandPresentationStub().MapList(items).OrderBy(x=>x.Name) });
        }

        public string BindingCarModel()
        {
            GridRequestParameters param = GridRequestParameters.Current;

            List<car_model> items = RepoCarModel.FindAll(null, null, null, (param.Filters != null ? param.Filters : null));
            int total = items.Count();

            return new JavaScriptSerializer().Serialize(new { total = total, data = new CarModelPresentationStub().MapList(items).OrderBy(x=>x.Name) });
        }

        private Guid GetOwnerId()
        {
            //replace 
            Guid id = (User as CustomPrincipal).IdOwner.Value;

            return id;
        }
	}
}

