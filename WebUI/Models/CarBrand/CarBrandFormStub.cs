using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Business.Entities;

namespace WebUI.Models.CarBrand
{
    public class CarBrandFormStub
    {
		// Example model value from scaffolder script: 0
		[DisplayName("Id")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
		public System.Guid Id { get; set; }

		[DisplayName("Nama")]
        [MaxLength(50)]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
		public string Name { get; set; }

		public CarBrandFormStub() { }


		public CarBrandFormStub(car_brand dbItem)
			: this()
		{
			this.Id = dbItem.id;
			this.Name = dbItem.name;
		}

		public car_brand GetDbObject(car_brand dbItem) {
			dbItem.id = this.Id;
			dbItem.name = this.Name;
			return dbItem;
		}

		#region options


		#endregion

	}
}

