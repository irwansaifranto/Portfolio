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
using WebUI.Models.Owner;
using SecurityGuard.Interfaces;
using SecurityGuard.Services;
using System.Web.Security;

namespace WebUI.Controllers
{
    [AuthorizeUser(ModuleName = "Owner")]
    public class OwnerController : MyController
    {
		private IOwnerRepository RepoOwner;
        private IUserRepository RepoUser;
		private ILogRepository RepoLog;
        private ICityRepository RepoCity;

        public OwnerController(IOwnerRepository repoOwner, ILogRepository repoLog, IUserRepository repoUser, ICityRepository repoCity)
			: base(repoLog)
        {
            RepoOwner = repoOwner;
            RepoUser = repoUser;
            RepoCity = repoCity;
        }

		[MvcSiteMapNode(Title = "Owner", ParentKey = "Dashboard",Key="IndexOwner")]
		[SiteMapTitle("Breadcrumb")]
        public ActionResult Index()
        {
            return View();
        }

        public string Binding()
        {
            //kamus
            GridRequestParameters param = GridRequestParameters.Current;

            List<owner> items = RepoOwner.FindAll(param.Skip, param.Take, (param.Sortings != null ? param.Sortings.ToList() : null), param.Filters);
            int total = RepoOwner.Count(param.Filters);

            return new JavaScriptSerializer().Serialize(new { total = total, data = new OwnerPresentationStub().MapList(items) });
        }



		[MvcSiteMapNode(Title = "Create", ParentKey = "IndexOwner")]
		[SiteMapTitle("Breadcrumb")]
        public ActionResult Create()
        {
			
            OwnerFormStub formStub = new OwnerFormStub();

            return View("Form", formStub);
        }

        [HttpPost]
		[SiteMapTitle("Breadcrumb")]
        public ActionResult Create(OwnerFormStub model)
        {
            var currentDate = DateTime.Now;
            model.CreatedBy = User.Identity.Name;
            model.CreatedTime = currentDate;
            model.UpdatedBy = User.Identity.Name;
            model.UpdatedTime = currentDate;

            bool checkUniqueness = RepoOwner.CheckCodeUniqueness(model.Code);
            if (checkUniqueness == false)
            {
                ModelState.AddModelError("Code", "Code sudah pernah digunakan sebelumnya");
            }

            //Entah kenapa validation failed terus klo ga di buat seperti ini
            ModelState.Remove("CreatedBy");
            ModelState.Remove("UpdatedBy");

            if (ModelState.IsValid)
            {             
                owner dbItem = new owner();
                dbItem = model.GetDbObject();

                RepoOwner.Save(dbItem);

                //message
                string template = HttpContext.GetGlobalResourceObject("MyGlobalMessage", "CreateSuccess").ToString();
                this.SetMessage(model.Name, template);

                return RedirectToAction("Index");
            }
            else
            {
                //var errors = ModelState.Select(x => x.Value.Errors)
                //           .Where(y => y.Count > 0)
                //           .ToList();
                return View("Form", model);
            }
        }

		[MvcSiteMapNode(Title = "Edit", ParentKey = "IndexOwner", Key = "EditOwner", PreservedRouteParameters = "id")]
		[SiteMapTitle("Breadcrumb")]
        public ActionResult Edit(Guid id)
        {
            owner owner = RepoOwner.FindByPk(id);
            OwnerFormStub formStub = new OwnerFormStub(owner);
            ViewBag.name = owner.name;
            return View("Form", formStub);
        }

        [HttpPost]
		[SiteMapTitle("Breadcrumb")]
        public ActionResult Edit(OwnerFormStub model)
        {
            //bool isNameExist = RepoKompetitor.Find().Where(p => p.name == model.Name && p.id != model.Id).Count() > 0;
            var currentDate = DateTime.Now;
            model.UpdatedBy = User.Identity.Name;
            model.UpdatedTime = currentDate;

            //Entah kenapa validation failed terus klo ga di buat seperti ini
            ModelState.Remove("CreatedBy");
            ModelState.Remove("UpdatedBy");
            if (ModelState.IsValid)
            {
                owner dbItem = RepoOwner.FindByPk(model.Id);
                if(dbItem.code != model.Code)
                {
                    var checkUniqueness = RepoOwner.CheckCodeUniqueness(model.Code);
                    if (checkUniqueness == false)
                    {
                        ModelState.AddModelError("Code", "Code sudah pernah digunakan sebelumnya");
                        ViewBag.name = dbItem.name;
                        return View("Form", model);
                    }
                }
                model.UpdateDbObject(dbItem);

                //Remove this line when User Identity Name is working
                if(dbItem.created_by == null)
                {
                    dbItem.created_by = "";
                }

                try
                {
                    RepoOwner.Save(dbItem);
                }
                catch (Exception e)
                { 
                    return View("Form", model);
                }

                //message
                string template = HttpContext.GetGlobalResourceObject("MyGlobalMessage", "CreateSuccess").ToString();
                this.SetMessage(model.Name, template);

                return RedirectToAction("Index");
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList();

                owner owner = RepoOwner.FindByPk(model.Id);
                ViewBag.name = owner.name;
                return View("Form", model);
            }
        }

