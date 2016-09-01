using Business.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebUI.Infrastructure.Concrete;
using WebUI.Models.CarBrand;
using WebUI.Models.CarModel;

namespace WebUI.Models.CarPackage
{
    public class CarPackageFormStub
    {
        [DisplayName("Id")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public System.Guid Id { get; set; }

        [DisplayName("Merek")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public System.Guid? IdCarBrand { get; set; }

        [DisplayName("Model")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public System.Guid? IdCarModel { get; set; }

        [DisplayName("Paket")]
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public string Name { get; set; }

        public string CarBrandName { get; set; }

        public string CarModelName { get; set; }

        [DisplayName("Harga / Hari")]
        [Range(1, 2147483647, ErrorMessageResourceName = "Range", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public int? Price { get; set; }
        public int SubTotal { get; set; }

        [DisplayName("Paket Masih Tersedia")]
        public bool IsActive { get; set; }

        #region options

        private List<CarBrandPresentationStub> CarBrandOptions { get; set; }
        public List<CarBrandPresentationStub> GetCarBrandOptions()
        {
            return CarBrandOptions;
        }
        public string GetCarBrandOptionsAsJson()
        {
            return new JavaScriptSerializer().Serialize(CarBrandOptions);
        }
        public void SetCarBrandOptions(List<car_brand> dbItems)
        {
            CarBrandOptions = new CarBrandPresentationStub().MapList(dbItems);
        }

        private List<CarModelPresentationStub> CarModelOptions { get; set; }
        public List<CarModelPresentationStub> GetCarModelOptions()
        {
            return CarModelOptions;
        }
        public string GetCarModelOptionsAsJson()
        {
            return new JavaScriptSerializer().Serialize(CarModelOptions);
        }
        public void SetCarModelOptions(List<car_model> dbItems)
        {
            CarModelOptions = new CarModelPresentationStub().MapList(dbItems);
        }

        #endregion

        public CarPackageFormStub() 
        {
            IsActive = true;
        }

        public CarPackageFormStub(car_package dbItem)
        {
            this.Id = dbItem.id;
            this.IdCarBrand = dbItem.car_model.car_brand.id;
            this.IdCarModel = dbItem.id_car_model;
            this.Name = dbItem.name;
            this.Price = dbItem.price;
            this.CarBrandName = dbItem.car_model.car_brand.name;
            this.CarModelName = dbItem.car_model.name;
            this.IsActive = dbItem.is_active;
            this.SubTotal = dbItem.rent_package.FirstOrDefault().price_each;
        }

        public car_package GetDbObject(Guid idOwner, string user)
        {
            car_package dbItem = new car_package();

            dbItem.id = Id;
            dbItem.id_car_model = IdCarModel.Value;
            dbItem.name = Name;
            dbItem.price = Price.Value;
            dbItem.created_by = user;
            dbItem.id_owner = idOwner;
            dbItem.is_active = IsActive;

            dbItem.created_time = DateTimeOffset.Now;
            dbItem.created_by = user;

            return dbItem;
        }

        public car_package UpdateDbObject(car_package dbItem, CustomPrincipal user)
        {

            dbItem.id = Id;
            dbItem.id_car_model = IdCarModel.Value;
            dbItem.name = Name;
            dbItem.price = Price.Value;
            dbItem.is_active = IsActive;
            dbItem.id_owner = user.IdOwner.Value;

            dbItem.updated_time = DateTimeOffset.Now;
            dbItem.updated_by = user.Identity.Name;

            return dbItem;
        }

        //#region options

        //public void FillCarModelOptions(List<Business.Entities.car_model> list)
        //{
        //    CarModelOptions = new List<SelectListItem>();
        //    CarModelOptions.Add(new SelectListItem { Text = "Choose One", Value = "" });
        //    foreach (Business.Entities.car_model item in list)
        //    {
        //        CarModelOptions.Add(new SelectListItem { Text = item.name, Value = item.id.ToString() });
        //    }

        //}

        //public void FillCarBrandOptions(List<Business.Entities.car_brand> list)
        //{
        //    CarBrandOptions = new List<SelectListItem>();
        //    CarBrandOptions.Add(new SelectListItem { Text = "Choose One", Value = "" });
        //    foreach (Business.Entities.car_brand item in list)
        //    {
        //        CarBrandOptions.Add(new SelectListItem { Text = item.name, Value = item.id.ToString() });
        //    }

        //}

        //#endregion

    }
}