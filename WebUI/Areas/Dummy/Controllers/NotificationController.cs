using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUI.Controllers;
using Business.Abstract;
using WebUI.Models.Booking;
using Common.Enums;
using Business.Entities;
using WebUI.Infrastructure.Concrete;
using WebUI.Areas.Dummy.Models;
using System.Web.Script.Serialization;


namespace WebUI.Areas.Dummy.Controllers
{
    public class NotificationController : MyController
    {
        //
        // GET: /Dummy/Notification/
        private IDummyNotificationRepository RepoDummyNotification;
        private IRentRepository RepoRent;
        private IOwnerRepository RepoOwner;
        private ICustomerRepository RepoCustomer;
        private ICarModelRepository RepoCarModel;

        public NotificationController(ILogRepository repoLog, IDummyNotificationRepository repoDummyNotification, IRentRepository repoRent, IOwnerRepository repoOwner, ICustomerRepository repoCustomer, ICarModelRepository repoCarModel)
            : base(repoLog)
        {
            RepoDummyNotification = repoDummyNotification;
            RepoRent = repoRent;
            RepoOwner = repoOwner;
            RepoCustomer = repoCustomer;
            RepoCarModel = repoCarModel;
        }

        public ActionResult Index()
        {
            return View();
        }

        public string Add()
        {
            //kamus
            string code;
            Guid idOwner = new Guid("156c2cde-2c19-46c3-bcba-a27f9d1e4998");
            owner owner = RepoOwner.FindByPk(idOwner);
            BookingFormStub bfs = new BookingFormStub();
            Business.Infrastructure.FilterInfo filters = new Business.Infrastructure.FilterInfo
            {
                Filters = new List<Business.Infrastructure.FilterInfo> 
                {
                    new Business.Infrastructure.FilterInfo { Field = "name", Operator = "eq", Value = "Avanza" }
                },
                Logic = "and"
            };
            car_model cm;
            DateTime dtStart, dtFinish;
            DateTimeOffset dtoStart, dtoFinish;

            //algoritma
            code = RepoRent.GenerateRentCode(owner);

            bfs.IdCustomer = new Guid("54612b5e-611f-4b71-9dbd-ed35b6f2976b");
            bfs.Code = code;
            bfs.PickupLocation = "Jl. Sudirman no. 16";

            cm = RepoCarModel.Find(null, filters);
            if (cm != null)
                bfs.IdCarModel = cm.id;

            dtStart = new DateTime(2016, 6, 4, 8, 0, 0);
            dtStart = DateTime.SpecifyKind(dtStart, DateTimeKind.Local);
            dtoStart = dtStart;

            dtFinish = new DateTime(2016, 6, 4, 8, 0, 0);
            dtFinish = DateTime.SpecifyKind(dtFinish, DateTimeKind.Local);
            dtoFinish = dtFinish;

            bfs.StartRent = dtoStart;
            bfs.FinishRent = dtoFinish;

            bfs.Price = 300000;
            bfs.Status = RentStatus.NEW;

            //save rent
            rent dbItem = bfs.GetDbObjectOnCreate("dummy", idOwner);
            RepoRent.Save(dbItem);

            //save notification
            NotificationFormStub nfs = new NotificationFormStub();

            nfs.Message = "Booking baru dari Citylink Cars";
            nfs.IdOwner = idOwner;

            d_notification notif = nfs.GetDBobject(idOwner);
            RepoDummyNotification.Save(notif);

            return new JavaScriptSerializer().Serialize(bfs);
        }

        public string Clear()
        {
            Guid idOwner = new Guid("156c2cde-2c19-46c3-bcba-a27f9d1e4998");
            Guid idCustomer = new Guid("54612b5e-611f-4b71-9dbd-ed35b6f2976b");

            Business.Infrastructure.FilterInfo filters = new Business.Infrastructure.FilterInfo
            {
                Filters = new List<Business.Infrastructure.FilterInfo>
                {
                    new Business.Infrastructure.FilterInfo { Field = "id_owner", Operator = "eq", Value = idOwner.ToString() },
                    new Business.Infrastructure.FilterInfo { Field = "id_customer", Operator = "eq", Value = idCustomer.ToString() },
                    new Business.Infrastructure.FilterInfo { Field = "created_by", Operator = "eq", Value = "dummy" }
                },
                Logic = "and"
            };

            List<rent> rents = RepoRent.FindAll(null, null, null, filters);
            int count = rents.Count();
            foreach (rent row in rents)
            {
                RepoRent.Delete(row);
            }

            return count.ToString() + " dihapus";
        }


        public string Get()
        {
            //kamus
            List<d_notification> notificationList;
            CustomPrincipal user = User as CustomPrincipal;
            Business.Infrastructure.FilterInfo filters = new Business.Infrastructure.FilterInfo { Filters = new List<Business.Infrastructure.FilterInfo>(), Logic = "and" };
            List<string> messageList;
            string response;

            //algoritma
            filters.Filters.Add(new Business.Infrastructure.FilterInfo { Field = "id_owner", Operator = "eq", Value = user.IdOwner.Value.ToString()});
            filters.Filters.Add(new Business.Infrastructure.FilterInfo { Field = "is_read", Operator = "eq", Value = false.ToString() });
            notificationList = RepoDummyNotification.FindAll(null, null, null, filters);

            foreach (d_notification n in notificationList)
            {
                if (n.is_read == false)
                {
                    n.is_read = true;
                }
                RepoDummyNotification.Save(n);
            }

            messageList = notificationList.Select(m => m.message).ToList();
            response = string.Join("<br>", messageList);

            return response;
        }
    }
}
