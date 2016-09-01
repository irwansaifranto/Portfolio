using Business.Abstract;
using Business.Entities;
using MvcSiteMapProvider;
using MvcSiteMapProvider.Web.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebUI.Extension;
using System.Web.Mvc;
using WebUI.Infrastructure;
using WebUI.Infrastructure.Concrete;
using WebUI.Models.CarExpense;
using System.Web.Script.Serialization;
using Common.Enums;
using WebUI.Models.Car;

namespace WebUI.Controllers
{
    [AuthorizeUser(ModuleName = "Car Expense")]
    public class CarExpenseController : MyController
    {
        private ILogRepository RepoLog;
        private ICarExpenseRepository RepoCarExpense;
        private ICarRepository RepoCar;
        //
        // GET: /CarExpense/
        public CarExpenseController(ILogRepository repoLog, ICarExpenseRepository repoCarExpense, ICarRepository repoCar)
            : base(repoLog)
        {
            RepoCarExpense = repoCarExpense;
            RepoCar = repoCar;
        }

        [MvcSiteMapNode(Title = "Biaya Kendaraan", ParentKey = "Dashboard", Key = "IndexCarExpense")]
        [SiteMapTitle("Breadcrumb")]
        public ActionResult Index()
        {
            CarExpenseFormStub model = new CarExpenseFormStub();
            ViewBag.ListExpenseType = new JavaScriptSerializer().Serialize(model.CarExpenseTypeOptions);

            //var modelCar = new CarFormStub();

            //ViewBag.ListBrand = new JavaScriptSerializer().Serialize(modelCar.CarBrandOptions);
            return View();
        }

        public string Binding()
        {
            //kamus
            GridRequestParameters param = GridRequestParameters.Current;
            Business.Infrastructure.FilterInfo filters = param.Filters;
            Guid? idOwner = (User as CustomPrincipal).IdOwner;
            List<car_expense> items;
            List<CarExpensePresentationStub> result = new List<CarExpensePresentationStub>();
            int total = 0;
            //algoritma
            if (idOwner.HasValue)
            {
                AddOwnerFilter(filters, idOwner.Value);

                items = RepoCarExpense.FindAll(param.Skip, param.Take, (param.Sortings != null ? param.Sortings.ToList() : null), filters);
                total = RepoCarExpense.Count(param.Filters);
                result = new CarExpensePresentationStub().MapList(items);
            }

            return new JavaScriptSerializer().Serialize(new { total = total, data = result });
        }

        //public string Binding()
        //{
        //    //kamus
        //    GridRequestParameters param = GridRequestParameters.Current;
        //    int total = 0;
        //    //penanganan filter null
        //    if (param.Filters.Filters == null)
        //    {
        //        param.Filters.Filters = new List<Business.Infrastructure.FilterInfo>();
        //        param.Filters.Logic = "and";
        //    }

        //    List<car_expense> items = RepoCarExpense.FindAll(param.Skip, param.Take, (param.Sortings != null ? param.Sortings.ToList() : null), param.Filters);
        //    total = RepoCarExpense.Count(param.Filters);

        //    return new JavaScriptSerializer().Serialize(new { total = total, data = new CarExpensePresentationStub().MapList(items) });
        //}

        [MvcSiteMapNode(Title = "Tambah", ParentKey = "IndexCarExpense", Key = "CreateCarExpense")]
        [SiteMapTitle("Breadcrumb")]
        public ActionResult Create() //get
        {
            CarExpenseFormStub formStub = new CarExpenseFormStub();
            formStub.ExpenseDate = DateTime.Now;
            FillModelOptions(formStub);

            return View("Form", formStub);
        }

