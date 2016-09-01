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
using Common.Enums;
using WebUI.Models.Invoice;
using WebUI.Infrastructure.Concrete;
using Rotativa;
using WebUI.Models.Booking;

namespace WebUI.Controllers
{
    [AuthorizeUser(ModuleName = "Invoice")]
    public class InvoiceController : MyController
    {
		private ILogRepository RepoLog;
        private IInvoiceRepository RepoInvoice;
        private IRentRepository RepoRent;

        public InvoiceController(ILogRepository repoLog, IInvoiceRepository repoInvoice, IRentRepository repoRent)
			: base(repoLog)
        {
            RepoInvoice = repoInvoice;
            RepoRent = repoRent;
        }

        private StatisticPresentationStub GetStatistic() {
            Guid idOwner = (User as CustomPrincipal).IdOwner.Value;

            List<invoice> calculate = RepoInvoice.FindAll().Where(x => x.rent.id_owner == idOwner).ToList();
            return new StatisticPresentationStub(calculate);
        }

        [MvcSiteMapNode(Title = "Invoice", ParentKey = "Dashboard", Key = "IndexInvoice")]
        [SiteMapTitle("Breadcrumb")]
        public ActionResult Index()
        {
            var model = new InvoiceFormStub();
            StatisticPresentationStub sp = new StatisticPresentationStub();
            sp = GetStatistic();

            model.FillStatusOptions();
            ViewBag.ListStatus = model.GetStatusOptions();

            //print
            Guid? printId = null;
            if (TempData["idPrint"] != null)
            {
                printId = TempData["idPrint"] as Nullable<Guid>;
            }
            ViewBag.PrintId = printId;

            return View("Index", sp);
        }

        public string Binding()
        {
            //kamus
            GridRequestParameters param = GridRequestParameters.Current;
            Business.Infrastructure.FilterInfo filters = param.Filters;
            Guid? idOwner = (User as CustomPrincipal).IdOwner;
            int total = 0;
            List<invoice> items = new List<invoice>();

            if (idOwner.HasValue)
            {
                AddOwnerFilter(filters, idOwner.Value);
                
                items = RepoInvoice.FindAll(param.Skip, param.Take, (param.Sortings != null ? param.Sortings.ToList() : null), param.Filters);
                total = RepoInvoice.Count(param.Filters);
            }

            return new JavaScriptSerializer().Serialize(new { total = total, data = new InvoicePresentationStub().MapList(items) });
        }

        #region create & edit

        [MvcSiteMapNode(Title = "Tambah", ParentKey = "IndexInvoice")]
        [SiteMapTitle("Breadcrumb")]
        public ActionResult Create()
        {
            Guid? idOwner = (User as CustomPrincipal).IdOwner;
            List<Business.Entities.rent> listRent = new List<rent>();

            if (idOwner.HasValue)
            {
                listRent = RepoRent.FindAll().Where(x => x.id_owner == idOwner.Value).ToList();
            }
            else
            {
                var wrapper = new HttpContextWrapper(System.Web.HttpContext.Current);
                return this.InvokeHttp404(wrapper, System.Net.HttpStatusCode.Forbidden);
            }

            InvoiceFormStub formStub = new InvoiceFormStub(listRent);
            formStub.Status = InvoiceStatus.UNPAID.ToString();
            
            return View("Form", formStub);
        }

        [HttpPost]
        public ActionResult Create(InvoiceFormStub model, bool print = false)
        {
            if (model.AdditionalItemText != null && model.AdditionalItemText != "")
                model.AdditionalItem = new JavaScriptSerializer().Deserialize<List<InvoiceItemFormStub>>(model.AdditionalItemText);
            //bool isNameExist = RepoCar.Find().Where(p => p.name == model.Name).Count() > 0;

            if (ModelState.IsValid)
            {
                invoice dbItem = model.GetDbObject((User as CustomPrincipal).Identity.Name);

                RepoInvoice.Save(dbItem);

                if (model.AdditionalItem != null && model.AdditionalItem.Count() > 0)
                {
                    foreach (InvoiceItemFormStub single in model.AdditionalItem)
                    {
                        RepoInvoice.SaveItem(single.GetDbObject(dbItem.id));
                    }
                }
                               
                //message
                string template = HttpContext.GetGlobalResourceObject("MyGlobalMessage", "CreateSuccess").ToString();
                this.SetMessage(model.Code, template);

                //print flag
                if (print)
                    TempData["idPrint"] = dbItem.id;

                return RedirectToAction("Index");
            }
            else
            {
                Guid? idOwner = (User as CustomPrincipal).IdOwner;
                List<Business.Entities.rent> listRent = new List<rent>();

                if (idOwner.HasValue)
                {
                    listRent = RepoRent.FindAll().Where(x => x.id_owner == idOwner.Value).ToList();
                }

                model.FillRentOptions(listRent);
                model.FillStatusOptions();

                return View("Form", model);
            }
        }

        [MvcSiteMapNode(Title = "Edit", ParentKey = "IndexInvoice")]
        [SiteMapTitle("Breadcrumb")]
        public ActionResult Edit(Guid id)
        {
            Guid? idOwner = (User as CustomPrincipal).IdOwner;
            List<Business.Entities.rent> listRent = new List<rent>();

            if (idOwner.HasValue)
            {
                listRent = RepoRent.FindAll().Where(x => x.id_owner == idOwner.Value).ToList();
            }
            else
            {
                var wrapper = new HttpContextWrapper(System.Web.HttpContext.Current);
                return this.InvokeHttp404(wrapper, System.Net.HttpStatusCode.Forbidden);
            }

            invoice dbItem = RepoInvoice.FindByPk(id);
            InvoiceFormStub detailStub = new InvoiceFormStub(dbItem, listRent);

            ViewBag.Name = dbItem.code;

            return View("Form", detailStub);
        }

