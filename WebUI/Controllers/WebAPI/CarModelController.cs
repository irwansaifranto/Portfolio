using Business.Abstract;
using Business.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebUI.Infrastructure;
using WebUI.Infrastructure.Concrete;
using WebUI.Models.Service;
using WebUI.Models.Service.Parameter;

namespace WebUI.Controllers.WebAPI
{
    [Authorize]
    public class CarModelController : ApiController
    {
        private ICarModelRepository RepoCarModel;
        private ICarRepository RepoCar;

        public CarModelController(ICarModelRepository repoCarModel, ICarRepository repoCar)
        {
            RepoCarModel = repoCarModel;
            RepoCar = repoCar;
        }

        [HttpPost]
        public List<CarModel> FindAll()
        {
            //dummy
            Dictionary<string, Guid> cityMap = new Dictionary<string, Guid>();
            cityMap.Add("bali", new Guid("9e9af49f-742a-40a2-9338-228e56060442"));
            cityMap.Add("surabaya", new Guid("7a683eb8-656d-4b4c-be71-157dd2328a64"));

            //kamus
            List<CarModel> result;
            List<car> cars, ownerCars;
            Business.Infrastructure.FilterInfo filters;

            //algoritma
            //brands = RepoCarBrand.FindAll();
            cars = new List<car>();
            foreach (KeyValuePair<string, Guid> single in cityMap)
            {
                filters = new Business.Infrastructure.FilterInfo { Field = "id_owner", Operator = "eq", Value = single.Value.ToString() };
                ownerCars = RepoCar.FindAll(null, null, null, filters);
                cars.AddRange(ownerCars);
            }

            //foreach (car_brand r in brands)
            //{
            //    carbrand = new CarBrand();
            //    carbrand.CarBrandName = r.name;
            //    result.Add(carbrand);
            //}

            result = CarModel.Map(cars);

            return result;
        }
    }
}
