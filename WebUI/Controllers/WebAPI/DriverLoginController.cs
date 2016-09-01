using Business.Abstract;
using Business.Entities;
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
using WebUI.Models.Driver;
using WebUI.Models.Service;
using WebUI.Models.Service.Parameter;

namespace WebUI.Controllers.WebAPI
{
    public class DriverLoginController : ApiController
    {
        private IDriverRepository RepoDriver;
        private IMembershipService membershipService;
        private IAuthenticationService authenticationService;

        public DriverLoginController(IDriverRepository repoDriver)
        {

            this.membershipService = new MembershipService(System.Web.Security.Membership.Provider);
            this.authenticationService = new AuthenticationService(membershipService, new FormsAuthenticationService());
            RepoDriver = repoDriver;
        }

        [HttpPost]
        public HttpResponseMessage Submit([FromBody] DriverLoginParam param)
        {
            //kamus
            Business.Infrastructure.FilterInfo filters = new Business.Infrastructure.FilterInfo { Filters = new List<Business.Infrastructure.FilterInfo>(), Logic = "and" };
            DriverUserModel user = new DriverUserModel();
            List<driver> driverList = new List<driver>();
            driver driver;
            HttpStatusCode httpStatus = HttpStatusCode.Forbidden;
            string responseMessage;
            responseMessage = null;

            //algoritma
            if (param.Username == null || param.Username == "")
            {
                responseMessage = "Username wajib diisi";
            }
            else if (param.Password == null || param.Password == "")
            {
                responseMessage = "Password wajib diisi";
            }
            else if (param.DeviceId == null || param.DeviceId == "")
            {
                responseMessage = "Device ID wajib diisi";
            }
            else //semua terisi
            {
                if (authenticationService.LogOn(param.Username, param.Password, false))
                {
                    filters.Filters.Add(new Business.Infrastructure.FilterInfo { Field = "username", Operator = "eq", Value = param.Username.ToString() });
                    driverList = RepoDriver.FindAll(null, null, null, filters);
                    driver = driverList.FirstOrDefault();

                    if (driver != null)
                    {
                        user.IdDriver = driver.id;
                        user.Username = driver.username;

                        //meng-update driver.device_id
                        driver = RepoDriver.FindByPk(driverList.FirstOrDefault().id);
                        driver.device_id = param.DeviceId;
                        RepoDriver.Save(driver);

                        httpStatus = HttpStatusCode.OK;
                    }

                    authenticationService.LogOff();
                }
                else
                {
                    responseMessage = "Username atau password yang anda masukkan salah";
                }
            }

            if (responseMessage != null)
            {
                HttpContext.Current.Response.AppendHeader("ResponseMessage", responseMessage);
            }
            return Request.CreateResponse(httpStatus, user);
        }
    }
}
