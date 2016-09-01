using Business.Abstract;
using Business.Entities;
using Common.Enums;
using SecurityGuard.Interfaces;
using SecurityGuard.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using System.Web.Security;
using WebUI.Infrastructure;
using WebUI.Mailers;
using WebUI.Models.Booking;
using WebUI.Models.Driver;
using WebUI.Models.Service;
using WebUI.Models.Service.Parameter;

namespace WebUI.Controllers.WebAPI
{
    public class DriverOrderHistoryController : ApiController
    {
        private IRentRepository RepoRent;
        private Business.Entities.Entities context = new Business.Entities.Entities();
         
        public DriverOrderHistoryController(IRentRepository repoRent)
        {
            RepoRent = repoRent;
        }

        [HttpPost]
        public HttpResponseMessage Find([FromBody] GridRequestParameters param)
        {
            //kamus         
            HttpStatusCode httpStatus = HttpStatusCode.OK;
            string responseMessage = null;          
            List<rent> listRent = new List<rent>();
            IEnumerable<string> headerValues;
            Business.Infrastructure.FilterInfo filters = new Business.Infrastructure.FilterInfo { Filters = new List<Business.Infrastructure.FilterInfo>(), Logic = "and" };
            List<BookingPresentationStub> results;
            
            //algoritma
            if (Request.Headers.TryGetValues("Username", out headerValues))
            {
                //mengambil order berdasarkan status = 'finish'
                filters.Filters.Add(new Business.Infrastructure.FilterInfo { Field = "driver.username", Operator = "eq", Value = headerValues.FirstOrDefault() });
                filters.Filters.Add(new Business.Infrastructure.FilterInfo { Field = "status", Operator = "eq", Value = RentStatus.FINISH.ToString() });

                //mengambil order finish sesuai param
                listRent = RepoRent.FindAll(param.Skip, param.Take, (param.Sortings != null ? param.Sortings.ToList() : null), filters);
            }
            else
            {
                httpStatus = HttpStatusCode.Forbidden;
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
