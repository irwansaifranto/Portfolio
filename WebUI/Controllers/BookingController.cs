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
using WebUI.Models.Owner;
using WebUI.Models.CarPackage;

namespace WebUI.Controllers
{
    [AuthorizeUser(ModuleName = "Booking")]
    public class BookingController : MyController
    {
        private ILogRepository RepoLog;
        private ICustomerRepository RepoCustomer;
        private IDriverRepository RepoDriver;
        private ICarModelRepository RepoCarModel;
        private ICarRepository RepoCar;
        private IRentRepository RepoRent;
        private IOwnerRepository RepoOwner;
        private IInvoiceRepository RepoInvoice;
        private IRentPositionRepository RepoRentPosition;
        private ICityRepository RepoCityPosition;
        private IDummyNotificationRepository RepoDummyNotification;
        private ICarPackageRepository RepoCarPackage;

        public BookingController(ILogRepository repoLog, ICustomerRepository repoCustomer, IDriverRepository repoDriver,
            ICarModelRepository repoCarModel, ICarRepository repoCar, IRentRepository repoRent, IOwnerRepository repoOwner,
            IInvoiceRepository repoInvoice, IRentPositionRepository repoRentPosition, ICityRepository repoCityPosition, IDummyNotificationRepository repoDummyNotification, ICarPackageRepository repoCarPackage)
            : base(repoLog)
        {
            RepoCustomer = repoCustomer;
            RepoDriver = repoDriver;
            RepoCarModel = repoCarModel;
            RepoCar = repoCar;
            RepoRent = repoRent;
            RepoOwner = repoOwner;
            RepoInvoice = repoInvoice;
            RepoRentPosition = repoRentPosition;
            RepoCityPosition = repoCityPosition;
            RepoDummyNotification = repoDummyNotification;
            RepoCarPackage = repoCarPackage;
        }

        [MvcSiteMapNode(Title = "Dashboard", ParentKey = "Dashboard", Key = "IndexBooking")]
        public ViewResult Index()
        {
            //status
            EnumHelper eh = new EnumHelper();
            List<SelectListItem> listStatus = new List<SelectListItem>();

            List<RentStatus> enumList = eh.EnumToList<RentStatus>().ToList();
            foreach (RentStatus single in enumList)
            {
                listStatus.Add(new SelectListItem { Text = eh.GetEnumDescription(single), Value = single.ToString() });
            }

            //print
            Guid? idPrint = null;
            if (TempData["idPrint"] != null)
            {
                idPrint = TempData["idPrint"] as Nullable<Guid>;
            }

            //viewbag
            ViewBag.Statistic = CalculateStatistic();
            ViewBag.ListStatus = new JavaScriptSerializer().Serialize(listStatus);
            ViewBag.IdPrint = idPrint;


            //notification
            //List<d_notification> result = new List<d_notification>();
            //CustomPrincipal user = User as CustomPrincipal;

            //Business.Infrastructure.FilterInfo filters = new Business.Infrastructure.FilterInfo { Filters = new List<Business.Infrastructure.FilterInfo>(), Logic = "and" };
            //filters.Filters.Add(new Business.Infrastructure.FilterInfo { Field = "id_owner", Operator = "eq", Value = user.IdOwner.Value.ToString() });
            //filters.Filters.Add(new Business.Infrastructure.FilterInfo { Field = "is_read", Operator = "eq", Value = false.ToString() });
            //result = RepoDummyNotification.FindAll(null, null, null, filters);

            //ViewBag.TotalNotif = result.Count();
            return View();
        }

        [MvcSiteMapNode(Title = "Detail", ParentKey = "IndexBooking")]
        public ViewResult Detail(Guid id)
        {
            rent dbItem = RepoRent.FindByPk(id);
            List<rent_package> listRentPackage = RepoRent.FindAllRentPackage().Where(n => n.id_rent == id).ToList();
            BookingPresentationStub model = new BookingPresentationStub(dbItem, listRentPackage);

            return View(model);
        }

