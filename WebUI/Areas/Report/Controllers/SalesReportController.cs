using Business.Abstract;
using Business.Entities;
using Common.Enums;
using MvcSiteMapProvider;
using MvcSiteMapProvider.Web.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebUI.Areas.Report.Models;
using WebUI.Controllers;
using WebUI.Infrastructure;
using WebUI.Infrastructure.Concrete;
using WebUI.Models.Invoice;

namespace WebUI.Areas.Report.Controllers
{
    [AuthorizeUser(ModuleName = "Sales Report")]
    public class SalesReportController : MyController
    {
        private IInvoiceRepository RepoInvoice;

        public SalesReportController(ILogRepository repoLog, IInvoiceRepository repoInvoice)
            : base(repoLog)
        {
            RepoInvoice = repoInvoice;
        }

        [MvcSiteMapNode(Title = "Laporan Penjualan", ParentKey = "Dashboard", Key = "IndexSalesReport")]
        [SiteMapTitle("Breadcrumb")]
        public ActionResult Index()
        {
            return View(new DailyMonthlyFilterModel());
        }

        public ActionResult Detail()
        {
            return View();
        }

        public string Binding()
        {
            //kamus
            DisplayFormatHelper dfh = new DisplayFormatHelper();
            GridRequestParameters param = GridRequestParameters.Current;
            List<ChartAttribute> result;
            List<invoice> items = null;
            DailyMonthlyFilterModel fm;
            Business.Infrastructure.FilterInfo filters;

            Guid idOwner = (Guid)(User as CustomPrincipal).IdOwner;

            //algoritma
            fm = ParseFilterInfo(param.Filters);
            filters = new Business.Infrastructure.FilterInfo 
            {
                Filters = new List<Business.Infrastructure.FilterInfo>
                {
                    new Business.Infrastructure.FilterInfo { Field = "rent.id_owner", Operator = "eq", Value = idOwner.ToString() },
                    new Business.Infrastructure.FilterInfo { Field = "invoice_date", Operator = "gte", Value = fm.StartDate.ToString(dfh.SqlDateFormat) },
                    new Business.Infrastructure.FilterInfo { Field = "invoice_date", Operator = "lte", Value = fm.EndDate.ToString(dfh.SqlDateFormat) },
                    new Business.Infrastructure.FilterInfo { Field = "status", Operator = "neq", Value = InvoiceStatus.CANCEL.ToString() }
                },
                Logic = "and"
            };

            items = RepoInvoice.FindAll(null, null, null, filters);
            result = GenerateReport(fm.ReportType, items, fm.StartDate, fm.EndDate);

            return new JavaScriptSerializer().Serialize(result);
        }

        #region private

        private List<ChartAttribute> GenerateReport(ReportType type, List<invoice> items, DateTime start, DateTime finish)
        {
            //kamus
            List<ChartAttribute> chart = new List<ChartAttribute>();
            ChartAttribute temp = new ChartAttribute();
            DateTime tempDate;

            //algoritma
            if (type.Equals(ReportType.DAILY))
            {
                tempDate = start;//kondisi startnya berdasarkan tanggal awal inputan buat pembanding
                while (tempDate <= finish)//dibandingkan dengan tanggal akhir inputan
                {

                    temp = new ChartAttribute();
                    temp.Category = tempDate;
                    temp.Value = items.Where(n => n.invoice_date == tempDate).Sum(m => m.total);

                    chart.Add(temp);
                    tempDate = tempDate.AddDays(1);
                }

            }
            else
            {
                tempDate = start;
                while (tempDate <= finish)
                {
                    temp = new ChartAttribute();
                    temp.Category = tempDate;
                    temp.Value = items.Where(n => n.invoice_date.Month == tempDate.Month).Sum(m => m.total);

                    chart.Add(temp);
                    tempDate = tempDate.AddMonths(1);
                }
            }

            return chart;
        }

        /// <summary>
        /// start: ... - ... - 01
        /// end: ... - ... - 30/31
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        private DailyMonthlyFilterModel ParseFilterInfo(Business.Infrastructure.FilterInfo filter)
        {
            DailyMonthlyFilterModel fm = new DailyMonthlyFilterModel();
            Business.Infrastructure.FilterInfo single;

            if (filter != null && filter.Filters != null)
            {
                single = filter.Filters.Where(m => m.Field == "ReportType").FirstOrDefault();

                if (single != null)
                    fm.ReportType = (ReportType)Enum.Parse(typeof(ReportType), single.Value);

                single = filter.Filters.Where(m => m.Field == "StartDate").FirstOrDefault();
                if (single != null)
                {
                    fm.StartDate = DateTime.Parse(single.Value);

                    if (fm.ReportType == ReportType.MONTHLY)
                    {
                        fm.StartDate = new DateTime(fm.StartDate.Year, fm.StartDate.Month, 1);
                    }
                }

                single = filter.Filters.Where(m => m.Field == "EndDate").FirstOrDefault();
                if (single != null)
                {
                    fm.EndDate = DateTime.Parse(single.Value);

                    if (fm.ReportType == ReportType.MONTHLY)
                    {
                        fm.EndDate = fm.EndDate.AddMonths(1);
                        fm.EndDate = new DateTime(fm.EndDate.Year, fm.EndDate.Month, 1);
                        fm.EndDate = fm.EndDate.AddDays(-1);
                    }
                }
            }

            return fm;
        }

        #endregion
    }
}
