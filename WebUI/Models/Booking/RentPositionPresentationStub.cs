using Business.Entities;
using Common.Enums;
using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace WebUI.Models.Booking
{
    public class RentPositionPresentationStub
    {

        // Buat googleMap
        public Guid Id { get; set; }
        public Guid IdRent { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset CreatedTimePos { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string StatusEnum { get; set; }


        public BookingPresentationStub Rent { get; set; }

        public RentPositionPresentationStub() { }

        public RentPositionPresentationStub(rent_position dbItem)
        {
            
            Id = dbItem.id;
            IdRent = dbItem.id_rent;
            CreatedBy = dbItem.created_by;
            CreatedTimePos = dbItem.created_time;
            Latitude = dbItem.latitude;
            Longitude = dbItem.longitude;
            RentStatus rs;

            if (dbItem.rent.status != null)
            {
                rs = (RentStatus)Enum.Parse(typeof(RentStatus), dbItem.rent.status);
                StatusEnum = rs.ToString();
                Status = new EnumHelper().GetEnumDescription(rs);
            }

            Rent = new BookingPresentationStub(dbItem.rent);
        }

        public List<RentPositionPresentationStub> Map(List<rent_position> dbItems)
        {
            List<RentPositionPresentationStub> retList = new List<RentPositionPresentationStub>();

            foreach (rent_position dbItem in dbItems)
                retList.Add(new RentPositionPresentationStub(dbItem));
            return retList;
        }
    }

    
}