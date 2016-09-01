using Business.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebUI.Models.Booking
{
    public class DefaultMapPresentationStub
    {
        //
        // GET: /DefaultMapPresentationStub/
        // Default Map
        public int Id { get; set; }
        public string City { get; set; }
        public double LatitudeCity { get; set; }
        public double LongitudeCity { get; set; }

        public DefaultMapPresentationStub() {}

        public DefaultMapPresentationStub(city dbItem)
        {
            this.Id = dbItem.id;
            this.City = dbItem.name;
            this.LatitudeCity = dbItem.latitude;
            this.LongitudeCity = dbItem.longitude;
        }

        public List<DefaultMapPresentationStub> Map(List<city> dbItems)
        {
            List<DefaultMapPresentationStub> retList = new List<DefaultMapPresentationStub>();

            foreach (city dbItem in dbItems)
                retList.Add(new DefaultMapPresentationStub(dbItem));
            return retList;
        }
    }
}
