using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Business.Entities;
using Common.Enums;
using System.ComponentModel;

namespace WebUI.Models.Car
{
    public class CarPresentationStub
    {
		// Example model value from scaffolder script: 0
		public System.Guid Id { get; set; }

        [DisplayName("Model")]
		public System.Guid IdCarModel { get; set; }

        [DisplayName("Plat Nomor")]
		public string LicensePlate { get; set; }
		public int? Capacity { get; set; }
		public string Status { get; set; }

        [DisplayName("Kendaraan Masih Tersedia")]
		public bool IsActive { get; set; }
		public string CarModelName { get; set; }
        public string CarBrandName { get; set; }

        [DisplayName("Tahun")]
        public int? ModelYear { get; set; }

        [DisplayName("Transmisi")]
        public string Transmission { get; set; }

        [DisplayName("BBM")]
        public string Fuel { get; set; }

        [DisplayName("Warna")]
        public string Color { get; set; }

        [DisplayName("Foto Mobil")]
        public string Photo { get; set; }

        [DisplayName("Merek")]
        public System.Guid IdCarBrand { get; set; }

        [DisplayName("STNK")]
        public string VehicleRegistrationFile { get; set; }

        [DisplayName("Pajak Kendaraan")]
        public string TaxFile { get; set; }

		public CarPresentationStub() { }

        public CarPresentationStub(car dbItem)
        {
            this.Id = dbItem.id;
            this.IdCarBrand = dbItem.car_model.car_brand.id;
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
            this.VehicleRegistrationFile = dbItem.vehicle_registration_file;
            this.TaxFile = dbItem.tax_file;
            this.CarModelName = dbItem.car_model != null ? dbItem.car_model.name : "";
            this.CarBrandName = dbItem.car_model.car_brand.name;
            if (dbItem.photo == null)
                this.Photo = VirtualPathUtility.ToAbsolute("~/Content/theme/noimage.png");
            else
                this.Photo = VirtualPathUtility.ToAbsolute(dbItem.photo);

            EnumHelper eh = new EnumHelper();
            if (dbItem.transmission != null && dbItem.transmission != "")
            {
                CarTransmission type = (CarTransmission)Enum.Parse(typeof(Common.Enums.CarTransmission), dbItem.transmission);
                Transmission = eh.GetEnumDescription(type);
            }
        }

		public List<CarPresentationStub> MapList(List<car> dbItems)
        {
            List<CarPresentationStub> retList = new List<CarPresentationStub>();

            foreach (car dbItem in dbItems)
                retList.Add(new CarPresentationStub(dbItem));

            return retList;
        }
	}
}