        #region map

        [MvcSiteMapNode(Title = "Lokasi Mobil", ParentKey = "IndexBooking")]
        public ViewResult Map()
        {
            CustomPrincipal user = User as CustomPrincipal;
            owner owner = RepoOwner.FindByPk(user.IdOwner.Value);

            //viewbag
            ViewBag.Owner = new OwnerPresentationStub(owner);

            return View();
        }

        public string BindingMap()
        {
            // Kamus
            List<RentPositionPresentationStub> result = new List<RentPositionPresentationStub>();
            List<rent_position> rentPositionList = new List<rent_position>();
            List<rent> rentList;
            CustomPrincipal user = User as CustomPrincipal;

            Business.Infrastructure.FilterInfo filters = new Business.Infrastructure.FilterInfo { Filters = new List<Business.Infrastructure.FilterInfo>(), Logic = "and" };
            rent_position rentPosition;

            // algoritma
            //ambil rent yang GO untuk user login
            filters.Filters.Add(new Business.Infrastructure.FilterInfo { Field = "status", Operator = "eq", Value = RentStatus.GO.ToString() });
            filters.Filters.Add(new Business.Infrastructure.FilterInfo { Field = "id_owner", Operator = "eq", Value = user.IdOwner.Value.ToString() });

            rentList = RepoRent.FindAll(null, null, null, filters);

            //ambil rent position terakhir dari setiap rent
            foreach (rent r in rentList)
            {
                rentPosition = r.rent_position.OrderByDescending(m => m.created_time).FirstOrDefault();
                //dimasukin ke rentPosition
                if (rentPosition != null)
                {
                    rentPositionList.Add(rentPosition);
                }
            }

            //mengisi RentPositionPresentationStub list
            result = new RentPositionPresentationStub().Map(rentPositionList);

            return new JavaScriptSerializer().Serialize(result);
        }

        #endregion

        public ViewResult PrintQuotation(Guid id)
        {
            rent dbItem = RepoRent.FindByPk(id);
            List<rent_package> listRentPackage = RepoRent.FindAllRentPackage().Where(n => n.id_rent == id).ToList();
            BookingPresentationStub model = new BookingPresentationStub(dbItem, listRentPackage);

            return View(model);
        }

        #region create

        [MvcSiteMapNode(Title = "Tambah", ParentKey = "IndexBooking")]
        public ActionResult Create()
        {
            BookingFormStub model = new BookingFormStub();

            FillModelOptions(model);

            return View("Form", model);
        }

        [HttpPost]
        public ActionResult Create(BookingFormStub model, bool print = false)
        {
            if (model.Status != RentStatus.CANCEL)
                ModelState.Remove("CancelNotes");
            if (model.IdCarPackage == Guid.Empty)
                ModelState.Remove("IdCarPackage");

            if (model.ListRentPackageText != null)
                model.ListRentPackageItem = new JavaScriptSerializer().Deserialize<List<RentPackageFormStub>>(model.ListRentPackageText);
            ModelState.Remove("PriceDay");
            //ModelState.Remove("Price");

            if (ModelState.IsValid)
            {
                CustomPrincipal user = User as CustomPrincipal;

                //melengkapi data model
                owner owner = RepoOwner.FindByPk(user.IdOwner.Value);
                model.Code = RepoRent.GenerateRentCode(owner);

                customer cust;
                if (model.IdCustomer.HasValue == false)
                {

                    cust = model.CreateNewCustomer(user.IdOwner.Value);
                    RepoCustomer.Save(cust);
                    model.IdCustomer = cust.id;
                }
                else
                {
                    cust = RepoCustomer.FindByPk(model.IdCustomer.Value);
                    model.UpdateCustomer(cust);

                    RepoCustomer.Save(cust);
                }

                //save to db
                rent dbItem = model.GetDbObjectOnCreate(user.Identity.Name, user.IdOwner.Value);
                //RepoRent.Save(dbItem);
                
                Guid idRentPackage = RepoRent.Save(dbItem);

                if (model.ListRentPackageItem != null && model.ListRentPackageItem.Count() > 0)
                {
                    foreach (RentPackageFormStub single in model.ListRentPackageItem)
                    {
                        RepoRent.SaveListPackage(single.GetDbObject(idRentPackage, single.IdCarPackage));
                    }
                }

                //message
                string template = HttpContext.GetGlobalResourceObject("MyGlobalMessage", "CreateSuccess").ToString();
                this.SetMessage("Booking " + model.Name, template);

                //print flag
                if (print)
                    TempData["idPrint"] = dbItem.id;

                return RedirectToAction("Index");
            }
            else
            {
                FillModelOptions(model);

                return View("Form", model);
            }
        }

