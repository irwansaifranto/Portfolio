using Business.Abstract;
using Business.Entities;
using SecurityGuard.Interfaces;
using SecurityGuard.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Security;

using WebUI.Models.Booking;
using WebUI.Models.Service;
using WebUI.Models.Service.Parameter;

namespace WebUI.Controllers.WebAPI
{
    public class DriverActiveRentController : ApiController
    {
        //
        private IRentRepository RepoRent;
        private IDriverRepository RepoDriver;
        
        public DriverActiveRentController(IRentRepository repoRent, IDriverRepository repoDriver)
        {
            RepoRent = repoRent;
            RepoDriver = repoDriver;
        }

        [HttpPost]
        public HttpResponseMessage GetActiveRent([FromBody] List<DriverActiveRentParam> param)
        {
            //kamus
            List<BookingPresentationStub> results;
            List<rent> listRent = new List<rent>();            
            rent rent;
            driver driver;
            Guid idDriver;

            DateTime dtUpdated;
            DateTimeOffset dtoUpdated;

            IEnumerable<string> headerValues;
            Business.Infrastructure.FilterInfo filters = new Business.Infrastructure.FilterInfo { Filters = new List<Business.Infrastructure.FilterInfo>(), Logic = "and" };
            
            HttpStatusCode httpStatus = HttpStatusCode.Forbidden;
            string responseMessage = null;

            //algoritma
            if (Request.Headers.TryGetValues("Username", out headerValues))
            {
                //mengambil semua rent milik idDriver dengan status NEW / GO
                filters.Filters.Add(new Business.Infrastructure.FilterInfo { Field = "username", Operator = "eq", Value = headerValues.FirstOrDefault() });
                driver = RepoDriver.FindAll(null, null, null, filters).FirstOrDefault();
                if (driver != null)
                {
                    idDriver = driver.id;

                    filters = new Business.Infrastructure.FilterInfo
                    {
                        Filters = new List<Business.Infrastructure.FilterInfo>
                        {
                            new Business.Infrastructure.FilterInfo 
                            { 
                                Field = "id_driver", Operator = "eq", Value = idDriver.ToString() 
                            },
                            new Business.Infrastructure.FilterInfo
                            {
                                Filters = new List<Business.Infrastructure.FilterInfo>
                                {
                                    new Business.Infrastructure.FilterInfo { Field = "status", Operator = "eq", Value = Common.Enums.RentStatus.GO.ToString() },
                                    new Business.Infrastructure.FilterInfo { Field = "status", Operator = "eq", Value = Common.Enums.RentStatus.NEW.ToString() },
                                },
                                Logic = "or"
                            },
                        },
                        Logic = "and"
                    };

                    listRent = RepoRent.FindAll(null, null, null, filters);

                    //menghapus data dari list rent yang <= param.update_time
                    if (param != null)
                    {
                        foreach (DriverActiveRentParam single in param)
                        {
                            //mengambil data yang bersesuaian di listRent
                            rent = listRent.Where(m => m.id == single.Id).FirstOrDefault();

                            //if data di listRent lebih lama (<=), dihapus dari listRent
                            if (rent != null)
                            {
                                if (rent.updated_time.Value.UtcDateTime <= single.UpdatedTimeUtc)
                                    listRent.Remove(rent);
                            }
                            else //rent tidak ditemukan di listRent
                            {
                                rent = RepoRent.FindByPk(single.Id);
                                if (rent != null)
                                {
                                    if (rent.updated_time.Value.UtcDateTime > single.UpdatedTimeUtc)
                                        listRent.Add(rent);
                                }
                            }
                        }
                    }

                    httpStatus = HttpStatusCode.OK;
                }
                else
                {
                    responseMessage = "Username Anda tidak ditemukan.";
                }
            }
            else
            {
                responseMessage = "Username Anda tidak ditemukan.";
            }


            //result
            results = new BookingPresentationStub().MapList(listRent);

            if (responseMessage != null)
            {
                HttpContext.Current.Response.AppendHeader("ResponseMessage", responseMessage);
            }
            return Request.CreateResponse(httpStatus, results);
        }
    }
}
