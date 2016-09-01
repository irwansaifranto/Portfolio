using Business.Entities;
using Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebUI.Areas.Dummy.Models
{
    public class NotificationFormStub
    {

        // d_notification
        [Key]
        public int Id { get; set; }
        public string Message { get; set; }
        public Guid IdOwner { get; set; }

        public bool IsRead = false;

        //rent
        //public Guid IdRent { get; set; }
        //public Guid? IdCustomer { get; set; }
        //public string PickupLocation { get; set; }
        //public Guid? IdDriver { get; set; }
        //public Guid? IdCarModel { get; set; }
        //public Guid? IdCar { get; set; }
        //public DateTimeOffset StartRent { get; set; }
        //public DateTimeOffset FinishRent { get; set; }
        //public int? Price { get; set; }
        //public string Code { get; set; }
        //public string CancelNotes { get; set; }
        //public string Notes { get; set; }
        //public RentStatus Status { get; set; }
        public NotificationFormStub() 
        {
       
        }

        public d_notification GetDBobject( Guid idOwner)
        {
            d_notification dbItem = new d_notification {
                id = Id,
                message = Message,
                is_read = IsRead,
                id_owner = idOwner,
            };
            
            return dbItem;
        }


    }
}
