 using Business.Abstract;
using Business.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUI.Infrastructure.Concrete;
using WebUI.Models.CarPackage;
using WebUI.Extension;
using System.Web.Script.Serialization;
using WebUI.Infrastructure;
using WebUI.Models.CarModel;
using WebUI.Models.CarBrand;
using WebUI.Models.Car;
using MvcSiteMapProvider;
using MvcSiteMapProvider.Web.Mvc.Filters;
using WebUI.Models;


namespace WebUI.Controllers
{
    [AuthorizeUser(ModuleName = "Car Package")]
    public class CarPackageController : MyController
    {
        //
        // GET: /CarPackage/
        private ILogRepository RepoLog;
        private ICarPackageRepository RepoCarPackage;
        private ICarModelRepository RepoCarModel;
        private ICarBrandRepository RepoCarBrand;
        private ICarRepository RepoCar;

        public CarPackageController(ILogRepository repoLog, ICarPackageRepository repoCarPackage, ICarModelRepository repoCarModel, ICarBrandRepository repoCarBrand, ICarRepository repoCar)
			: base(repoLog)
        {
            RepoCarPackage = repoCarPackage;
            RepoCarModel = repoCarModel;
            RepoCarBrand = repoCarBrand;
            RepoCar = repoCar;
        }

        [MvcSiteMapNode(Title = "Paket Kendaraan", ParentKey = "Dashboard", Key = "IndexCarPackage")]
        [SiteMapTitle("Breadcrumb")]
        public ActionResult Index()
        {
            return View();
        }

        public string Binding()
        {
            //kamus
            GridRequestParameters param = GridRequestParameters.Current;
            int total = 0;

            //add owner filter
            if (param.Filters.Filters == null)
            {
                param.Filters.Filters = new List<Business.Infrastructure.FilterInfo>();
                param.Filters.Logic = "and";
            }
            param.Filters.Filters.Add(new Business.Infrastructure.FilterInfo
            {
                Field = "id_owner",
                Operator = "eq",
                Value = (User as CustomPrincipal).IdOwner.Value.ToString()
            });
            List<car_package> items = RepoCarPackage.FindAll(param.Skip, param.Take, (param.Sortings != null ? param.Sortings.ToList() : null), param.Filters);
            total = RepoCarPackage.Count(param.Filters);

            return new JavaScriptSerializer().Serialize(new { total = total, data = new CarPackagePresentationStub().MapList(items) });

        }

        [MvcSiteMapNode(Title = "Tambah", ParentKey = "IndexCarPackage", Key = "CreateCarPackage")]
        [SiteMapTitle("Breadcrumb")]
        public ActionResult Create() //get
        {
            CarPackageFormStub model = new CarPackageFormStub();

            FillModelOptions(model);

            return View("Form", model);
        }

        [HttpPost]
        public ActionResult Create(CarPackageFormStub model)
        {
            if (ModelState.IsValid)
            {
                CustomPrincipal user = User as CustomPrincipal;
                car_package dbItem = model.GetDbObject(user.IdOwner.Value, user.Identity.Name);

                try
                {
                    RepoCarPackage.Save(dbItem);
                }
                catch (Exception e)
                {
                    FillModelOptions(model);
                    return View("Form", model);
                }

                //message
                string template = HttpContext.GetGlobalResourceObject("MyGlobalMessage", "CreateSuccess").ToString();
                this.SetMessage(model.Name, template);

                return RedirectToAction("Index");
            }
            else
            {
                FillModelOptions(model);
                return View("Form", model);
            }
        }

        [MvcSiteMapNode(Title = "Edit", ParentKey = "IndexCarPackage", Key = "EditCarPackage")]
        [SiteMapTitle("Breadcrumb")]
        public ActionResult Edit(Guid id)
        {
            car_package carPackage = RepoCarPackage.FindByPk(id);

            CarPackageFormStub model = new CarPackageFormStub(carPackage);
            FillModelOptions(model);

            ViewBag.Name = carPackage.name;

            return View("Form", model);
        }

