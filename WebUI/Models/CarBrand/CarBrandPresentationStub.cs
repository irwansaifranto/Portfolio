using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Business.Entities;

namespace WebUI.Models.CarBrand
{
    public class CarBrandPresentationStub
    {
        public System.Guid Id { get; set; }
        public System.Guid IdCarBrand { get; set; }
		public string Name { get; set; }
		
		public CarBrandPresentationStub() { }

		public CarBrandPresentationStub(car_brand dbItem) {
            this.Id = dbItem.id;
            this.IdCarBrand = dbItem.id;
			this.Name = dbItem.name;
		}

		public List<CarBrandPresentationStub> MapList(List<car_brand> dbItems)
        {
            List<CarBrandPresentationStub> retList = new List<CarBrandPresentationStub>();

            foreach (car_brand dbItem in dbItems)
                retList.Add(new CarBrandPresentationStub(dbItem));

            return retList;
        }
	}
}

