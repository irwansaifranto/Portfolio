using Business.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.Models.Service
{
    public class CarBrand
    {
        public string CarBrandName { get; set; }
        public CarBrand() { }

        public CarBrand(car_brand dbitem)
        {
            CarBrandName = dbitem.name;            
        }

        public static List<CarBrand> Map(List<car_brand> dbItems)
        {
            List<CarBrand> retlist = new List<CarBrand>();
            foreach (car_brand c in dbItems)
                retlist.Add(new CarBrand(c));
            return retlist;
        }

        public static List<CarBrand> Map(List<car> dbItems)
        {
            List<CarBrand> retlist = new List<CarBrand>();
            List<string> brands = dbItems.Select(m => m.car_model.car_brand.name).Distinct().ToList();

            foreach (string row in brands)
            {
                retlist.Add(new CarBrand { CarBrandName = row });
            }

            return retlist;
        }
    }
}