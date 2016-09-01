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
    public class CityController : ApiController
    {
        public CityController()
        {
        }

        [HttpPost]
        public List<City> FindAll()
        {
            //kamus
            List<City> result = new List<City>();
            City city;
            Dictionary<string, Guid> cityMap;

            //algoritma
            cityMap = new Dictionary<string, Guid>();
            cityMap.Add("bali", new Guid("9e9af49f-742a-40a2-9338-228e56060442"));
            cityMap.Add("surabaya", new Guid("7a683eb8-656d-4b4c-be71-157dd2328a64"));
            
            foreach (KeyValuePair<string,Guid> x in cityMap)
            {
                city = new City();
                city.CityName = x.Key;
                result.Add(city);
            }

            return result;
        }
    }
}
