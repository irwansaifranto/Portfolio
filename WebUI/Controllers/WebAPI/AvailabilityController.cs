using Business.Abstract;
using Business.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using WebUI.Infrastructure;
using WebUI.Infrastructure.Concrete;
using WebUI.Models.Service;
using WebUI.Models.Service.Parameter;

namespace WebUI.Controllers.WebAPI
{
    [Authorize]
    public class AvailabilityController : ApiController
    {
        private IRentRepository RepoRent;
        private ICarRepository RepoCar;
        private ILogRepository RepoLog;
        public AvailabilityController(IRentRepository repoRent, ICarRepository repoCar, ILogRepository repoLog)
        {
            RepoRent = repoRent;
            RepoCar = repoCar;
            RepoLog = repoLog;
        }

        [HttpPost]
        public HttpResponseMessage CheckAvailability([FromBody] CheckAvailabilityParam param)
        {
            //kamus
            List<CarModel> result = new List<CarModel>();
            DisplayFormatHelper dfh = new DisplayFormatHelper();
            List<car> ownerCars = new List<car>();
            List<rent> rents = new List<rent>();
            Dictionary<string, Guid> cityMap;
            Guid idOwner = new Guid();
            List<Guid> rentedCars;
            string message = null;
            HttpResponseMessage responseMessage = new HttpResponseMessage();
            HttpStatusCode httpStatus = HttpStatusCode.OK;
            bool isDataValid = true;
            log_ws logWs = new log_ws();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Business.Infrastructure.FilterInfo filters = new Business.Infrastructure.FilterInfo { Filters = new List<Business.Infrastructure.FilterInfo>(), Logic = "and" };

            DateTime dtStart, dtFinish;
            DateTimeOffset dtoStart, dtoFinish;

            //algoritma
            //hardcode data
            cityMap = new Dictionary<string, Guid>();
            cityMap.Add("bali", new Guid("9e9af49f-742a-40a2-9338-228e56060442"));
            cityMap.Add("surabaya", new Guid("7a683eb8-656d-4b4c-be71-157dd2328a64"));

            //mengecek validitas data
            if (param.StartRent == null || param.FinishRent == null)
            {
                isDataValid = false;
                message = HttpContext.GetGlobalResourceObject("WebServiceMessage", "DateNullAlert").ToString();
                httpStatus = HttpStatusCode.BadRequest;
            }

            //algoritma
            if (isDataValid)
            {
                dtStart = new DateTime(param.StartRent.Value.Year, param.StartRent.Value.Month, param.StartRent.Value.Day, 0, 0, 0);
                dtStart = DateTime.SpecifyKind(dtStart, DateTimeKind.Local);
                dtoStart = dtStart;

                dtFinish = new DateTime(param.FinishRent.Value.Year, param.FinishRent.Value.Month, param.FinishRent.Value.Day, 23, 59, 59);
                dtFinish = DateTime.SpecifyKind(dtFinish, DateTimeKind.Local);
                dtoFinish = dtFinish;

                param.City = param.City.ToLower();
                if (cityMap.ContainsKey(param.City))
                {
                    idOwner = cityMap[param.City];

                    //mengambil rent dari start s/d finish untuk owner tertentu
                    rents = RepoRent.FindAll(idOwner, dtStart, dtFinish);

                    rents.RemoveAll(m => m.finish_rent == dtoStart);
                    rents.RemoveAll(m => m.start_rent == dtoFinish);

                    //mengambil car sesuai parameter                                                                                                                                                                                                                      //mengambil car sesuai parameter
                    filters.Filters.Add(new Business.Infrastructure.FilterInfo { Field = "id_owner", Operator = "eq", Value = idOwner.ToString() });
                    if (param.CarBrandName != null)
                    {
                        filters.Filters.Add(new Business.Infrastructure.FilterInfo { Field = "car_model.car_brand.name", Operator = "eq", Value = param.CarBrandName });
                    }
                    if (param.CarModelName != null)
                    {
                        filters.Filters.Add(new Business.Infrastructure.FilterInfo { Field = "car_model.name", Operator = "eq", Value = param.CarModelName });
                    }
                    if (param.Capacity != null)
                    {
                        filters.Filters.Add(new Business.Infrastructure.FilterInfo { Field = "car_model.capacity", Operator = "gte", Value = param.Capacity.Value.ToString() });
                    }
                    ownerCars = RepoCar.FindAll(null, null, null, filters);

                    //menghapus car yang sudah ada di rent               
                    rentedCars = rents.Select(m => m.id_car_model).ToList();

                    //menghapus mobil yang sudah di-booking
                    car carByModel;
                    foreach (Guid idCarModel in rentedCars)
                    {
                        carByModel = ownerCars.Where(m => m.id_car_model == idCarModel).FirstOrDefault();
                        if (carByModel != null)
                        {
                            ownerCars.Remove(carByModel);
                        }
                    }

                    result = CarModel.Map(ownerCars); //mapping dbItems ke PresentationStub

                }
            }
            
            //menyimpan data ke log_ws
            //menggunakan ILogRepository.SaveLogWS

            logWs.created_time = DateTimeOffset.Now;
            logWs.request_body = serializer.Serialize(param);
            logWs.response_body = serializer.Serialize(result);
            logWs.url = HttpContext.Current.Request.Url.AbsolutePath;
            RepoLog.SaveLogWS(logWs);

            if (message != null)
            {
                HttpContext.Current.Response.AppendHeader("ResponseMessage", message);
            }

            return Request.CreateResponse(httpStatus, result);
        }
    }
}