        #endregion

        #region edit

        [MvcSiteMapNode(Title = "Edit", ParentKey = "IndexBooking")]
        public ActionResult Edit(Guid id)
        {
            rent dbItem = RepoRent.FindByPk(id);
            Guid? idOwner = (User as CustomPrincipal).IdOwner;
            List<car_package> listCarPackage = RepoCarPackage.FindAll().Where(n => n.id_owner == idOwner).ToList();
            if (dbItem == null)
            {
                var wrapper = new HttpContextWrapper(System.Web.HttpContext.Current);
                return this.InvokeHttp404(wrapper);
            }

            BookingFormStub model = new BookingFormStub(dbItem, listCarPackage);
            FillModelOptions(model);

            ViewBag.name = "Booking " + model.Name;

            return View("Form", model);
        }

        [HttpPost]
        public ActionResult Edit(BookingFormStub model, bool print = false)
        {
            if (model.ListRentPackageText != null && model.ListRentPackageText != "")
                model.ListRentPackageItem = new JavaScriptSerializer().Deserialize<List<RentPackageFormStub>>(model.ListRentPackageText);
            List<rent_package> pakets;
            RentPackageFormStub rentPackageFS;

            rent dbItem = RepoRent.FindByPk(model.Id);

            if (model.Status != RentStatus.CANCEL)
                ModelState.Remove("CancelNotes");

            if (ModelState.IsValid)
            {
                CustomPrincipal user = User as CustomPrincipal;

                //process customer
                customer cust;
                if (model.IdCustomer.HasValue == false)
                {
                    cust = model.CreateNewCustomer(user.IdOwner.Value);
                    RepoCustomer.Save(cust);
                    model.IdCustomer = cust.id;
                }
                else
                {
                    cust = RepoCustomer.FindByPk(model.IdCustomer.Value);
                    model.UpdateCustomer(cust);

                    RepoCustomer.Save(cust);
                }

                //save to db
                model.UpdateDbObject(dbItem, user);
                RepoRent.Save(dbItem);

                //save rent_package
                pakets = dbItem.rent_package.ToList();
                foreach (rent_package paket in pakets.ToList())
                {
                    RepoRent.DeleteRentPackage(paket);
                }

                if (model.ListRentPackageItem != null && model.ListRentPackageItem.Count() > 0)
                {
                    foreach (RentPackageFormStub single in model.ListRentPackageItem)
                    {
                        RepoRent.SaveListPackage(single.GetDbObject(model.Id, single.IdCarPackage));
                    }
                }



                //rent_package idCarPackage;
                //foreach (RentPackageFormStub Single in model.ListRentPackageItem)
                //{
                //    idCarPackage = pakets.Where(n => n.id_rent == Single.IdRent).FirstOrDefault();
                //    foreach (rent_package paket in pakets)
                //    {
                //        if (idCarPackage.id_car_package != paket.id_car_package)
                //        {
                //            pakets.Remove(paket); 
                //            //rentPackageFS.SetDbObject(paket);
                //            //RepoRent.SaveListPackage(paket);
                //        }
                //    }
                //}

                //foreach (rent_package paket in pakets)
                //{
                //    //idCarPackage = pakets.Where(n => n.id_car_package == paket.id_car_package).FirstOrDefault();
                //    rentPackageFS = model.ListRentPackageItem.Where(n => n.IdRent == paket.id_rent).FirstOrDefault();
                //    if (rentPackageFS != null)
                //    {
                //        rentPackageFS.SetDbObject(paket);
                //        RepoRent.SaveListPackage(paket);
                //    }
                //}

                //message
                string template = HttpContext.GetGlobalResourceObject("MyGlobalMessage", "EditSuccess").ToString();
                this.SetMessage("Booking " + model.Name, template);

                //print flag
                if (print)
                    TempData["idPrint"] = dbItem.id;

                return RedirectToAction("Index");
            }
            else
            {
                model.Code = dbItem.code;
                FillModelOptions(model);

                ViewBag.name = "Booking " + dbItem.customer.name;

                return View("Form", model);
            }
        }