        [HttpPost]
        [SiteMapTitle("Breadcrumb")]
        public ActionResult Edit(CarPackageFormStub model)
        {
            if (ModelState.IsValid)
            {
                car_package dbItem = RepoCarPackage.FindByPk(model.Id);
                CustomPrincipal user = User as CustomPrincipal;
                dbItem = model.UpdateDbObject(dbItem, user);

                try
                {
                    RepoCarPackage.Save(dbItem);
                }
                catch (Exception e)
                {
                    FillModelOptions(model);
                    return View("Form", model);
                }

                //message
                string template = HttpContext.GetGlobalResourceObject("MyGlobalMessage", "CreateSuccess").ToString();
                this.SetMessage(model.Name, template);

                return RedirectToAction("Index");
            }
            else
            {
                FillModelOptions(model);

                car_package carPackage = RepoCarPackage.FindByPk(model.Id);
                ViewBag.name = carPackage.name;

                return View("Form", model);
            }
        }

        //[HttpPost]
        //public JsonResult Delete(Guid id)
        //{
        //    string template = "";
        //    ResponseModel response = new ResponseModel(true);
        //    car_package dbItem = RepoCarPackage.FindByPk(id);

        //    RepoCarPackage.Delete(dbItem);

        //    return Json(response);
        //}

        //public string BindingCarBrand()
        //{
        //    Guid idOwner = (User as CustomPrincipal).IdOwner.Value;
        //    List<car_brand> items = new List<car_brand>();
        //    items = RepoCar.FindAll().Where(x => x.id_owner == idOwner).Select(x => x.car_model.car_brand).Distinct().ToList();
        //    int total = items.Count();

        //    return new JavaScriptSerializer().Serialize(new { total = total, data = new CarBrandPresentationStub().MapList(items).OrderBy(x => x.Name) });
        //}

        //public string BindingCarModel(Guid? id)
        //{
        //    GridRequestParameters param = GridRequestParameters.Current;
        //    Guid idOwner = (User as CustomPrincipal).IdOwner.Value;
        //    //Business.Infrastructure.FilterInfo filters = new Business.Infrastructure.FilterInfo { Filters = new List<Business.Infrastructure.FilterInfo>(), Logic = "and" };
        //    //filters.Filters.Add(new Business.Infrastructure.FilterInfo { Field = "id_owner", Operator = "eq", Value = idOwner.ToString() });
        //    //List<car> cars = RepoCar.FindAll(null, null, null, filters);
        //    //List<car> cars = RepoCar.FindAll().Where(x => x.id_owner == idOwner).ToList();

        //    ///var n dari car_model sedangkan x dari cars
        //    //List<car_model> items = RepoCarModel.FindAll().Where(n=>cars.Any(x=>x.id_car_model == n.id) && n.id_car_brand == id).ToList();
        //    List<car_model> items = RepoCar.FindAll().Where(n => n.car_model.car_brand.id == id && n.id_owner == idOwner).Select(x => x.car_model).Distinct().ToList();                               
        //    int total = items.Count();

        //    return new JavaScriptSerializer().Serialize(new { total = total, data = new CarModelPresentationStub().MapList(items).OrderBy(x => x.Name) });
        //}

        #region private

        private void FillModelOptions(CarPackageFormStub model)
        {
            Guid? idOwner = (User as CustomPrincipal).IdOwner;
            Business.Infrastructure.FilterInfo filters = new Business.Infrastructure.FilterInfo { Field = "id_owner", Operator = "eq", Value = idOwner.ToString() };
            List<car> cars = RepoCar.FindAll(null, null, null, filters);

            List<car_model> models = cars.Select(m => m.car_model).Distinct().ToList();
            List<car_brand> brands = models.Select(m => m.car_brand).Distinct().ToList();

            model.SetCarBrandOptions(brands);
            model.SetCarModelOptions(models);
        }

        #endregion

    }
}
