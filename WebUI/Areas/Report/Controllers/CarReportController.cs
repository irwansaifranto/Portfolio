using Business.Abstract;
using Business.Entities;
using MvcSiteMapProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUI.Areas.Report.Models;
using WebUI.Controllers;
using WebUI.Infrastructure;
using WebUI.Infrastructure.Concrete;
using Common.Enums;

namespace WebUI.Areas.Report.Controllers
{
    [AuthorizeUser(ModuleName = "Car Report")]
    public class CarReportController : MyController
    {
        public IExpenseRepository RepoExpense;
        public IRentRepository RepoRent;
        public ICarRepository RepoCar;
        public ICarExpenseRepository RepoCarExpense;
        public IInvoiceRepository RepoInvoice;

        public CarReportController(ILogRepository repoLog, IExpenseRepository repoExpense, IRentRepository repoRent, ICarRepository repoCar, ICarExpenseRepository repoCarExpense, IInvoiceRepository repoInvoice)
            : base(repoLog)
        {
            RepoExpense = repoExpense;
            RepoRent = repoRent;
            RepoCar = repoCar;
            RepoCarExpense = repoCarExpense;
            RepoInvoice = repoInvoice;
        }

        [MvcSiteMapNode(Title = "Laporan Kendaraan", ParentKey = "Dashboard", Key = "IndexCarReport")]
        public ActionResult Index()
        {
            DailyFilterModel filter = new DailyFilterModel();
            return View(filter);
        }

        [MvcSiteMapNode(Title = "Detail", ParentKey = "IndexCarReport", Key = "DetailCarReport")]
        public ActionResult Detail(Guid id, DateTime startDate, DateTime endDate)
        {
            FilterModelDetail filter = new FilterModelDetail(startDate, endDate, id);
            ViewBag.licensePlate = RepoCar.FindByPk(id).license_plate;
            return View(filter);
        }

        //public JsonResult Binding(DailyFilterModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        Guid idOwner = (User as CustomPrincipal).IdOwner.Value;
        //        List<VehicleExpenseReport> result = RepoExpense.GetVehicleReport(idOwner, model.StartDate, model.EndDate);
        //        List<rent> rents = RepoRent.FindAll();
        //        return Json(new CarReportPresentationStub().MapList(result, rents), JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json(false, JsonRequestBehavior.AllowGet);
        //    }
        //}

        //private void AddExpenseFilter(Business.Infrastructure.FilterInfo filters, Guid idRents)
        //{
        //    if (filters == null)
        //        filters = new Business.Infrastructure.FilterInfo { Filters = new List<Business.Infrastructure.FilterInfo>(), Logic = "and" };

        //    if (filters.Filters == null)
        //        filters.Filters = new List<Business.Infrastructure.FilterInfo>();
        //    else
        //    {
        //            filters.Filters.Add(new Business.Infrastructure.FilterInfo { Field = "id_rent", Operator = "eq", Value = idRents });
        //    }
        //}


        public JsonResult Binding(DailyFilterModel model)
        {
            if (ModelState.IsValid)
            {
                //kasmus
                Guid idOwner = (User as CustomPrincipal).IdOwner.Value;

                Business.Infrastructure.FilterInfo filters;

                List<rent> rents;
                List<expense_item> expenseItem;
                List<car> cars;
                List<car_expense> carExpenses;
                DateTime dtStart, dtEnd;

                //algoritma
                dtStart = new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day, 0, 0, 0);
                dtEnd = new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day, 23, 59, 59);

                //mengambil car yang statusnya active
                filters = new Business.Infrastructure.FilterInfo
                {
                    Filters = new List<Business.Infrastructure.FilterInfo>
                    {
                        new Business.Infrastructure.FilterInfo { Field = "is_active", Operator = "eq", Value = true.ToString() },
                        new Business.Infrastructure.FilterInfo { Field = "id_owner", Operator = "eq", Value = idOwner.ToString() }
                    }
                };
                cars = RepoCar.FindAll(null, null, null, filters);

