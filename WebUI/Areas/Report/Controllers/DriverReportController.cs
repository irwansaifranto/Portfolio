using Business.Abstract;
using Business.Entities;
using Common.Enums;
using MvcSiteMapProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebUI.Areas.Report.Models;
using WebUI.Areas.Report.Models.DriverReport;
using WebUI.Controllers;
using WebUI.Infrastructure;
using WebUI.Infrastructure.Concrete;

namespace WebUI.Areas.Report.Controllers
{
    [AuthorizeUser(ModuleName = "Driver Report")]
    public class DriverReportController : MyController
    {
        private IExpenseRepository RepoExpense;
        private IDriverRepository RepoDriver;
        private IRentRepository RepoRent;

        public DriverReportController(ILogRepository repoLog, IExpenseRepository repoExpense, IDriverRepository repoDriver, IRentRepository repoRent)
            : base(repoLog)
        {
            RepoExpense = repoExpense;
            RepoDriver = repoDriver;
            RepoRent = repoRent;
        }

        [MvcSiteMapNode(Title = "Laporan Supir", ParentKey = "Dashboard", Key = "IndexDriverReport")]
        public ActionResult Index()
        {
            return View(new DailyFilterModel());
        }

        [MvcSiteMapNode(Title = "Detail", ParentKey = "IndexDriverReport", Key = "DetailDriverReport")]
        public ActionResult Detail(Guid id, DateTime startDate, DateTime endDate)
        {
            DailyFilterModel filter = new DailyFilterModel(startDate, endDate);
            driver d = RepoDriver.FindByPk(id);

            ViewBag.DriverId = id;
            ViewBag.DriverName = d.name;

            return View(filter);
        }

        public string Binding(DailyFilterModel model)
        {
            //kamus
            List<rent> rents;
            List<driver> drivers;
            List<expense_item> expenseItems;
            Guid idOwner = (User as CustomPrincipal).IdOwner.Value;
            DateTime dtStart, dtEnd;

            Business.Infrastructure.FilterInfo filters;

            //algoritma
            //items = RepoExpense.GetDriverReport(idOwner, model.StartDate, model.EndDate);

            dtStart = new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day, 0, 0, 0);
            dtEnd = new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day, 23, 59, 59);

            //mengambil data booking
            rents = RepoRent.FindAll(idOwner, dtStart, dtEnd);

            //mengambil data driver
            filters = new Business.Infrastructure.FilterInfo
            {
                Filters = new List<Business.Infrastructure.FilterInfo>
                {
                    new Business.Infrastructure.FilterInfo { Field = "id_owner", Operator = "eq", Value = idOwner.ToString() },
                }
            };
            drivers = RepoDriver.FindAll(null, null, null, filters);

            //mengambil data epneseItem untuk pemasukan
            filters = new Business.Infrastructure.FilterInfo
            {
                Filters = new List<Business.Infrastructure.FilterInfo>
                {
                    new Business.Infrastructure.FilterInfo { Field = "expense.date", Operator = "gte", Value = dtStart.ToString() },
                    new Business.Infrastructure.FilterInfo { Field = "expense.date", Operator = "lte", Value = dtEnd.ToString() },
                    new Business.Infrastructure.FilterInfo { Field = "category", Operator = "eq", Value = ExpenseItemCategory.DRIVER.ToString() },
                    new Business.Infrastructure.FilterInfo { Field = "expense.rent.id_owner", Operator = "eq", Value = idOwner.ToString() },
                }
            };
            expenseItems = RepoExpense.FindAllItem(null, null, null, filters);

            return new JavaScriptSerializer().Serialize(new SummaryStub().MapList(drivers, rents, expenseItems));
        }

        public string BindingDetail()
        {
            //kamus
            GridRequestParameters param = GridRequestParameters.Current;

            List<expense_item> items = RepoExpense.FindAllItem(param.Skip, param.Take, (param.Sortings != null ? param.Sortings.ToList() : null), param.Filters);
            int total = RepoExpense.CountItem(param.Filters);

            return new JavaScriptSerializer().Serialize(new { total = total, data = new DetailStub().MapList(items) });
        }
    }
}
