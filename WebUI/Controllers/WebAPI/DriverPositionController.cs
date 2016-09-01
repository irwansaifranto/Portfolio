using Business.Abstract;
using Business.Entities;
using Common.Enums;
using SecurityGuard.Interfaces;
using SecurityGuard.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
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
    public class DriverPositionController : ApiController
    {
        private IRentRepository RepoRent;
        private IDriverRepository RepoDriver;
        private IRentPositionRepository RepoPos;

        public DriverPositionController(IRentRepository repoRent, IDriverRepository repoDriver, IRentPositionRepository repoPos)
        {
            RepoRent = repoRent;
            RepoDriver = repoDriver;
            RepoPos = repoPos;
        }

        [HttpPost]
        public HttpResponseMessage Submit([FromBody] DriverPositionParam param)
        {
            //kamus         
            BookingPresentationStub result = new BookingPresentationStub();
            rent rent = new rent();
            bool isDatavalid = true;
            List<rent> listRent = new List<rent>();
            rent_position rentPos = new rent_position();
            string username = null;

            IEnumerable<string> headerValues = Enumerable.Empty<string>();
            HttpStatusCode httpStatus = HttpStatusCode.InternalServerError;
            string responseMessage = null;
            rent = RepoRent.FindByPk(param.Id);

            //algoritma
            //Mengecek Parameter
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

            // Status Check
            //mengambil rent milik id Driver
            if (isDatavalid)
            {
                // Jika rent status GO => menyimpan kedatabase rent_position
                if (rent.status == RentStatus.GO.ToString())
                {
                    rentPos.id_rent = rent.id;
                    rentPos.longitude = param.Long;
                    rentPos.latitude = param.Lat;
                    rentPos.created_by = rent.driver.username;
                    rentPos.created_time = DateTime.Now;
                    RepoPos.Save(rentPos);
                    httpStatus = HttpStatusCode.OK;
                }
                // Jika rent status NEW => 500 Internal server error
                else if (rent.status == RentStatus.NEW.ToString())
                {
                    responseMessage = HttpContext.GetGlobalResourceObject("WebServiceMessage", "NewAlert").ToString();
                    httpStatus = HttpStatusCode.InternalServerError;
                }
                // Jika rent status FINISH => 500 Internal server error
                else if (rent.status == RentStatus.FINISH.ToString())
                {
                    responseMessage = HttpContext.GetGlobalResourceObject("WebServiceMessage", "FinishAlert").ToString();
                    httpStatus = HttpStatusCode.InternalServerError;
                }
                // Jika rent status CANCEL => 500 Internal Server error
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
