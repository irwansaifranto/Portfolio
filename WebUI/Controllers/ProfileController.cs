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
using WebUI.Models.Car;
using WebUI.Models.CarBrand;
using WebUI.Models.CarModel;
using WebUI.Infrastructure.Concrete;
using WebUI.Models.Profile;


namespace WebUI.Controllers
{
    // [AuthorizeUser(ModuleName = "Owner")]
    public class ProfileController : MyController
    {
        private IOwnerRepository RepoOwner;
        private IOwnerRepository RepoOwnerModel;
      
        private ILogRepository RepoLog;
       
        private Guid id;

        public ProfileController(IOwnerRepository repoOwner, ILogRepository repoLog)
            : base(repoLog)
        {
            RepoOwner = repoOwner;
          
        }


        public ActionResult Index()
        {
            var model = new ProfileFormStub();

          

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
            
          
            List<owner> items = RepoOwner.FindAll(param.Skip, param.Take, (param.Sortings != null ? param.Sortings.ToList() : null), param.Filters);
            int total = RepoOwner.Count(param.Filters);

            return new JavaScriptSerializer().Serialize(new { total = total, data = new ProfilePresentationStub().MapList(items) });
        }

        //[MvcSiteMapNode(Title = "Mobil", ParentKey = "Dashboard",Key="IndexCar")]
        //[SiteMapTitle("Breadcrumb")]
        public ActionResult Update()
        {
          
            Guid idOwner = (User as CustomPrincipal).IdOwner.Value;// Ngambil nilai idOwner dari session
            owner owner = RepoOwner.FindByPk(idOwner); // mengambil nilai owner dari repository yang idowner nya bernilai sama
            ProfileFormStub formStub = new ProfileFormStub(owner); // memasukan data owner dari repository ke formstub 
            ViewBag.name = owner.name;
            return View(formStub);          
        }

        [HttpPost]
        public ActionResult Update(ProfileFormStub model)
        {

            if (ModelState.IsValid)
            {
                owner dbItem = RepoOwner.FindByPk(model.Id);
                Guid idOwner = (User as CustomPrincipal).IdOwner.Value;
                dbItem = model.GetProfile(dbItem, idOwner);
                dbItem.updated_by = (User as CustomPrincipal).Identity.Name;
                dbItem.updated_time = DateTimeOffset.Now;

                try
                {
                    
                    RepoOwner.Save(dbItem);
                }
                catch (Exception e)
                { 
				//model.FillCarModelOptions(RepoCarModel.FindAll());
                    return View("");
                }

                //message
                string template = HttpContext.GetGlobalResourceObject("MyGlobalMessage", "EditSuccess").ToString();
                this.SetMessage("Profil", template);

            
                return RedirectToAction("Update");
            }
            else
            {
                owner owner = RepoOwner.FindByPk(model.Id);
                ViewBag.name = owner.logo;
				//model.FillCarModelOptions(RepoCarModel.FindAll());
                return View("Update", model);
          
        }

       
	}
  }
}

