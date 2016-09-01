using Business.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.Models.Service
{
    public class CarModel
    {
        public string CarBrandName { get; set; }
        public string CarModelName { get; set; }
        public CarModel() { }

        public CarModel(car dbitem)
        {
            CarBrandName = dbitem.car_model.car_brand.name;
            CarModelName = dbitem.car_model.name;
        }

        public static List<CarModel> Map(List<car> dbItems)
        {
            List<CarModel> retlist = new List<CarModel>();
            List<car_model> carModels = dbItems.Select(m => m.car_model).Distinct().ToList();

            foreach (car_model row in carModels)
            {
                retlist.Add(new CarModel { CarBrandName = row.car_brand.name, CarModelName = row.name });
            }

            return retlist;
        }
    }
}