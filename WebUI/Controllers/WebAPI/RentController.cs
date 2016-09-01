using Business.Abstract;
using Business.Entities;
using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebUI.Infrastructure;
using WebUI.Infrastructure.Concrete;
using WebUI.Models.ApiRent;
using WebUI.Models.Booking;
using WebUI.Models.Service;
using WebUI.Models.Service.Parameter;

namespace WebUI.Controllers.WebAPI
{   
    [Authorize]
    public class RentController : ApiController
    {
        private IRentRepository RepoRent;
        private ICarRepository RepoCar;
        private ICustomerRepository RepoCustomer;
        private IOwnerRepository RepoOwner;
        private ICarModelRepository RepoCarModel;
        private IApiRentRepository RepoApiRent;
        private ILogRepository RepoLog;

        public RentController(IRentRepository repoRent, ICarRepository repoCar, ICustomerRepository repoCustomer, IOwnerRepository repoOwner, ICarModelRepository repoCarModel, IApiRentRepository repoRentApi, ILogRepository repoLog)
        {
            RepoRent = repoRent;
            RepoCar = repoCar;
            RepoCustomer = repoCustomer;
            RepoOwner = repoOwner;
            RepoCarModel = repoCarModel;
            RepoApiRent = repoRentApi;
            RepoLog = repoLog;
        }