        [HttpPost]
        public ActionResult Edit(InvoiceFormStub model, bool print = false)
        {
            if (model.AdditionalItemText != null && model.AdditionalItemText != "")
                model.AdditionalItem = new JavaScriptSerializer().Deserialize<List<InvoiceItemFormStub>>(model.AdditionalItemText);

            //kamus
            invoice dbItem = RepoInvoice.FindByPk(model.Id);
            List<invoice_item> items;
            InvoiceItemFormStub itemFS;
            List<invoice_item> deleted = new List<invoice_item>();

            //algoritma
            if (ModelState.IsValid)
            {
                //save invoice
                dbItem = model.SetDbObject(dbItem, (User as CustomPrincipal).Identity.Name);
                RepoInvoice.Save(dbItem);

                //save invoice_item
                items = dbItem.invoice_item.ToList();
                foreach (invoice_item item in items)
                {
                    itemFS = model.AdditionalItem.Where(m => m.Id == item.id).FirstOrDefault();
                    if (itemFS != null)
                    {
                        itemFS.SetDbObject(item);
                        RepoInvoice.SaveItem(item);
                    }
                    else
                    {
                        deleted.Add(item);
                    }
                }

                if (model.AdditionalItem != null)
                {
                    foreach (InvoiceItemFormStub single in model.AdditionalItem.Where(m => m.Id == null).ToList()) //new items
                    {
                        RepoInvoice.SaveItem(single.GetDbObject(dbItem.id));
                    }
                }

                foreach (invoice_item single in deleted)
                {
                    RepoInvoice.DeleteItem(single);
                }


                //message
                string template = HttpContext.GetGlobalResourceObject("MyGlobalMessage", "EditSuccess").ToString();
                this.SetMessage(model.Code, template);

                //print flag
                if (print)
                    TempData["idPrint"] = dbItem.id;

                return RedirectToAction("Index");
            }
            else
            {
                Guid? idOwner = (User as CustomPrincipal).IdOwner;
                List<Business.Entities.rent> listRent = new List<rent>();
                if (idOwner.HasValue)
                {
                    listRent = RepoRent.FindAll().Where(x => x.id_owner == idOwner.Value).ToList();
                }

                model.FillRentOptions(listRent);
                model.FillStatusOptions();

                ViewBag.Name = dbItem.code;

                return View("Form", model);
            }
        }

        #endregion

        [MvcSiteMapNode(Title = "Detail", ParentKey = "IndexInvoice")]
        [SiteMapTitle("Breadcrumb")]
        public ActionResult Detail(Guid id)
        {
            //Guid? idOwner = (User as CustomPrincipal).IdOwner;
            //List<Business.Entities.rent> listRent = new List<rent>();
            //if (idOwner.HasValue)
            //{
            //    listRent = RepoRent.FindAll().Where(x => x.id_owner == idOwner.Value).ToList();
            //}

            invoice dbItem = RepoInvoice.FindByPk(id);
            InvoicePresentationStub detailStub = new InvoicePresentationStub(dbItem);

            return View("Detail", detailStub);
        }

        public ActionResult PrintInvoice(Guid id)
        {
            invoice dbItem = RepoInvoice.FindByPk(id);
            List<rent_package> listRentPackage = RepoRent.FindAllRentPackage().Where(n => n.id_rent == dbItem.id_rent).ToList();
            InvoicePresentationStub datas = new InvoicePresentationStub(dbItem, listRentPackage);

            //return new ViewAsPdf(datas)
            //{
            //    //CustomSwitches = "--dpi 100 --minimum-font-size 12",
            //    PageOrientation = Rotativa.Options.Orientation.Portrait,
            //    PageSize = Rotativa.Options.Size.A4,
            //};
            return View(datas);
        }

        public JsonResult GetBooking(Guid id) {
            List<string> result = new List<string>();
            if (id != null) {
                rent r = RepoRent.FindByPk(id);
                result.Add(r.customer.name);
                result.Add(r.start_rent.ToString(new DisplayFormatHelper().FullDateFormat) + " s/d " + r.finish_rent.ToString(new DisplayFormatHelper().FullDateFormat));
                result.Add(r.price.ToString());
                result.Add(r.code);
                
            }
            return Json(result);
        }

        #region private

        private void AddOwnerFilter(Business.Infrastructure.FilterInfo filters, Guid idOwner)
        {
            if (filters == null)
                filters = new Business.Infrastructure.FilterInfo { Filters = new List<Business.Infrastructure.FilterInfo>(), Logic = "and" };

            if (filters.Filters == null)
                filters.Filters = new List<Business.Infrastructure.FilterInfo>();

            if (idOwner != null)
            {
                filters.Filters.Add(new Business.Infrastructure.FilterInfo { Field = "rent.id_owner", Operator = "eq", Value = idOwner.ToString() });
            }
        }

        private void AddOwnerFilterRent(Business.Infrastructure.FilterInfo filters, Guid idOwner)
        {
            if (filters == null)
                filters = new Business.Infrastructure.FilterInfo { Filters = new List<Business.Infrastructure.FilterInfo>(), Logic = "and" };

            if (filters.Filters == null)
                filters.Filters = new List<Business.Infrastructure.FilterInfo>();

            if (idOwner != null)
            {
                filters.Filters.Add(new Business.Infrastructure.FilterInfo { Field = "id_owner", Operator = "eq", Value = idOwner.ToString() });
            }
        }

        #endregion
    }
}

