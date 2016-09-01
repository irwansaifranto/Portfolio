using Business.Abstract;
using Business.Entities;
using Common.Enums;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WebUI.Models.Service.Parameter;

namespace WebUI.Controllers.WebAPI
{
    public class ConfirmRentController : ApiController
    {
        private IApiRentRepository RepoApiRent;
        private IRentRepository RepoRent;

        public ConfirmRentController(IApiRentRepository repoApiRent, IRentRepository repoRent)
        {
            RepoApiRent = repoApiRent;
            RepoRent = repoRent;
        }

        [HttpPut]
        public HttpResponseMessage Submit([FromBody] GetRentParam param)
        {
            //kamus
            HttpResponseMessage response;
            HttpStatusCode status = HttpStatusCode.OK;
            Business.Infrastructure.FilterInfo filters;
            rent rent;
            api_rent apiRent;
            ApiRentStatus apiStatus;
            string message = null;

            //algoritma
            filters = new Business.Infrastructure.FilterInfo { Field = "code", Operator = "eq", Value = param.RentCode };
            rent = RepoRent.Find(filters);

            if (rent == null)
            {
                status = HttpStatusCode.BadRequest;
                message = WebServiceMessage.OrderNotFoundAlert;
            }
            else
            {
                filters = new Business.Infrastructure.FilterInfo { Field = "id_rent", Operator = "eq", Value = rent.id.ToString() };
                apiRent = RepoApiRent.Find(filters);

                if (apiRent == null)
                {
                    status = HttpStatusCode.BadRequest;
                    message = WebServiceMessage.OrderNotFoundAlert;
                }
                else
                {
                    apiStatus = (ApiRentStatus)Enum.Parse(typeof(ApiRentStatus), apiRent.status);

                    if (apiRent.cancellation_status != null)
                    {
                        status = HttpStatusCode.BadRequest;
                        message = WebServiceMessage.CancelAlert;
                    }
                    else if (apiStatus == ApiRentStatus.CONFIRM)
                    {
                        status = HttpStatusCode.BadRequest;
                        message = WebServiceMessage.ConfirmAlert;
                    }
                    else
                    {
                        apiRent.status = ApiRentStatus.CONFIRM.ToString();
                        RepoApiRent.Save(apiRent);
                    }
                }
            }

            response = Request.CreateResponse(status);
            response.Headers.Add("ResponseMessage", message);

            return response;
        }

    }
}