        //[HttpPost]
        //public JsonResult Delete(int id)
        //{
        //    string template = "";
        //    ResponseModel response = new ResponseModel(true);
        //    owner dbItem = RepoOwner.FindByPk(id);

        //    RepoOwner.Delete(dbItem);

        //    return Json(response);
        //}

        #region owner user

        public string BindingUser()
        {
            //kamus
            GridRequestParameters param = GridRequestParameters.Current;

            List<owner_user> items = RepoOwner.FindAllUser(param.Skip, param.Take, (param.Sortings != null ? param.Sortings.ToList() : null), param.Filters);
            int total = RepoOwner.CountUser(param.Filters);

            return new JavaScriptSerializer().Serialize(new { total = total, data = new OwnerPresentationStub().MapListUser(items) });
        }

        [MvcSiteMapNode(Title = "Assign User", ParentKey = "IndexOwner")]
        [SiteMapTitle("Breadcrumb")]
        public ActionResult AssignUser()
        {

            OwnerUserFormStub formStub = new OwnerUserFormStub();

            return View("FormAssign", formStub);
        }

        [HttpPost]
        [SiteMapTitle("Breadcrumb")]
        public ActionResult AssignUser(OwnerUserFormStub model)
        {
            //bool isNameExist = RepoOwner.Find().Where(p => p.name == model.Name).Count() > 0;

            if (ModelState.IsValid)
            {
                var checkUniqueness = RepoOwner.CheckUsername(model.Username,model.IdOwner);
                if (checkUniqueness == false)
                {
                    ModelState.AddModelError("Username", "Username sudah pernah di assign pada perusahaan ini sebelumnya");
                    return View("FormAssign", model);
                }

                owner_user dbItem = new owner_user();
                dbItem = model.GetDbObject(dbItem);

                owner_user savedData = RepoOwner.SaveUser(dbItem);

                owner ownerData = RepoOwner.FindOwnerByUserName(savedData.username);

                //message
                //string template = HttpContext.GetGlobalResourceObject("MyGlobalMessage", "CreateSuccess").ToString();
                this.SetMessage(savedData.username+ " berhasil di assign ke "+ownerData.name);

                return RedirectToAction("Index");
            }
            else
            {
                //var errors = ModelState.Select(x => x.Value.Errors)
                //           .Where(y => y.Count > 0)
                //           .ToList();
                return View("FormAssign", model);
            }
        }

        public string BindingOwner()
        {
            List<owner> items = RepoOwner.FindAll(null, null, null, null);
            int total = items.Count();

            return new JavaScriptSerializer().Serialize(new { total = total, data = new OwnerPresentationStub().MapList(items).OrderBy(x => x.Name) });
        }

        public string BindingCity()
        {
            List<city> items = RepoCity.FindAll(null, null, null, null);
            int total = items.Count();

            return new JavaScriptSerializer().Serialize(new { total = total, data = new OwnerPresentationStub().MapListCity(items).OrderBy(x => x.CityName) });
        }

        public string BindingUsername()
        {
            IMembershipService membershipService = new MembershipService(System.Web.Security.Membership.Provider);
            
            int total;
            //membershipService.
            MembershipUserCollection items = membershipService.GetAllUsers(0, RepoUser.Count(), out total);

            return new JavaScriptSerializer().Serialize(new { total = total, data = new OwnerPresentationStub().MapListUsername(items).OrderBy(x => x.Username) });
        }

        [HttpPost]
        public JsonResult DeleteUser(Guid id)
        {
            ResponseModel response = new ResponseModel(true);
            owner_user dbItem = RepoOwner.FindByPkUser(id);

            RepoOwner.DeleteUser(dbItem);

            return Json(response);
        }

        #endregion

    }
}

