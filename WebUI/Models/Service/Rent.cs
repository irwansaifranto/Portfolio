using Business.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.Models.Service
{
    public class Rent
    {
        public string RentCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string PickupLocation { get; set; }
        public string DriverName { get; set; }
        public string CarBrandName { get; set; }
        public string CarModelName { get; set; }
        public string CarLicensePlate { get; set; }
        public DateTime? StartRent { get; set; }
        public DateTime? FinishRent { get; set; }

        public Rent() { }

        public Rent(rent dbItem)
        {
            this.RentCode = dbItem.code;
            this.CustomerName = dbItem.customer.name;
            this.CustomerPhoneNumber = dbItem.customer.phone_number;
            this.PickupLocation = dbItem.pickup_location;

            if (dbItem.driver != null)
            {
                this.DriverName = dbItem.driver.name;
            }

            this.CarBrandName = dbItem.car_model.car_brand.name;
            this.CarModelName = dbItem.car_model.name;

            if (dbItem.car != null)
            {
                this.CarLicensePlate = dbItem.car.license_plate;
            }

            this.StartRent = new DateTime(dbItem.start_rent.Year, dbItem.start_rent.Month, dbItem.start_rent.Day, dbItem.start_rent.Hour, dbItem.start_rent.Minute, dbItem.start_rent.Second);
            this.FinishRent = new DateTime(dbItem.finish_rent.Year, dbItem.finish_rent.Month, dbItem.finish_rent.Day, dbItem.finish_rent.Hour, dbItem.finish_rent.Minute, dbItem.finish_rent.Second);
        }

        //public static List<RentModel> Map(List<car> dbItems)
        //{
        //    List<RentModel> retlist = new List<RentModel>();
        //    foreach (car c in dbItems)
        //        retlist.Add(new RentModel(c));
        //    return retlist;
        //}
    }
}