        #endregion

        #region binding

        /// <summary>
        /// special filter parameter:
        ///     filter_date
        /// </summary>
        /// <returns></returns>
        public string Binding()
        {
            //kamus
            GridRequestParameters param = GridRequestParameters.Current;
            Business.Infrastructure.FilterInfo filters = param.Filters;
            Guid? idOwner = (User as CustomPrincipal).IdOwner;
            List<rent> items;
            List<BookingPresentationStub> result = new List<BookingPresentationStub>();
            int total = 0;

            //algoritma
            if (idOwner.HasValue)
            {
                AddOwnerFilter(filters, idOwner.Value);

                items = RepoRent.FindAll(param.Skip, param.Take, (param.Sortings != null ? param.Sortings.ToList() : null), filters);
                total = RepoRent.Count(param.Filters);
                result = new BookingPresentationStub().MapList(items);
            }

            return new JavaScriptSerializer().Serialize(new { total = total, data = result });
        }

        //public string BindingCustomer()
        //{
        //    //kamus
        //    GridRequestParameters param = GridRequestParameters.Current;
        //    Business.Infrastructure.FilterInfo filters = param.Filters;
        //    Guid? idOwner = (User as CustomPrincipal).IdOwner;
        //    List<customer> items;
        //    List<CustomerPresentationStub> result = new List<CustomerPresentationStub>();
        //    int total = 0;

        //    //algoritma
        //    if (idOwner.HasValue)
        //    {
        //        AddOwnerFilter(filters, idOwner.Value);

        //        items = RepoCustomer.FindAll(null, null, null, filters);
        //        total = items.Count();
        //        result = new CustomerPresentationStub().MapList(items).OrderBy(x => x.Name).ToList();
        //    }

        //    return new JavaScriptSerializer().Serialize(new { total = total, data = result });
        //}

        public string BindingCarPackage()
        {
            //kamus
            GridRequestParameters param = GridRequestParameters.Current;
            Business.Infrastructure.FilterInfo filters = param.Filters;

            Guid? idOwner = (User as CustomPrincipal).IdOwner;
            List<car_package> items;
            List<CarPackagePresentationStub> result = new List<CarPackagePresentationStub>();
            int total = 0;

            //algoritma
            if (idOwner.HasValue)
            {
                AddOwnerFilter(filters, idOwner.Value);

                items = RepoCarPackage.FindAll(null, null, null, filters);
                total = items.Count();
                result = new CarPackagePresentationStub().MapList(items).ToList();
            }

            return new JavaScriptSerializer().Serialize(new { total = total, data = result });
        }

