using Business.Abstract;
using Business.Entities;
using Common.Enums;
using MvcSiteMapProvider;
using MvcSiteMapProvider.Web.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebUI.Areas.Report.Models;
using WebUI.Controllers;
using WebUI.Infrastructure;
using WebUI.Infrastructure.Concrete;
using WebUI.Models.Booking;

namespace WebUI.Areas.Report.Controllers
{
    [AuthorizeUser(ModuleName = "Utilization Report")]
    public class UtilizationReportController : MyController
    {
        private IRentRepository RepoRent;
        private ICarModelRepository RepoCarModel;
        private ICarRepository RepoCar;

        public UtilizationReportController(ILogRepository repoLog, IRentRepository repoRent, ICarRepository repoCar, ICarModelRepository repoCarModel)
            : base(repoLog)
        {
            RepoRent = repoRent;
            RepoCar = repoCar;
            RepoCarModel = repoCarModel;
        }

        [MvcSiteMapNode(Title = "Laporan Utilisasi", ParentKey = "Dashboard", Key = "IndexUtilizationReport")]
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
            List<rent> items = null;
            DailyFilterModel fm;
            Guid idOwner = (Guid)(User as CustomPrincipal).IdOwner;

            //algoritma
            fm = ParseFilterInfo(param.Filters);

            items = RepoRent.FindAll(idOwner, fm.StartDate, fm.EndDate).Where(n => n.id_car != null && n.car.is_active == true).ToList();

            result = GenerateReport(items, fm.StartDate, fm.EndDate);

            return new JavaScriptSerializer().Serialize(result);
        }

        #region private

        private DailyFilterModel ParseFilterInfo(Business.Infrastructure.FilterInfo filter)
        {
            DailyFilterModel fm = new DailyFilterModel();
            Business.Infrastructure.FilterInfo single;

            if (filter != null && filter.Filters != null)
            {
                single = filter.Filters.Where(m => m.Field == "StartDate").FirstOrDefault();
                if (single != null)
                {
                    fm.StartDate = DateTime.Parse(single.Value);
                    fm.StartDate = new DateTime(fm.StartDate.Year, fm.StartDate.Month, 1, 0, 0, 0);
                }

                single = filter.Filters.Where(m => m.Field == "EndDate").FirstOrDefault();
                if (single != null)
                {
                    fm.EndDate = DateTime.Parse(single.Value);
                    fm.EndDate = fm.EndDate.AddMonths(1);
                    fm.EndDate = new DateTime(fm.EndDate.Year, fm.EndDate.Month, 1, 23, 59, 59);
                    fm.EndDate = fm.EndDate.AddDays(-1);
                }
            }

            return fm;
        }

