using Business.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.Models.CarPackage
{
    public class CarPackagePresentationStub
    {
        public System.Guid Id { get; set; }
        public System.Guid IdCarBrand { get; set; }
        public System.Guid IdCarModel { get; set; }
        public string CarModelName { get; set; }
        public string CarBrandName { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public bool IsActive { get; set; }
        public int SubTotal { get; set; }

        public CarPackagePresentationStub() { }

        public CarPackagePresentationStub(car_package dbItem)
        {
            this.Id = dbItem.id;
            this.IdCarBrand = dbItem.car_model.car_brand.id;
            this.IdCarModel = dbItem.id_car_model;
            this.CarBrandName = dbItem.car_model.car_brand.name;
            this.CarModelName = dbItem.car_model != null ? dbItem.car_model.name : "";
            this.Name = dbItem.name;
            this.Price = dbItem.price;
            this.IsActive = dbItem.is_active;
            //this.SubTotal = dbItem.rent_package.FirstOrDefault().price_each;
        }

        public List<CarPackagePresentationStub> MapList(List<car_package> dbItems)
        {
            List<CarPackagePresentationStub> retList = new List<CarPackagePresentationStub>();

            foreach (car_package dbItem in dbItems)
                retList.Add(new CarPackagePresentationStub(dbItem));

            return retList;
        }
    }
}