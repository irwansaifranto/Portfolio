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
using WebUI.Models.Customer;
using Common.Enums;
using WebUI.Infrastructure.Concrete;

namespace WebUI.Controllers
{
    [AuthorizeUser(ModuleName = "Customer")]
    public class CustomerController : MyController
    {
		private ICustomerRepository RepoCustomer;
		private IOwnerRepository RepoOwner;
		private ILogRepository RepoLog;

        public CustomerController(ICustomerRepository repoCustomer, ILogRepository repoLog, IOwnerRepository repoOwner)
			: base(repoLog)
        {
            RepoCustomer = repoCustomer;
			RepoOwner = repoOwner;
        }

		[MvcSiteMapNode(Title = "Tamu", ParentKey = "Dashboard",Key="IndexCustomer")]
		[SiteMapTitle("Breadcrumb")]
        public ActionResult Index()
        {
            EnumHelper enumHelper = new EnumHelper();
            var model = new CustomerFormStub();
            List<SelectListItem> TypeCustomerOptions = new List<SelectListItem>();
            foreach (CustomerType item in enumHelper.EnumToList<CustomerType>().ToList())
                {
                    TypeCustomerOptions.Add(new SelectListItem { Value = item.ToString(), Text = enumHelper.GetEnumDescription(item) });
                }

            ViewBag.ListTypeCustomer= new JavaScriptSerializer().Serialize(TypeCustomerOptions);
            ViewBag.ListName = new JavaScriptSerializer().Serialize(model.NameOptions);

            return View();
        }

        public string Binding()
        {
            //kamus
            GridRequestParameters param = GridRequestParameters.Current;

            //algoritma
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

            List<customer> items = RepoCustomer.FindAll(param.Skip, param.Take, (param.Sortings != null ? param.Sortings.ToList() : null), param.Filters);
            int total = RepoCustomer.Count(param.Filters);

            return new JavaScriptSerializer().Serialize(new { total = total, data = new CustomerPresentationStub().MapList(items) });
        }

		[MvcSiteMapNode(Title = "Tambah", ParentKey = "IndexCustomer")]
		[SiteMapTitle("Breadcrumb")]
        public ActionResult Create()
        {
			List<Business.Entities.owner> listOwner = RepoOwner.FindAll();
			
            CustomerFormStub formStub = new CustomerFormStub(listOwner);
            formStub.FillTypeOptions();
            formStub.FillTitleOptions();
            formStub.IdOwner = GetOwnerId();
            return View("Form", formStub);
        }

        [HttpPost]
        [SiteMapTitle("Breadcrumb")]
        public ActionResult Create(CustomerFormStub model)
        {
            //bool isNameExist = RepoCustomer.Find().Where(p => p.name == model.Name).Count() > 0;

            if (ModelState.IsValid)
            {
                customer dbItem = new customer();
                dbItem = model.GetDbObject(dbItem);

                try
                {
                    RepoCustomer.Save(dbItem);
                }
                catch (Exception e)
                {
                    model.FillOwnerOptions(RepoOwner.FindAll());
                    model.FillTypeOptions();
                    model.FillTitleOptions();

                    return View("Form", model);
                }

                //message
                string template = HttpContext.GetGlobalResourceObject("MyGlobalMessage", "CreateSuccess").ToString();
                this.SetMessage(model.Name, template);

                return RedirectToAction("Index");
            }
            else
            {
                model.FillOwnerOptions(RepoOwner.FindAll());
                model.FillTypeOptions();
                model.FillTitleOptions();
                return View("Form", model);
            }
        }

		[MvcSiteMapNode(Title = "Edit", ParentKey = "IndexCustomer", Key = "EditCustomer", PreservedRouteParameters = "id")]
		[SiteMapTitle("Breadcrumb")]
        public ActionResult Edit(Guid id)
        {
            customer customer = RepoCustomer.FindByPk(id);
			List<Business.Entities.owner> listOwner = RepoOwner.FindAll();
            CustomerFormStub formStub = new CustomerFormStub(customer,listOwner);
            
            formStub.FillTypeOptions();
            formStub.FillTitleOptions();
            ViewBag.name = customer.name;
            return View("Form", formStub);
        }

        [HttpPost]
        [SiteMapTitle("Breadcrumb")]
        public ActionResult Edit(CustomerFormStub model)
        {
            //bool isNameExist = RepoKompetitor.Find().Where(p => p.name == model.Name && p.id != model.Id).Count() > 0;

            if (ModelState.IsValid)
            {
                customer dbItem = RepoCustomer.FindByPk(model.Id);
                dbItem = model.GetDbObject(dbItem);

                try
                {
                    RepoCustomer.Save(dbItem);
                }
                catch (Exception e)
                {
                    model.FillOwnerOptions(RepoOwner.FindAll());
                    model.FillTypeOptions();
                    model.FillTitleOptions();
                    return View("Form", model);
                }

                //message
                string template = HttpContext.GetGlobalResourceObject("MyGlobalMessage", "CreateSuccess").ToString();
                this.SetMessage(model.Name, template);

                return RedirectToAction("Index");
            }
            else
            {
                customer customer = RepoCustomer.FindByPk(model.Id);
                ViewBag.name = customer.name;
                model.FillOwnerOptions(RepoOwner.FindAll());
                model.FillTypeOptions();
                model.FillTitleOptions();
                return View("Form", model);
            }
        }

        private Guid GetOwnerId()
        {
            //replace 
           Guid id = (User as CustomPrincipal).IdOwner.Value;

            return id;
        }


	}
}

