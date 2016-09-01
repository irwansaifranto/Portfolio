using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Business.Entities;

namespace WebUI.Models.CarModel
{
    public class CarModelPresentationStub
    {
		// Example model value from scaffolder script: 0
        public System.Guid Id { get; set; }
        public System.Guid IdCarModel { get; set; }
		public string Name { get; set; }
		public System.Guid IdCarBrand { get; set; }
		public string CarBrandName { get; set; }
        public Int16 Capacity { get; set; }
        
		
		public CarModelPresentationStub() { }

		public CarModelPresentationStub(car_model dbItem) {
            this.Id = dbItem.id;
            this.IdCarModel = dbItem.id;
			this.Name = dbItem.name;
			this.IdCarBrand = dbItem.id_car_brand;
			this.CarBrandName = dbItem.car_brand != null ? dbItem.car_brand.name : "";
            this.Capacity = dbItem.capacity;
		}

		public List<CarModelPresentationStub> MapList(List<car_model> dbItems)
        {
            List<CarModelPresentationStub> retList = new List<CarModelPresentationStub>();

            foreach (car_model dbItem in dbItems)
                retList.Add(new CarModelPresentationStub(dbItem));

            return retList;
        }
	}
}

