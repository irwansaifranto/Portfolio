using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Business.Entities;
using Common.Enums;

namespace WebUI.Models.Car
{
    public class CarFormStub
    {
        // Example model value from scaffolder script: 0
        [DisplayName("Id")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public System.Guid Id { get; set; }

        [DisplayName("Merek")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public System.Guid IdCarBrand { get; set; }

        [DisplayName("Model")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public System.Guid IdCarModel { get; set; }

        [DisplayName("Plat Nomor")]
        [MaxLength(50)]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public string LicensePlate { get; set; }

        [DisplayName("Tahun")]
        public int? ModelYear { get; set; }

        [DisplayName("Transmisi")]
        public string Transmission { get; set; }

        [DisplayName("BBM")]
        public string Fuel { get; set; }

        [DisplayName("Warna")]
        [StringLength(16, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public string Color { get; set; }

        [DisplayName("Upload Foto Mobil")]
        public string Photo { get; set; }

        [DisplayName("Kendaraan Masih Tersedia")]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public bool IsActive { get; set; }
        public string Status { get; set; }
        public int? Capacity { get; set; }
        
        [DisplayName("STNK")]
        public string VehicleRegistrationFile { get; set; }

        [DisplayName("Pajak Kendaraan")]
        public string TaxFile { get; set; }

        public List<SelectListItem> CarModelOptions { get; set; }

        public List<SelectListItem> CarBrandOptions { get; set; }

        public List<SelectListItem> CarTransmissionOptions
        {
            get
            {
                EnumHelper enumHelper = new EnumHelper();

                List<SelectListItem> options = new List<SelectListItem>();
                foreach (CarTransmission item in enumHelper.EnumToList<CarTransmission>().ToList())
                    options.Add(new SelectListItem { Value = (item).ToString(), Text = enumHelper.GetEnumDescription(item) });

                return options;
            }
        }

        public List<SelectListItem> CarFuelOptions
        {
            get
            {
                EnumHelper enumHelper = new EnumHelper();

                List<SelectListItem> options = new List<SelectListItem>();
                foreach (CarFuel item in enumHelper.EnumToList<CarFuel>().ToList())
                    options.Add(new SelectListItem { Value = (item).ToString(), Text = enumHelper.GetEnumDescription(item) });

                return options;
            }
        }
        
        public List<SelectListItem> YearOptions
        {
            get
            {                
                List<SelectListItem> options = new List<SelectListItem>();
                int startYear = DateTime.Now.Year;
                int maxYear = startYear - 10;               
                for (int i = startYear; i >= maxYear; i--)
                {
                    options.Add(new SelectListItem { Value = (i).ToString(), Text = i.ToString() });
                }

                return options;
            }
        }
        public CarFormStub()
        {
            IsActive = true;
        }


        public CarFormStub(car dbItem)
        {

            this.Id = dbItem.id;
            this.IdCarModel = dbItem.id_car_model;
            this.LicensePlate = dbItem.license_plate;
            this.ModelYear = dbItem.model_year;
            this.Transmission = dbItem.transmission;
            this.Fuel = dbItem.fuel;
            this.Color = dbItem.color;
            this.Photo = dbItem.photo;
            this.Capacity = dbItem.capacity;
            this.Status = dbItem.status;
            this.IsActive = dbItem.is_active;
            this.IdCarBrand = dbItem.car_model.car_brand.id;
            this.VehicleRegistrationFile = dbItem.vehicle_registration_file;
            this.TaxFile = dbItem.tax_file;
        }

        public car GetDbObject(car dbItem, Guid idOwner)
        {
            //Logika untuk mengubah LicensePlate

            //string words = this.LicensePlate.Remove(words.ToString('_'));
            //dbItem.license_plate = words.ToString();
            //if (this.LicensePlate == ('_'))
            //{
            // this.LicensePlate.Remove('_');
            //}
            //else
            //{
            //    dbItem.license_plate = this.LicensePlate;
            //}
            //string[] ch = this.LicensePlate;
            //dbItem.license_plate = this.LicensePlate.Trim("_",ch);
            dbItem.license_plate = this.LicensePlate.Replace("_", "").ToUpper();

            dbItem.id = this.Id;
            dbItem.id_car_model = this.IdCarModel;            
            dbItem.model_year = this.ModelYear;
            dbItem.transmission = this.Transmission;
            dbItem.fuel = this.Fuel;
            dbItem.color = this.Color;
            dbItem.photo = this.Photo;
            dbItem.capacity = this.Capacity;
            dbItem.status = this.Status;
            dbItem.is_active = this.IsActive;
            dbItem.id_owner = idOwner;
            dbItem.vehicle_registration_file = this.VehicleRegistrationFile;
            dbItem.tax_file = this.TaxFile;

            return dbItem;
        }

        #region options

        public void FillCarModelOptions(List<Business.Entities.car_model> list)
        {
            CarModelOptions = new List<SelectListItem>();
            foreach (Business.Entities.car_model item in list)
            {
                CarModelOptions.Add(new SelectListItem { Text = item.name, Value = item.id.ToString() });
            }
            CarModelOptions = CarModelOptions.OrderBy(m => m.Text).ToList();
        }

        public void FillCarBrandOptions(List<Business.Entities.car_brand> list)
        {
            CarBrandOptions = new List<SelectListItem>();
            foreach (Business.Entities.car_brand item in list)
            {
                CarBrandOptions.Add(new SelectListItem { Text = item.name, Value = item.id.ToString() });
            }
            CarBrandOptions = CarBrandOptions.OrderBy(m => m.Text).ToList();
        }

        #endregion

    }
}

