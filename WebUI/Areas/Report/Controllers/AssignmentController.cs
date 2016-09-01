using Business.Abstract;
using Business.Entities;
using MvcSiteMapProvider;
using MvcSiteMapProvider.Web.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebUI.Areas.Report.Models;
using WebUI.Areas.Report.Models.Assignment;
//using WebUI.Areas.Report.Models.Assignment;
using WebUI.Controllers;
using WebUI.Infrastructure;
using WebUI.Infrastructure.Concrete;
using WebUI.Models.Booking;
using WebUI.Models.Car;

namespace WebUI.Areas.Report.Controllers
{
    [AuthorizeUser(ModuleName = "Assignment")]
    public class AssignmentController : MyController
    {
        private IRentRepository RepoRent;
        private ICarRepository RepoCar;
        private ICarModelRepository RepoCarModel;

        public AssignmentController(ILogRepository repoLog, IRentRepository repoRent, ICarRepository repoCar, ICarModelRepository repoCarModel)
            : base(repoLog)
        {
            RepoRent = repoRent;
            RepoCar = repoCar;
            RepoCarModel = repoCarModel;
        }

        [MvcSiteMapNode(Title = "Penugasan", ParentKey = "Dashboard", Key = "IndexAssignment")]
        [SiteMapTitle("Breadcrumb")]
        public ActionResult Index()
        {
            //kamus
            Guid? idOwner = (User as CustomPrincipal).IdOwner;
            List<car> dbCarList = new List<car>();
            List<CarPresentationStub> pCarList = new List<CarPresentationStub>();
            Dictionary<string, string> jsonCarListByModel = new Dictionary<string, string>();
            List<string> carModels;
            List<GenericModel> gCarList;
            Boolean active = true;
            Business.Infrastructure.FilterInfo filters;

            //algoritma
            filters = new Business.Infrastructure.FilterInfo
            {
                Filters = new List<Business.Infrastructure.FilterInfo>
                {
                    new Business.Infrastructure.FilterInfo { Field = "id_owner", Operator = "eq", Value = idOwner.ToString() },
                    new Business.Infrastructure.FilterInfo { Field = "is_active", Operator = "eq", Value = active.ToString() },
                },
                Logic = "and"
            };
            dbCarList = RepoCar.FindAll(null, null, null, filters);
            pCarList = new CarPresentationStub().MapList(dbCarList);
            carModels = pCarList.Select(m => m.CarModelName).Distinct().ToList();
            
            foreach (string cm in carModels)
            {
                gCarList = new List<GenericModel>();
                foreach (CarPresentationStub single in pCarList.Where(n => n.CarModelName == cm).ToList())
                {
                    gCarList.Add(new GenericModel { text = single.LicensePlate, value = single.LicensePlate });
                }
                jsonCarListByModel.Add(cm.Replace(" ","_"), new JavaScriptSerializer().Serialize(gCarList));
            }

            //viewbag
            ViewBag.JsonCarListByModel = jsonCarListByModel;

            return View(new DailyFilterModel());
        }

        //public string Binding()
        //{
        //    //kamus
        //    GridRequestParameters param = GridRequestParameters.Current;
        //    List<rent> items = null;

        //    //algoritma
        //    Guid idOwner = (Guid)(User as CustomPrincipal).IdOwner;
        //    DateTime start = DateTime.Parse("2016-01-01");
        //    DateTime finish = DateTime.Parse("2016-01-10");
        //    items = RepoRent.FindAll(idOwner, start, finish);

        //    return new JavaScriptSerializer().Serialize(new BookingPresentationStub().MapList(items));
        //}

        public JsonResult Binding()
        {
            //kamus
            GridRequestParameters param = GridRequestParameters.Current;
            Guid idOwner = (Guid)(User as CustomPrincipal).IdOwner;

            List<rent> rents;
            List<car> cars;
            List<car_model> carModels = new List<car_model>();

            DailyFilterModel fm = ParseFilterInfo(param.Filters);
            List<BookingPresentationStub> result = new List<BookingPresentationStub>();
            Business.Infrastructure.FilterInfo filters;
            List<string> carModel = new List<string>();
            BookingPresentationStub child;
            List<Guid> carsId;

            DateTime dtStart, dtEnd;
            DateTimeOffset dtoStart, dtoEnd;

            //algoritma
            dtStart = new DateTime(fm.StartDate.Year, fm.StartDate.Month, fm.StartDate.Day, fm.StartDate.Hour, fm.StartDate.Minute, fm.StartDate.Second);
            dtStart = DateTime.SpecifyKind(dtStart, DateTimeKind.Local);
            dtoStart = dtStart;

            dtEnd = new DateTime(fm.EndDate.Year, fm.EndDate.Month, fm.EndDate.Day, fm.EndDate.Hour, fm.EndDate.Minute, fm.EndDate.Second);
            dtEnd = DateTime.SpecifyKind(dtEnd, DateTimeKind.Local);
            dtoEnd = dtEnd;

            rents = RepoRent.FindAll(idOwner, fm.StartDate, fm.EndDate);
           
            //menambahkan filter terkait CarModelName
            filters = new Business.Infrastructure.FilterInfo
            {
                Filters = new List<Business.Infrastructure.FilterInfo>
                {
                    new Business.Infrastructure.FilterInfo { Field = "id_owner", Operator = "eq", Value = idOwner.ToString() },
                    new Business.Infrastructure.FilterInfo { Field = "car_model.name", Operator = "eq", Value = fm.CarModelName },
                    new Business.Infrastructure.FilterInfo { Field = "is_active", Operator = "eq", Value = true.ToString() },
                },
                Logic = "and"
            };            
            cars = RepoCar.FindAll(null, null, null, filters);
            carsId = cars.Select(m => m.id).ToList();

            rents = rents.Where(m => m.id_car != null && carsId.Contains(m.id_car.Value)).ToList();
            foreach (rent r in rents)
            {
                child = new BookingPresentationStub(r);

                result.Add(child);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #region private

        private DailyFilterModel ParseFilterInfo(Business.Infrastructure.FilterInfo filter)
        {
            DailyFilterModel fm = new DailyFilterModel();
            Business.Infrastructure.FilterInfo single;
            DateTime start, end;

            if (filter != null && filter.Filters != null)
            {
                single = filter.Filters.Where(m => m.Field == "StartDate").FirstOrDefault();
                if (single != null)
                {
                    start = DateTime.Parse(single.Value);
                    fm.StartDate = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0);
                }

                single = filter.Filters.Where(m => m.Field == "EndDate").FirstOrDefault();
                if (single != null)
                {
                    end = DateTime.Parse(single.Value);
                    fm.EndDate = new DateTime(end.Year, end.Month, end.Day, 23, 59, 59);
                }

                single = filter.Filters.Where(m => m.Field == "CarModelName").FirstOrDefault();
                if (single != null)
                {
                    fm.CarModelName = single.Value;
                }
            }

            return fm;
        }

        #endregion
    }
}
