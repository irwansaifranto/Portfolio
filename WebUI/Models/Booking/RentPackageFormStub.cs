using Business.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebUI.Models.Booking
{
    public class RentPackageFormStub
    {
        public Guid Id { get; set; }
        public Guid IdRent { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public Guid IdCarPackage { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public int Quantity { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public int PriceEach { get; set; }

        public RentPackageFormStub() { }
        public RentPackageFormStub(rent_package dbItem) : this()
        {
            Id = dbItem.id;
            IdRent = dbItem.id_rent;
            IdCarPackage = dbItem.id_car_package;
            Quantity = dbItem.quantity;
            PriceEach = dbItem.price_each;
        }

        public rent_package GetDbObject(Guid RentId, Guid CarPackageId)
        {
            rent_package dbItem = new rent_package
            {
                id_rent = RentId,
                id_car_package = CarPackageId,
                quantity = Quantity,
                price_each = PriceEach,
            };

            return dbItem;
        }

        public void SetDbObject(rent_package dbItem)
        {
            dbItem.quantity = Quantity;
            dbItem.price_each = PriceEach;
        }
    }
}