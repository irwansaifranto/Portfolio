using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebUI.Models;
using WebUI.Controllers;
using WebUI.Infrastructure;
using WebUI.Extension;
using MvcSiteMapProvider;
using MvcSiteMapProvider.Web.Mvc.Filters;
using Business.Abstract;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System.IO;
using System.Threading;
using Business.Entities;
using WebUI.Models.Driver;
using Common.Enums;
using WebUI.Infrastructure.Concrete;
using SecurityGuard.ViewModels;
using System.Web.Security;
using System.Web.Routing;

namespace WebUI.Controllers
{
    [AuthorizeUser(ModuleName = "Driver")]
    public class DriverController : MyController
    {
		private IDriverRepository RepoDriver;
		private IOwnerRepository RepoOwner;
		private ILogRepository RepoLog;

        public DriverController(IDriverRepository repoDriver, ILogRepository repoLog, IOwnerRepository repoOwner)
			: base(repoLog)
        {
            RepoDriver = repoDriver;
			RepoOwner = repoOwner;
        }

		[MvcSiteMapNode(Title = "Supir", ParentKey = "Dashboard", Key="IndexDriver")]
		[SiteMapTitle("Breadcrumb")]
        public ActionResult Index()
        {
            EnumHelper enumHelper = new EnumHelper();
            var model = new DriverFormStub();
        
            List<SelectListItem> TypeDriverOptions = new List<SelectListItem>();
            foreach (DriverType item in enumHelper.EnumToList<DriverType>().ToList())
            {
                TypeDriverOptions.Add(new SelectListItem { Value = item.ToString(), Text = enumHelper.GetEnumDescription(item) });
            }

            ViewBag.ListTypeDriver = new JavaScriptSerializer().Serialize(TypeDriverOptions);            
            return View();
           
        }

        public string Binding()
        {
            //kamus
            GridRequestParameters param = GridRequestParameters.Current;
            
            if (param.Filters.Filters == null)
            {
                param.Filters.Filters = new List<Business.Infrastructure.FilterInfo>();
                param.Filters.Logic = "and";
            }
            

            param.Filters.Filters.Add(new Business.Infrastructure.FilterInfo
            {
                Field = "id_owner",
                Operator = "eq",
                Value = GetOwnerId().ToString()
            });

            List<driver> items = RepoDriver.FindAll(param.Skip, param.Take, (param.Sortings != null ? param.Sortings.ToList() : null), param.Filters);
            int total = RepoDriver.Count(param.Filters);

            return new JavaScriptSerializer().Serialize(new { total = total, data = new DriverPresentationStub().MapList(items) });
           
        }

		[MvcSiteMapNode(Title = "Tambah", ParentKey = "IndexDriver")]
		[SiteMapTitle("Breadcrumb")]
        public ActionResult Create()
        {
            //kamus            
            DriverFormStub formStub = new DriverFormStub();

            //algoritma
            formStub.IdOwner = GetOwnerId();           

            SetModelOptionsOnCreate(formStub);

            return View("Form", formStub);
           
        }

        [HttpPost]
		[SiteMapTitle("Breadcrumb")]
        public ActionResult Create(DriverFormStub model)
        {
            Business.Infrastructure.FilterInfo filters = new Business.Infrastructure.FilterInfo { Filters = new List<Business.Infrastructure.FilterInfo>(), Logic = "and" };
            
            if (ModelState.IsValid)
            {               
                driver dbItem = new driver();
                dbItem = model.GetDbObject(dbItem);

                try
                {
                    RepoDriver.Save(dbItem);
                }
                catch (Exception e)
                {
                    SetModelOptionsOnCreate(model);

                    return View("Form", model);
                }
                
                //message
                string template = HttpContext.GetGlobalResourceObject("MyGlobalMessage", "CreateSuccess").ToString();
                this.SetMessage(model.Name, template);

                return RedirectToAction("Index");
            }
            else
            {
                //model.FillOwnerOptions(RepoOwner.FindAll());
                SetModelOptionsOnCreate(model);

                return View("Form", model);
            }
        }

		[MvcSiteMapNode(Title = "Edit", ParentKey = "IndexDriver", Key = "EditDriver", PreservedRouteParameters = "id")]
		[SiteMapTitle("Breadcrumb")]
        public ActionResult Edit(Guid id)
        {
            //kamus
            driver driver = RepoDriver.FindByPk(id);
			List<Business.Entities.owner> listOwner = RepoOwner.FindAll();
            List<Business.Entities.owner_user> listUser = RepoOwner.FindAllUser();
            DriverFormStub formStub = new DriverFormStub(driver,listOwner,listUser);

            //algoritma
            SetModelOptionsOnEdit(formStub);

            ViewBag.name = driver.name;
            
            return View("Form", formStub);
        }