        private List<ChartAttribute> GenerateReport(List<rent> items, DateTime start, DateTime finish)
        {
            //kamus
            List<ChartAttribute> result = new List<ChartAttribute>();
            ChartAttribute oneMonthData;
            List<rent> rentsPerMonth, rentsPerDay;
            int totalUsage;
            double utilization;
            List<Guid> carInOneDay;
            int ownerCarsCount;
            DateTime iteration;
            int daysInMonth;
            DateTime dtStartMonth, dtEndMonth, dtStartDay, dtEndDay;
            DateTimeOffset dtoStartMonth, dtoEndMonth, dtoStartDay, dtoEndDay;
            Guid? idOwner = (User as CustomPrincipal).IdOwner;

            //algoritma
            //Business.Infrastructure.FilterInfo filters = new Business.Infrastructure.FilterInfo { Field = "id_owner", Operator = "eq", Value = idOwner.ToString() }; // fungsi semacam where
            Business.Infrastructure.FilterInfo filters = new Business.Infrastructure.FilterInfo();
            if (filters.Filters == null)
            {
                filters.Filters = new List<Business.Infrastructure.FilterInfo>();
                filters.Logic = "and";
            }

            filters.Filters.Add(new Business.Infrastructure.FilterInfo
            {
                Field = "id_owner",
                Operator = "eq",
                Value = idOwner.ToString()
            });
            filters.Filters.Add(new Business.Infrastructure.FilterInfo
            {
                Field = "is_active",
                Operator = "eq",
                Value = true.ToString()
            });
            ownerCarsCount = RepoCar.Count(filters);

            iteration = new DateTime(start.Year, start.Month, 1);
            finish = new DateTime(finish.Year, finish.Month, 1);

            while (iteration <= finish)
            {
                //koding
                //List<rent> tempRent = items.Where(n => (n.start_rent <= iteration && n.finish_rent >= iteration) || (n.start_rent >= start && n.start_rent <= finish)).ToList();
                oneMonthData = new ChartAttribute();
                totalUsage = 0;
                utilization = 0;

                //mengecek hari dalam sebulan
                daysInMonth = System.DateTime.DaysInMonth(iteration.Year, iteration.Month);

                //mengambil data rent dalam 1 bulan
                dtStartMonth = new DateTime(iteration.Year, iteration.Month, 1, 0, 0, 0);
                dtStartMonth = DateTime.SpecifyKind(dtStartMonth, DateTimeKind.Local);
                dtoStartMonth = dtStartMonth;

                dtEndMonth = new DateTime(iteration.Year, iteration.Month, DateTime.DaysInMonth(iteration.Year, iteration.Month), 23, 59, 59);
                dtEndMonth = DateTime.SpecifyKind(dtEndMonth, DateTimeKind.Local);
                dtoEndMonth = dtEndMonth;

                rentsPerMonth = items.Where(n =>
                    (n.start_rent <= dtoStartMonth && n.finish_rent >= dtoStartMonth) ||
                    (n.start_rent <= dtoEndMonth && n.finish_rent >= dtoEndMonth) ||
                    (n.start_rent >= dtoStartMonth && n.finish_rent <= dtoEndMonth) ||
                    (n.start_rent <= dtoStartMonth && n.finish_rent >= dtoEndMonth)
                ).ToList();

                //iteration for eachday
                for (int j = 1; j <= daysInMonth; j++)
                {
                    //DateTime dateStartTemp = new DateTime(iteration.Year, iteration.Month, j);
                    //DateTime dateFinishTemp = new DateTime(iteration.Year, iteration.Month, j);

                    //rentPerDay = rentPerMonth.Where(n => (n.start_rent <= dateStartTemp && n.finish_rent >= dateStartTemp) || (n.start_rent >= dateStartTemp && n.finish_rent <= dateFinishTemp) || (n.finish_rent >= dateStartTemp && n.finish_rent <= dateFinishTemp) || (n.start_rent.Day <= dateStartTemp.Day && n.finish_rent.Day >= dateStartTemp.Day) || (n.start_rent.Day >= dateStartTemp.Day && n.finish_rent.Day <= dateFinishTemp.Day) || (n.finish_rent.Day >= dateStartTemp.Day && n.finish_rent.Day <= dateFinishTemp.Day) || (n.start_rent.Day <= dateStartTemp.Day && n.finish_rent.Day >= dateStartTemp.Day && n.start_rent.Month <= dateStartTemp.Month && n.finish_rent.Month >= dateStartTemp.Month) || (n.start_rent.Day >= dateStartTemp.Day && n.finish_rent.Day <= dateFinishTemp.Day && n.start_rent.Month >= dateStartTemp.Month && n.finish_rent.Month <= dateFinishTemp.Month) || (n.finish_rent.Day >= dateStartTemp.Day && n.finish_rent.Day <= dateFinishTemp.Day && n.finish_rent.Day >= dateStartTemp.Day && n.finish_rent.Month <= dateFinishTemp.Month) || (n.start_rent <= dateStartTemp && n.start_rent <= dateFinishTemp)).ToList();
                    //rentPerDay = rentPerMonth.Where(n => (n.start_rent <= dateStartTemp && n.finish_rent >= dateStartTemp) || (n.start_rent >= dateStartTemp && n.finish_rent <= dateFinishTemp) || (n.finish_rent >= dateStartTemp && n.finish_rent <= dateFinishTemp) || (n.start_rent.Day <= dateStartTemp.Day && n.finish_rent.Day >= dateStartTemp.Day) || (n.start_rent.Day >= dateStartTemp.Day && n.finish_rent.Day <= dateFinishTemp.Day) || (n.finish_rent.Day >= dateStartTemp.Day && n.finish_rent.Day <= dateFinishTemp.Day)).ToList();
                    //List<rent> daysMonth = tempRent.Where(tempRent.GetRange().ToList();
                    //rentsPerDay = rentsPerMonth.Where(n => 
                    //    (n.start_rent <= dateStartTemp && n.finish_rent >= dateFinishTemp) || 
                    //    (n.start_rent >= dateStartTemp && n.start_rent <= dateFinishTemp) || 
                    //    (n.finish_rent >= dateStartTemp && n.finish_rent <= dateFinishTemp) || 
                    //    (n.start_rent.Day <= dateStartTemp.Day && n.finish_rent.Day >= dateStartTemp.Day) || 
                    //    (n.start_rent.Day >= dateStartTemp.Day && n.finish_rent.Day <= dateFinishTemp.Day) || 
                    //    (n.finish_rent.Day >= dateStartTemp.Day && n.finish_rent.Day <= dateFinishTemp.Day)
                    //).ToList();

                    dtStartDay = new DateTime(iteration.Year, iteration.Month, j, 0, 0, 0);
                    dtStartDay = DateTime.SpecifyKind(dtStartDay, DateTimeKind.Local);
                    dtoStartDay = dtStartDay;

                    dtEndDay = new DateTime(iteration.Year, iteration.Month, j, 23, 59, 59);
                    dtEndDay = DateTime.SpecifyKind(dtEndDay, DateTimeKind.Local);
                    dtoEndDay = dtEndDay;

                    rentsPerDay = rentsPerMonth.Where(n =>
                        (n.start_rent <= dtoStartDay && n.finish_rent >= dtoStartDay) ||
                        (n.start_rent <= dtoEndDay && n.finish_rent >= dtoEndDay) ||
                        (n.start_rent >= dtoStartDay && n.finish_rent <= dtoEndDay) ||
                        (n.start_rent <= dtoStartDay && n.finish_rent >= dtoEndDay)
                    ).ToList();

                    carInOneDay = new List<Guid>();

                    foreach (rent r in rentsPerDay)
                    {
                        if (r.id_car.HasValue)
                        {
                            if (!carInOneDay.Contains(r.id_car.Value))
                            {
                                ++totalUsage;
                                carInOneDay.Add(r.id_car.Value);
                            }
                        }
                    }
                }

                utilization = ((double)(totalUsage * 100)) / ((double)(daysInMonth * ownerCarsCount));
                oneMonthData.Value = utilization;
                oneMonthData.Category = new DateTime(iteration.Year, iteration.Month, 1);
                result.Add(oneMonthData);

                iteration = iteration.AddMonths(1);
            }

            return result;
        }

        #endregion
    }
}