        [HttpPut]
        public RentResponse Create([FromBody] RentParam param)
        {
            //kamus
            RentResponse result = new RentResponse();
            DisplayFormatHelper dfh = new DisplayFormatHelper();
            List<Guid> idCar = new List<Guid>();
            List<car_model> carModel = new List<car_model>();

            rent dataRent = new rent();
            List<rent> rents = new List<rent>();
            Dictionary<string, Guid> cityMap;
            Guid idOwner = new Guid();

            string code;
            string city;
            Guid customerId;
            List<customer> customerList;

            bool isDataValid = true;

            Business.Infrastructure.FilterInfo filters = new Business.Infrastructure.FilterInfo { Filters = new List<Business.Infrastructure.FilterInfo>(), Logic = "and" };

            DateTime dtStart, dtFinish;
            DateTimeOffset dtoStart, dtoFinish;
            DateTime dtNull = DateTime.Parse("01/01/0001 00:00:00");

            List<car> ownerCars = new List<car>();  
            List<Guid> rentedCars;

            BookingFormStub bfs;
            rent dbItem;

            ApiRentFormStub afs;
            api_rent apiRent;
            log_api_rent logApiRent;

            //hardcode data
            cityMap = new Dictionary<string, Guid>();
            cityMap.Add("bali", new Guid("9e9af49f-742a-40a2-9338-228e56060442"));
            cityMap.Add("surabaya", new Guid("7a683eb8-656d-4b4c-be71-157dd2328a64"));

            //algoritma
            #region check data
            //mengecek validitas parameter
            if (param.StartRent == null)
            {
                isDataValid = false;

                result.Success = false;
                result.Message = "Parameter StartRent wajib diisi";
            }

            if (isDataValid)
            {
                if (param.FinishRent == null)
                {
                    isDataValid = false;

                    result.Success = false;
                    result.Message = "Parameter FinishRent wajib diisi";
                }
            }

            if (isDataValid)
            {
                if (param.City == "")
                {
                    isDataValid = false;

                    result.Success = false;
                    result.Message = "Parameter City wajib diisi";
                }
            }

            if (isDataValid)
            {
                if (param.CarBrandName == "")
                {
                    isDataValid = false;

                    result.Success = false;
                    result.Message = "Parameter CarBrandName wajib diisi";
                }
            }

            if (isDataValid)
            {
                if (param.CarModelName == "")
                {
                    isDataValid = false;

                    result.Success = false;
                    result.Message = "Parameter CarModelName wajib diisi";
                }
            }

            if (isDataValid)
            {
                if (param.CustomerName == "")
                {
                    isDataValid = false;

                    result.Success = false;
                    result.Message = "Parameter CustomerName wajib diisi";
                }
            }

            if (isDataValid)
            {
                if (param.CustomerPhoneNumber == "")
                {
                    isDataValid = false;

                    result.Success = false;
                    result.Message = "Parameter CustomerPhoneNumber wajib diisi";
                }
            }

            if (isDataValid)
            {
                if (param.PickupLocation == "")
                {
                    isDataValid = false;

                    result.Success = false;
                    result.Message = "Parameter PickupLocation wajib diisi";
                }
            }


            if (isDataValid)
            {
                city = param.City.ToLower();
                if (cityMap.ContainsKey(city))
                {
                    idOwner = cityMap[city];
                }
                else
                {
                    isDataValid = false;
                    result.Message = "Kota yang anda pilih saat ini belum tersedia";
                }
            }

            if (isDataValid)
            {
                //mengambil semua mobil milik owner
                filters.Filters.Add(new Business.Infrastructure.FilterInfo { Field = "id_owner", Operator = "eq", Value = idOwner.ToString() });
                if (param.CarBrandName != null)
                    filters.Filters.Add(new Business.Infrastructure.FilterInfo { Field = "car_model.car_brand.name", Operator = "eq", Value = param.CarBrandName });
                if (param.CarModelName != null)
                    filters.Filters.Add(new Business.Infrastructure.FilterInfo { Field = "car_model.name", Operator = "eq", Value = param.CarModelName });
                ownerCars = RepoCar.FindAll(null, null, null, filters);

                //mengambil booking pada tanggal sesuai request
                dtStart = new DateTime(param.StartRent.Value.Year, param.StartRent.Value.Month, param.StartRent.Value.Day, 0, 0, 0);
                dtStart = DateTime.SpecifyKind(dtStart, DateTimeKind.Local);
                dtoStart = dtStart;

                dtFinish = new DateTime(param.FinishRent.Value.Year, param.FinishRent.Value.Month, param.FinishRent.Value.Day, 23, 59, 59);
                dtFinish = DateTime.SpecifyKind(dtFinish, DateTimeKind.Local);
                dtoFinish = dtFinish;
                rents = RepoRent.FindAll(idOwner, dtStart, dtFinish);

                rents.RemoveAll(m => m.finish_rent == dtoStart);
                rents.RemoveAll(m => m.start_rent == dtoFinish);

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

                if (ownerCars.Count() == 0)
                {
                    isDataValid = false;
                    result.Message = "Mobil tidak tersedia";
                }
            }

            #endregion

            if (isDataValid)//insert booking
            {
                //mengecek ketersediaan mobil
                owner owner = RepoOwner.FindByPk(idOwner);
                code = RepoRent.GenerateRentCode(owner);

                //membuat booking baru              
                customerList = new List<customer>();
                Business.Infrastructure.FilterInfo filterCust = new Business.Infrastructure.FilterInfo { Filters = new List<Business.Infrastructure.FilterInfo>(), Logic = "and" };
                filterCust.Filters.Add(new Business.Infrastructure.FilterInfo { Field = "phone_number", Operator = "eq", Value = param.CustomerPhoneNumber });
                filterCust.Filters.Add(new Business.Infrastructure.FilterInfo { Field = "id_owner", Operator = "eq", Value = idOwner.ToString() });

                customerList = RepoCustomer.FindAll(null, null, null, filterCust);

                if (customerList.Count() > 0)
                {
                    customerId = customerList.FirstOrDefault().id;
                }
                else
                {
                    customer cust = new customer
                    {
                        name = param.CustomerName,
                        id_owner = idOwner,
                        phone_number = param.CustomerPhoneNumber
                    };
                    RepoCustomer.Save(cust);

                    customerId = cust.id;
                }


                //if (tempCustomer.Where(n => n.phone_number == param.CustomerPhoneNumber && n.id_owner != idOwner).Count() > 0)
                //{
                //    CustomerID = new Guid();
                //    customer cust = new customer
                //    {
                //        id = CustomerID,
                //        name = param.CustomerName,
                //        id_owner = idOwner,
                //        phone_number = param.CustomerPhoneNumber
                //    };
                //    RepoCustomer.Save(cust);
                //}

                //membuat BookingFormStub   
                bfs = new BookingFormStub();

                dtStart = new DateTime(param.StartRent.Value.Year, param.StartRent.Value.Month, param.StartRent.Value.Day, param.StartRent.Value.Hour, param.StartRent.Value.Minute, param.StartRent.Value.Second);
                dtStart = DateTime.SpecifyKind(dtStart, DateTimeKind.Local);
                dtoStart = dtStart;

                dtFinish = new DateTime(param.FinishRent.Value.Year, param.FinishRent.Value.Month, param.FinishRent.Value.Day, param.FinishRent.Value.Hour, param.FinishRent.Value.Minute, param.FinishRent.Value.Second);
                dtFinish = DateTime.SpecifyKind(dtFinish, DateTimeKind.Local);
                dtoFinish = dtFinish;

                bfs.Code = code;
                bfs.PhoneNumber = param.CustomerPhoneNumber;
                bfs.IdCustomer = customerId;
                bfs.PickupLocation = param.PickupLocation;
                bfs.IdCarModel = ownerCars.FirstOrDefault().id_car_model;  
                bfs.StartRent = dtoStart;
                bfs.FinishRent = dtoFinish;
                bfs.Price = 0;
                bfs.Status = RentStatus.NEW;
                bfs.IdCarPackage = new Guid("0abd31d3-2857-4311-a468-48538bbd790c");
                bfs.PackagePrice = 0;

                //save
                dbItem = bfs.GetDbObjectOnCreate("aerotrans", idOwner);
                RepoRent.Save(dbItem);
                
                //save to table api_rent
                afs = new ApiRentFormStub();
                afs.SetNewRent(dbItem, "ATS");
                apiRent = afs.GetDbObjectOnCreate("aerotrans");

                RepoApiRent.Save(apiRent);

                result.Success = true;
                result.RentCode = code;
            }

            return result;
        }

        [HttpPost]
        public HttpResponseMessage FindRent([FromBody] GetRentParam param)
        {
            //kamus
            Rent rent = null;
            rent dbRent;
            HttpStatusCode status = HttpStatusCode.OK;
            HttpResponseMessage response;
            string message = null;
            Business.Infrastructure.FilterInfo filters;

            //algoritma
            filters = new Business.Infrastructure.FilterInfo { Field = "code", Operator = "eq", Value = param.RentCode };
            dbRent = RepoRent.Find(filters);

            if (dbRent != null)
            {
                rent = new Rent(dbRent);
            }
            else
            {
                message = "Kode Booking tidak ditemukan";
                status = HttpStatusCode.BadRequest;
            }

            response = Request.CreateResponse(status, rent);
            if (message != null)
                response.Content.Headers.Add("ResponseMessage", message);

            return response;
        }
    }
}