        [HttpPost]
        [SiteMapTitle("Breadcrumb")]
        public ActionResult Edit(DriverFormStub model)
        {
            //bool isNameExist = RepoKompetitor.Find().Where(p => p.name == model.Name && p.id != model.Id).Count() > 0;
            Business.Infrastructure.FilterInfo filters = new Business.Infrastructure.FilterInfo { Filters = new List<Business.Infrastructure.FilterInfo>(), Logic = "and" };
            List<owner_user> ownerUser;
            List<driver> ownerDriverList; //driver punya owner
            List<string> driverUsernameList; //username milik driver
            if (ModelState.IsValid)
            {
                driver dbItem = RepoDriver.FindByPk(model.Id);
                dbItem = model.GetDbObject(dbItem);

                try
                {
                    RepoDriver.Save(dbItem);
                }
                catch (Exception e)
                {
                    model.FillOwnerOptions(RepoOwner.FindAll());
                    model.FillTypeOptions();
                    model.FillUsernameOptions(RepoOwner.FindAllUser());
                    return View("Form", model);
                }

                //message
                string template = HttpContext.GetGlobalResourceObject("MyGlobalMessage", "CreateSuccess").ToString();
                this.SetMessage(model.Name, template);

                return RedirectToAction("Index");
            }
            else
            {
                SetModelOptionsOnEdit(model);

                return View("Form", model);
            }
        }

        [MvcSiteMapNode(Title = "ChangePassword", ParentKey = "IndexDriver", Key = "ChangePassword", PreservedRouteParameters = "id")]
        [SiteMapTitle("Breadcrumb")]
        public virtual ActionResult ChangePassword(Guid id)
        {
            DriverPasswordFormStub viewModel = new DriverPasswordFormStub();
            driver driver = RepoDriver.FindByPk(id);

            ViewBag.Username = driver.username;

            return View("ChangePasswordForm", viewModel);
        }
        
        [HttpPost]
        [SiteMapTitle("Breadcrumb")]
        public virtual ActionResult ChangePassword(DriverPasswordFormStub model)
        {
            driver driver = RepoDriver.FindByPk(model.Id);

            if (ModelState.IsValid)
            {
                bool changePasswordSucceeded;
                
                try
                {
                    MembershipUser currentUser = System.Web.Security.Membership.GetUser(driver.username);
                    changePasswordSucceeded = currentUser.ChangePassword(currentUser.ResetPassword(), model.NewPassword);                   
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    string template = HttpContext.GetGlobalResourceObject("MyGlobalMessage", "ChangePassword").ToString();
                    this.SetMessage(template);
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            ViewBag.Username = driver.username;

            return View("ChangePasswordForm");            
        }

        [MvcSiteMapNode(Title = "Detail", ParentKey = "IndexDriver", Key = "DetailDriver")]
        public ViewResult Detail(Guid id)
        {
            driver dbItem = RepoDriver.FindByPk(id);
            DriverPresentationStub model = new DriverPresentationStub(dbItem);

            return View(model);
        }

        #region private

        private Guid GetOwnerId()
        {
            //replace 
            Guid id = (User as CustomPrincipal).IdOwner.Value;

            return id;
        }

        private void SetModelOptionsOnCreate(DriverFormStub formStub)
        {
            //kamus
            Business.Infrastructure.FilterInfo filters = new Business.Infrastructure.FilterInfo { Filters = new List<Business.Infrastructure.FilterInfo>(), Logic = "and" };
            List<owner_user> ownerUser;
            List<driver> ownerDriverList; //driver punya owner
            List<string> driverUsernameList; //username milik driver

            //algoritma
            formStub.FillTypeOptions();

            //mengisi pilihan username            
            filters.Filters.Add(new Business.Infrastructure.FilterInfo { Field = "id_owner", Operator = "eq", Value = formStub.IdOwner.ToString() });
            ownerUser = RepoOwner.FindAllUser(null, null, null, filters);
            ownerDriverList = RepoDriver.FindAll(null, null, null, filters);
            driverUsernameList = ownerDriverList.Select(m => m.username).ToList();

            //menghapus data dari ownerUser yang username nya ada di driverUsernameList
            ownerUser.RemoveAll(m => driverUsernameList.Contains(m.username));
            formStub.FillUsernameOptions(ownerUser);
        }

        private void SetModelOptionsOnEdit(DriverFormStub model)
        {
            //kamus
            Business.Infrastructure.FilterInfo filters = new Business.Infrastructure.FilterInfo { Filters = new List<Business.Infrastructure.FilterInfo>(), Logic = "and" };
            List<owner_user> ownerUser;
            List<driver> ownerDriverList; //driver punya owner
            List<string> driverUsernameList; //username milik driver

            //algoritma
            filters.Filters.Add(new Business.Infrastructure.FilterInfo { Field = "id_owner", Operator = "eq", Value = model.IdOwner.ToString() });
            ownerUser = RepoOwner.FindAllUser(null, null, null, filters);
            ownerDriverList = RepoDriver.FindAll(null, null, null, filters);

            driverUsernameList = ownerDriverList.Select(m => m.username).ToList();

            //hapus di driverUsernameList yang isinya sama dengan Username yang di edit  
            driverUsernameList.Remove(model.Username);
            ownerUser.RemoveAll(m => driverUsernameList.Contains(m.username));
            model.FillUsernameOptions(ownerUser);

            model.FillTypeOptions();
        }

        #endregion

        //[HttpPost]
        //public JsonResult Delete(int id)
        //{
        //    string template = "";
        //    ResponseModel response = new ResponseModel(true);
        //    driver dbItem = RepoDriver.FindByPk(id);

        //    RepoDriver.Delete(dbItem);

        //    return Json(response);
        //}

	}
}