        [HttpPost]
        public ActionResult Create(CarExpenseFormStub model, bool print = false)
        {
            //bool isNameExist = RepoCar.Find().Where(p => p.name == model.Name).Count() > 0;
            if (model.ExpenseType != CarExpenseType.MAINTENANCE.ToString())
            {
                ModelState.Remove("Distance");
            }

            if (ModelState.IsValid)
            {
                car_expense dbItem = model.GetDbObject((User as CustomPrincipal).Identity.Name);

                try
                {
                    RepoCarExpense.Save(dbItem);
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("Id", e.Message);

                    FillModelOptions(model);

                    return View("Form", model);
                }

                //message
                DisplayFormatHelper dfh = new DisplayFormatHelper();
                string template = HttpContext.GetGlobalResourceObject("MyGlobalMessage", "CreateSuccess").ToString();
                this.SetMessage(model.LicensePlate, template);

                return RedirectToAction("Index");
            }
            else
            {
                FillModelOptions(model);

                return View("Form", model);
            }
        }

        [MvcSiteMapNode(Title = "Edit", ParentKey = "IndexCarExpense", Key = "EditCarExpense", PreservedRouteParameters = "id")]
        [SiteMapTitle("Breadcrumb")]
        public ActionResult Edit(Guid id)
        {
            car_expense carExpense = RepoCarExpense.FindByPk(id);
            CarExpenseFormStub formStub = new CarExpenseFormStub(carExpense);
            FillModelOptions(formStub);

            ViewBag.Name = carExpense.car.license_plate;

            return View("Form", formStub);
        }

        [HttpPost]
        [SiteMapTitle("Breadcrumb")]
        public ActionResult Edit(CarExpenseFormStub model)
        {
            //bool isNameExist = RepoKompetitor.Find().Where(p => p.name == model.Name && p.id != model.Id).Count() > 0;
            if (model.ExpenseType != CarExpenseType.MAINTENANCE.ToString())
            {
                ModelState.Remove("Distance");
            }

            if (ModelState.IsValid)
            {
                car_expense dbItem = RepoCarExpense.FindByPk(model.Id);
                dbItem = model.UpdateObject(dbItem, (User as CustomPrincipal).Identity.Name);

                try
                {
                    RepoCarExpense.Save(dbItem);
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("ExpenseDate", e.Message);

                    car_expense carExpense = RepoCarExpense.FindByPk(model.Id);
                    FillModelOptions(model);

                    ViewBag.Name = carExpense.car.license_plate;

                    return View("Form", model);
                }

                //message
                string template = HttpContext.GetGlobalResourceObject("MyGlobalMessage", "CreateSuccess").ToString();
                this.SetMessage(model.LicensePlate, template);

                return RedirectToAction("Index");
            }
            else
            {
                car_expense carExpense = RepoCarExpense.FindByPk(model.Id);
                FillModelOptions(model);

                ViewBag.Name = carExpense.car.license_plate;

                return View("Form", model);
            }
        }

        [MvcSiteMapNode(Title = "Detail", ParentKey = "IndexCarExpense", Key = "DetailCarExpense")]
        public ViewResult Detail(Guid id)
        {
            car_expense dbItem = RepoCarExpense.FindByPk(id);
            CarExpensePresentationStub model = new CarExpensePresentationStub(dbItem);

            return View(model);
        }

        #region private

        private void FillModelOptions(CarExpenseFormStub model)
        {
            Guid? idOwner = (User as CustomPrincipal).IdOwner;
            Business.Infrastructure.FilterInfo filters = new Business.Infrastructure.FilterInfo { Field = "id_owner", Operator = "eq", Value = idOwner.ToString() };
            List<car> cars = RepoCar.FindAll(null, null, null, filters);

            model.FillCarOptions(cars);
        }

        private void AddOwnerFilter(Business.Infrastructure.FilterInfo filters, Guid idOwner)
        {
            if (filters == null)
                filters = new Business.Infrastructure.FilterInfo { Filters = new List<Business.Infrastructure.FilterInfo>(), Logic = "and" };

            if (filters.Filters == null)
                filters.Filters = new List<Business.Infrastructure.FilterInfo>();

            if (idOwner != null)
            {
                filters.Filters.Add(new Business.Infrastructure.FilterInfo { Field = "car.id_owner", Operator = "eq", Value = idOwner.ToString() });
            }
        }

        #endregion

    }

}