                //mengambil data booking
                rents = RepoRent.FindAll(idOwner, dtStart, dtEnd);

                //mengambil expense_item (pemasukan kendaraan)
                filters = new Business.Infrastructure.FilterInfo
                {
                    Filters = new List<Business.Infrastructure.FilterInfo>
                    {
                        new Business.Infrastructure.FilterInfo { Field = "expense.date", Operator = "gte", Value = dtStart.ToString() },
                        new Business.Infrastructure.FilterInfo { Field = "expense.date", Operator = "lte", Value = dtEnd.ToString() },
                        new Business.Infrastructure.FilterInfo { Field = "category", Operator = "eq", Value = ExpenseItemCategory.VEHICLE.ToString() },
                        new Business.Infrastructure.FilterInfo { Field = "expense.rent.id_owner", Operator = "eq", Value = idOwner.ToString() }
                    }
                };
                expenseItem = RepoExpense.FindAllItem(null, null, null, filters);

                //mengambil carExpense (biaya kendaraan)
                filters = new Business.Infrastructure.FilterInfo
                {
                    Filters = new List<Business.Infrastructure.FilterInfo>
                    {
                        new Business.Infrastructure.FilterInfo { Field = "expense_date", Operator = "gte", Value = dtStart.ToString() },
                        new Business.Infrastructure.FilterInfo { Field = "expense_date", Operator = "lte", Value = dtEnd.ToString() },
                        new Business.Infrastructure.FilterInfo { Field = "car.id_owner", Operator = "eq", Value = idOwner.ToString() }
                    }
                };
                carExpenses = RepoCarExpense.FindAll(null, null, null, filters);

                return Json(new CarReportPresentationStub().MapList(cars, rents, expenseItem, carExpenses), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult BindingDetail(FilterModelDetail model)
        {
            if (ModelState.IsValid)
            {
                //kasmus
                Business.Infrastructure.FilterInfo filters;
                List<expense_item> expenseItems;
                List<car_expense> carExpenses;
                DateTime dtStart, dtEnd;

                //algoritma
                dtStart = new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day, 0, 0, 0);
                dtEnd = new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day, 23, 59, 59);

                //mengambil expense_item (pemasukan kendaraan)
                filters = new Business.Infrastructure.FilterInfo
                {
                    Filters = new List<Business.Infrastructure.FilterInfo>
                    {
                        new Business.Infrastructure.FilterInfo { Field = "expense.date", Operator = "gte", Value = dtStart.ToString() },
                        new Business.Infrastructure.FilterInfo { Field = "expense.date", Operator = "lte", Value = dtEnd.ToString() },
                        new Business.Infrastructure.FilterInfo { Field = "category", Operator = "eq", Value = ExpenseItemCategory.VEHICLE.ToString() },
                        new Business.Infrastructure.FilterInfo { Field = "expense.rent.id_car", Operator = "eq", Value = model.IdCar.ToString() }
                    }
                };
                expenseItems = RepoExpense.FindAllItem(null, null, null, filters);

                //mengambil carExpense (biaya kendaraan)
                filters = new Business.Infrastructure.FilterInfo
                {
                    Filters = new List<Business.Infrastructure.FilterInfo>
                    {
                        new Business.Infrastructure.FilterInfo { Field = "expense_date", Operator = "gte", Value = dtStart.ToString() },
                        new Business.Infrastructure.FilterInfo { Field = "expense_date", Operator = "lte", Value = dtEnd.ToString() },
                        new Business.Infrastructure.FilterInfo { Field = "id_car", Operator = "eq", Value = model.IdCar.ToString() }
                    }
                };
                carExpenses = RepoCarExpense.FindAll(null, null, null, filters);
                
                return Json(new CarDetailReportPresentationStub().MapList(expenseItems, carExpenses), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
