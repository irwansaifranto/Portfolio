using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Business.Entities;

namespace WebUI.Models.CarModel
{
    public class CarModelFormStub
    {
		// Example model value from scaffolder script: 0
		[DisplayName("Id")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
		public System.Guid Id { get; set; }

		[DisplayName("Nama")]
        [MaxLength(100)]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
		public string Name { get; set; }

		[DisplayName("Merek")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
		public System.Guid IdCarBrand { get; set; }

        [DisplayName("Kapasitas")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public Int16 Capacity { get; set; }

		public List<SelectListItem> CarBrandOptions { get; set; }       

		public CarModelFormStub() { }

		public CarModelFormStub(List<Business.Entities.car_brand> listCarBrand)
			: this()
		{
			this.FillCarBrandOptions(listCarBrand);
		}


		public CarModelFormStub(car_model dbItem,List<Business.Entities.car_brand> listCarBrand)
			: this(listCarBrand)
		{
			this.Id = dbItem.id;
			this.Name = dbItem.name;
			this.IdCarBrand = dbItem.id_car_brand;
            this.Capacity = dbItem.capacity;
		}

		public car_model GetDbObject(car_model dbItem) {
			dbItem.id = this.Id;
			dbItem.name = this.Name;
			dbItem.id_car_brand = this.IdCarBrand;
            dbItem.capacity = this.Capacity;
;
			return dbItem;
		}

		#region options

		public void FillCarBrandOptions(List<Business.Entities.car_brand> list)
		{
			CarBrandOptions = new List<SelectListItem>();
			CarBrandOptions.Add(new SelectListItem { Text = "Choose One", Value = "" });
			foreach (Business.Entities.car_brand item in list)
            {
                CarBrandOptions.Add(new SelectListItem { Text = item.name, Value = item.id.ToString() });
            }
		
		}       

		#endregion

	}
}

