using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Business.Entities;
using Common.Enums;
using System.ComponentModel;
using WebUI.Models.Booking;

namespace WebUI.Areas.Administrator.Models
{
    public class RentPresentationStub
    {
        public BookingPresentationStub Base { get; set; }
        [DisplayName("Partners")]
        public string OwnerName { get; set; }
        [DisplayName("Tanggal Input")]
        public DateTimeOffset CreatedTime { get; set; }
        
        public RentPresentationStub() { }

        public RentPresentationStub(rent dbItem)
        {
            Base = new BookingPresentationStub(dbItem);
            CreatedTime = dbItem.created_time;
            OwnerName = dbItem.owner.name;
        }

        public List<RentPresentationStub> MapList(List<rent> dbItems)
        {
            List<RentPresentationStub> retList = new List<RentPresentationStub>();

            foreach (rent dbItem in dbItems)
                retList.Add(new RentPresentationStub(dbItem));

            return retList;
        }
    }
}