        public string BindingDriver()
        {
            //kamus
            GridRequestParameters param = GridRequestParameters.Current;
            Business.Infrastructure.FilterInfo filters = param.Filters;
            Guid? idOwner = (User as CustomPrincipal).IdOwner;
            List<driver> items;
            List<DriverPresentationStub> result = new List<DriverPresentationStub>();
            int total = 0;

            //algoritma
            if (idOwner.HasValue)
            {
                AddOwnerFilter(filters, idOwner.Value);

                items = RepoDriver.FindAll(null, null, null, filters);
                total = items.Count();
                result = new DriverPresentationStub().MapList(items).OrderBy(x => x.Name).ToList();
            }

            return new JavaScriptSerializer().Serialize(new { total = total, data = result });
        }

        public string BindingCarModel()
        {
            //kamus
            GridRequestParameters param = GridRequestParameters.Current;
            Business.Infrastructure.FilterInfo filters = param.Filters;
            Business.Infrastructure.FilterInfo carFilters;
            Guid? idOwner = (User as CustomPrincipal).IdOwner;
            List<car_model> carModels;
            List<CarModelPresentationStub> result = new List<CarModelPresentationStub>();
            int total = 0;
            List<car> cars;

            //algoritma
            carFilters = new Business.Infrastructure.FilterInfo { Field = "id_owner", Operator = "eq", Value = idOwner.ToString() };
            //filters.Filters = new List<Business.Infrastructure.FilterInfo>();
            //filters.Filters.Add(new Business.Infrastructure.FilterInfo { Field = "id_owner", Operator = "eq", Value = idOwner.Value.ToString() });
            //filters.Filters.Add(new Business.Infrastructure.FilterInfo { Field = "is_active", Operator = "eq", Value = "true" });
            cars = RepoCar.FindAll(null, null, null, carFilters).ToList();

            carModels = cars.Select(m => m.car_model).Distinct().ToList();
            total = carModels.Count();

            result = new CarModelPresentationStub().MapList(carModels).OrderBy(x => x.Name).ToList();

            return new JavaScriptSerializer().Serialize(new { total = total, data = result });
        }

        //obsolete
        //public string BindingCar(Guid idRent)
        //{
        //    //kamus
        //    GridRequestParameters param = GridRequestParameters.Current;
        //    Business.Infrastructure.FilterInfo filters = param.Filters;

        //    Guid? idOwner = (User as CustomPrincipal).IdOwner;
        //    Boolean active = true;
        //    List<car> items;
        //    List<CarPresentationStub> result = new List<CarPresentationStub>();
        //    int total = 0;
        //    rent editedRent;
        //    car editedCar;


        //    //algoritma
        //    if (idOwner.HasValue)
        //    {
        //        AddOwnerFilter(filters, idOwner.Value);
        //        AddActiveFilter(filters, active);
        //        items = RepoCar.FindAll(null, null, null, filters);

        //        if (idRent != Guid.Empty)
        //        {
        //            foreach (car singleCar in items)
        //            {
        //                editedCar = new car();
        //                editedCar = RepoCar.FindByPk((Guid)singleCar.id);
        //                if (editedCar.is_active == false)
        //                {
        //                    items.Add(editedCar);
        //                }
        //            }
        //        }

        //        total = items.Count();
        //        result = new CarPresentationStub().MapList(items).OrderBy(x => x.LicensePlate).ToList();
        //    }

        //    return new JavaScriptSerializer().Serialize(new { total = total, data = result });
        //}

        #endregion

        #region private

        private void AddOwnerFilter(Business.Infrastructure.FilterInfo filters, Guid idOwner)
        {
            if (filters == null)
                filters = new Business.Infrastructure.FilterInfo { Filters = new List<Business.Infrastructure.FilterInfo>(), Logic = "and" };

            if (filters.Filters == null)
                filters.Filters = new List<Business.Infrastructure.FilterInfo>();

            if (idOwner != null)
            {
                filters.Filters.Add(new Business.Infrastructure.FilterInfo { Field = "id_owner", Operator = "eq", Value = idOwner.ToString() });
            }
        }

