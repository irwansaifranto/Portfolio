using Business.Abstract;
using Business.Entities;
using Common.Enums;
using SecurityGuard.Interfaces;
using SecurityGuard.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Security;
using WebUI.Mailers;
using WebUI.Models.Booking;
using WebUI.Models.Driver;
using WebUI.Models.Service;
using WebUI.Models.Service.Parameter;

namespace WebUI.Controllers.WebAPI
{
    public class DriverFinishController : ApiController
    {
        private IDriverRepository RepoDriver;
        private IRentRepository RepoRent;

        public DriverFinishController(IDriverRepository repoDriver, IRentRepository repoRent)
        {
            RepoDriver = repoDriver;
            RepoRent = repoRent;
        }

        [HttpPost]
        public HttpResponseMessage Submit([FromBody] DriverRentParam param)
        {
            //kamus         
            BookingPresentationStub result = new BookingPresentationStub();
            rent rent = new rent();
            bool isDatavalid = true;
            IEnumerable<string> headerValues;
            HttpStatusCode httpStatus = HttpStatusCode.InternalServerError;
            string responseMessage = null;
            string username = null;

            // Mengecek parameter
            //jika ID kosong, 403 Forbidden
            if (param.Id == Guid.Empty)
            {
                isDatavalid = false;
                responseMessage = HttpContext.GetGlobalResourceObject("WebServiceMessage", "OrderNullAlert").ToString();
                httpStatus = HttpStatusCode.Forbidden;
            }

            //cek username
            if (isDatavalid)
            {
                if (Request.Headers.TryGetValues("Username", out headerValues))
                {
                    username = headerValues.FirstOrDefault();
                }
                else
                {
                    isDatavalid = false;
                    responseMessage = HttpContext.GetGlobalResourceObject("WebServiceMessage", "UsernameNullAlert").ToString();
                    httpStatus = HttpStatusCode.Forbidden;
                }
            }

            //jika Username tidak sama dg username driver
            if (isDatavalid)
            {
                rent = RepoRent.FindByPk(param.Id);
                if ((rent == null) || (rent.driver == null) || (username != rent.driver.username))
                {
                    isDatavalid = false;
                    responseMessage = HttpContext.GetGlobalResourceObject("WebServiceMessage", "NotAccess").ToString();
                    httpStatus = HttpStatusCode.Forbidden;
                }
            }

            // Pengecekan Status
            //mengambil rent milik idDriver
            if (isDatavalid)
            {
                // untuk merubah status dari NEW ke GO
                if (rent.status == RentStatus.GO.ToString())
                {
                    rent.status = RentStatus.FINISH.ToString();
                    rent.updated_by = rent.driver.username;
                    rent.updated_time = DateTime.Now;
                    RepoRent.Save(rent);

                    httpStatus = HttpStatusCode.OK;
                }
                // untuk mengecek status, jika NEW => 500 internal server error
                else if (rent.status == RentStatus.NEW.ToString())
                {
                    responseMessage = HttpContext.GetGlobalResourceObject("WebServiceMessage", "NewAlert").ToString();
                    httpStatus = HttpStatusCode.InternalServerError;
                }
                // untuk mengecek status, jika FINISH => 500 internal server error
                else if (rent.status == RentStatus.FINISH.ToString())
                {
                    responseMessage = HttpContext.GetGlobalResourceObject("WebServiceMessage", "FinishAlert").ToString();
                    httpStatus = HttpStatusCode.InternalServerError;
                }
                // untuk mengecek status, jika CANCEL => 500 internal server error
                else
                {
                    responseMessage = HttpContext.GetGlobalResourceObject("WebServiceMessage", "CancelAlert").ToString();
                    httpStatus = HttpStatusCode.InternalServerError;
                }
            }

            if (responseMessage != null)
            {
                HttpContext.Current.Response.AppendHeader("ResponseMessage", responseMessage);
            }
            return Request.CreateResponse(httpStatus);

        }
    }
}