        private void AddActiveFilter(Business.Infrastructure.FilterInfo filters, Boolean active)
        {
            if (filters == null)
                filters = new Business.Infrastructure.FilterInfo { Filters = new List<Business.Infrastructure.FilterInfo>(), Logic = "and" };

            if (filters.Filters == null)
                filters.Filters = new List<Business.Infrastructure.FilterInfo>();

            filters.Filters.Add(new Business.Infrastructure.FilterInfo { Field = "is_active", Operator = "eq", Value = active.ToString() });
        }

        private BookingStatisticModel CalculateStatistic()
        {
            //kamus
            DisplayFormatHelper dfh = new DisplayFormatHelper();
            int statistic;
            Business.Infrastructure.FilterInfo filters, singleFilter;
            BookingStatisticModel model = new BookingStatisticModel();

            //algoritma
            CustomPrincipal user = User as CustomPrincipal;

            if (user.IdOwner != null)
            {
                //berangkat besok
                filters = new Business.Infrastructure.FilterInfo
                {
                    Filters = new List<Business.Infrastructure.FilterInfo>
                    {
                        new Business.Infrastructure.FilterInfo { Field = "start_rent", Operator = "eq", Value = DateTime.Now.AddDays(1).ToString(dfh.SqlDateFormat) },
                        new Business.Infrastructure.FilterInfo { Field = "status", Operator = "neq", Value = RentStatus.CANCEL.ToString() }
                    },
                    Logic = "and"
                };
                AddOwnerFilter(filters, user.IdOwner.Value);

                statistic = RepoRent.Count(filters);
                model.TomorrowDeparture = statistic;

                //belum ditugaskan
                filters.Filters.Clear();
                filters.Filters.Add(new Business.Infrastructure.FilterInfo { Field = "status", Operator = "neq", Value = RentStatus.CANCEL.ToString() });
                AddOwnerFilter(filters, user.IdOwner.Value);

                statistic = RepoRent.CountUnassignedCar(filters);
                model.UnassignedCar = statistic;

                statistic = RepoRent.CountUnassignedDriver(filters);
                model.UnassignedDriver = statistic;

                //unpaid
                filters.Filters.Clear();
                singleFilter = new Business.Infrastructure.FilterInfo { Field = "rent.id_owner", Operator = "eq", Value = user.IdOwner.ToString() };
                filters.Filters.Add(singleFilter);
                singleFilter = new Business.Infrastructure.FilterInfo { Field = "status", Operator = "eq", Value = InvoiceStatus.UNPAID.ToString() };
                filters.Filters.Add(singleFilter);

                statistic = RepoInvoice.Count(filters);
                model.UnpaidInvoice = statistic;

                //cancel
                filters.Filters.Clear();
                singleFilter = new Business.Infrastructure.FilterInfo { Field = "status", Operator = "eq", Value = RentStatus.CANCEL.ToString() };
                filters.Filters.Add(singleFilter);
                AddOwnerFilter(filters, user.IdOwner.Value);

                statistic = RepoRent.Count(filters);
                model.CancelledRent = statistic;
            }

            return model;
        }

        private void FillModelOptions(BookingFormStub model)
        {
            Guid idOwner = (User as CustomPrincipal).IdOwner.Value;
            List<car_package> result = new List<car_package>();

            Business.Infrastructure.FilterInfo filters = new Business.Infrastructure.FilterInfo { Filters = new List<Business.Infrastructure.FilterInfo>(), Logic = "and" };
            filters.Filters.Add(new Business.Infrastructure.FilterInfo { Field = "id_owner", Operator = "eq", Value = idOwner.ToString() });
            filters.Filters.Add(new Business.Infrastructure.FilterInfo { Field = "is_active", Operator = "eq", Value = true.ToString() });

            result = RepoCarPackage.FindAll(null, null, null, filters);

            model.SetCarPackageOptions(result);

        }

        #endregion
    }
}

