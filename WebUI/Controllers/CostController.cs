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
using WebUI.Models.Cost;
using WebUI.Models.Booking;

namespace WebUI.Controllers
{
    public class CostController : MyController
    {
        private ILogRepository RepoLog;
        private IExpenseRepository RepoExpense;
        private IRentRepository RepoRent;
        private IInvoiceRepository RepoInvoice;

        public CostController(ILogRepository repoLog, IExpenseRepository repoExpense, IRentRepository repoRent, IInvoiceRepository repoInvoice)
            : base(repoLog)
        {
            RepoExpense = repoExpense;
            RepoRent = repoRent;
            RepoInvoice = repoInvoice;
        }


        [MvcSiteMapNode(Title = "Rincian Booking", ParentKey = "Dashboard", Key = "IndexCost")]
        [SiteMapTitle("Breadcrumb")]
        public ActionResult Index()
        {
            var model = new CostFormStub();
            StatisticPresentationStub sp = new StatisticPresentationStub();
            //sp = GetStatistic();

            //print
            Guid? printId = null;
            if (TempData["idPrint"] != null)
            {
                printId = TempData["idPrint"] as Nullable<Guid>;
            }
            ViewBag.PrintId = printId;

            model.FillCategoryOptions();
            ViewBag.ListCategory = model.GetCategoryOptions();


            return View("Index", sp);
        }

        public string Binding()
        {
            //kamus
            GridRequestParameters param = GridRequestParameters.Current;
            Business.Infrastructure.FilterInfo filters = param.Filters;
            Guid? idOwner = (User as CustomPrincipal).IdOwner;
            int total = 0;
            List<expense> items = new List<expense>();

            if (idOwner.HasValue)
            {
                AddOwnerFilter(filters, idOwner.Value);

                items = RepoExpense.FindAll(param.Skip, param.Take, (param.Sortings != null ? param.Sortings.ToList() : null), param.Filters);
                total = RepoExpense.Count(param.Filters);
            }

            return new JavaScriptSerializer().Serialize(new { total = total, data = new CostPresentationStub().MapList(items) });
        }

        public string BindingRent(Guid id, string action)
        {
            GridRequestParameters param = GridRequestParameters.Current;
            Business.Infrastructure.FilterInfo filters = param.Filters;

            Guid? idOwner = (User as CustomPrincipal).IdOwner;
            List<rent> listRent = new List<rent>();
            List<expense> listExpense = new List<expense>();
            List<BookingPresentationStub> result = new List<BookingPresentationStub>();
            Business.Infrastructure.FilterInfo filtersExpense = new Business.Infrastructure.FilterInfo { Filters = new List<Business.Infrastructure.FilterInfo>(), Logic = "and" };
            int total = 0;

            filtersExpense.Filters.Add(new Business.Infrastructure.FilterInfo { Field = "rent.id_owner", Operator = "eq", Value = idOwner.ToString() });
            listExpense = RepoExpense.FindAll().Where(n => n.rent.id_owner == idOwner).ToList();

            if (idOwner.HasValue)
            {
                GridRequestParameters paramRent = new GridRequestParameters();
                paramRent = GridRequestParameters.Current;
                Business.Infrastructure.FilterInfo rentFilters = paramRent.Filters;
                AddOwnerFilterRent(rentFilters, idOwner.Value);

                listRent = RepoRent.FindAll(paramRent.Skip, paramRent.Take, (paramRent.Sortings != null ? paramRent.Sortings.ToList() : null), paramRent.Filters);

                //menghapus booking yang sudah memiliki invoice
                rent idRent;
                foreach (expense r in listExpense)
                {
                    idRent = listRent.Where(n => n.id == r.id_rent).FirstOrDefault();
                    if (idRent != null)
                    {
                        listRent.Remove(idRent);

                        if (action == "Edit" && idRent.id == id)
                        {
                            listRent.Add(idRent);
                        }
                    }
                }

                total = listRent.Count();
            }

            result = new BookingPresentationStub().MapList(listRent);

            return new JavaScriptSerializer().Serialize(new { total = total, data = result });
        }

        public string BindingItem()
        {
            //kamus
            GridRequestParameters param = GridRequestParameters.Current;
            Business.Infrastructure.FilterInfo filters = param.Filters;

            int total = 0;
            //expense expenseSet = new expense();
            //List<expense_item> items = RepoExpense.FindAllItem().Where(x => x.id_expense == expenseSet.id).ToList();
            List<expense_item> items = new List<expense_item>();

            items = RepoExpense.FindAllItem(param.Skip, param.Take, (param.Sortings != null ? param.Sortings.ToList() : null), param.Filters);
            total = RepoExpense.CountItem(param.Filters);

            return new JavaScriptSerializer().Serialize(new { total = total, data = new CostPresentationStub().MapListItem(items) });

        }

        [MvcSiteMapNode(Title = "Tambah", ParentKey = "IndexCost")]
        [SiteMapTitle("Breadcrumb")]
        public ActionResult Create() //get
        {
            Guid? idOwner = (User as CustomPrincipal).IdOwner;
            List<Business.Entities.rent> listRent = new List<rent>();
            Business.Infrastructure.FilterInfo rentFilters = new Business.Infrastructure.FilterInfo { Field = "id_owner", Operator = "eq", Value = idOwner.ToString()};

            if (idOwner.HasValue)
            {
                //listRent = RepoRent.FindAll().Where(x => x.id_owner == idOwner.Value).ToList();
                //rentFilters.Filters.Add(new Business.Infrastructure.FilterInfo { Field = "id_owner", Operator = "eq", Value = idOwner.ToString() });
                listRent = RepoRent.FindAll(null, null, null, rentFilters);
            }
            
            CostFormStub formStub = new CostFormStub(listRent);
            formStub.Date = DateTime.Now;
            return View("Form", formStub);
        }

        [HttpPost]
        public ActionResult Create(CostFormStub model, bool print = false)
        {
            //bool isNameExist = RepoCar.Find().Where(p => p.name == model.Name).Count() > 0;

            if (ModelState.IsValid)
            {
                expense dbItem = new expense();
                expense_item expenseItem;
                dbItem = model.GetDbObject((User as CustomPrincipal).Identity.Name);

                try
                {
                    RepoExpense.Save(dbItem);

                    //save mobil
                    expenseItem = new expense_item { id_expense = dbItem.id, category = ExpenseItemCategory.VEHICLE.ToString(), value = model.ValueVehicle, description = model.Description };
                    RepoExpense.SaveItem(expenseItem);
                    expenseItem = new expense_item { id_expense = dbItem.id, category = ExpenseItemCategory.DRIVER.ToString(), value = model.ValueDriver, description = model.Description };
                    RepoExpense.SaveItem(expenseItem);
                    expenseItem = new expense_item { id_expense = dbItem.id, category = ExpenseItemCategory.GAS.ToString(), value = model.ValueGas, description = model.Description };
                    RepoExpense.SaveItem(expenseItem);
                    expenseItem = new expense_item { id_expense = dbItem.id, category = ExpenseItemCategory.TOLL.ToString(), value = model.ValueToll, description = model.Description };
                    RepoExpense.SaveItem(expenseItem);
                    expenseItem = new expense_item { id_expense = dbItem.id, category = ExpenseItemCategory.PARKING.ToString(), value = model.ValueParking, description = model.Description };
                    RepoExpense.SaveItem(expenseItem);
                    expenseItem = new expense_item { id_expense = dbItem.id, category = ExpenseItemCategory.OTHER.ToString(), value = model.ValueOther, description = model.Description };
                    RepoExpense.SaveItem(expenseItem);
                    //save supir

                    //save ...
                }
                catch (Exception e)
                {
                    Guid? idOwner = (User as CustomPrincipal).IdOwner;
                    List<Business.Entities.rent> listRent = new List<rent>();
                    if (idOwner.HasValue)
                    {
                        listRent = RepoRent.FindAll().Where(x => x.id_owner == idOwner.Value).ToList();
                    }

                    model.FillRentOptions(listRent);
                    model.FillCategoryOptions();
                    return View("Form", model);
                }

                //message
                DisplayFormatHelper dfh = new DisplayFormatHelper();
                string template = HttpContext.GetGlobalResourceObject("MyGlobalMessage", "CreateSuccess").ToString();
                this.SetMessage("Pengeluaran " + model.Date.ToString(dfh.FullDateFormat), template);

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
                model.FillCategoryOptions();
                return View("Form", model);
            }
        }

        [MvcSiteMapNode(Title = "Edit", ParentKey = "IndexCost")]
        [SiteMapTitle("Breadcrumb")]
        public ActionResult Edit(Guid id)
        {
            Guid? idOwner = (User as CustomPrincipal).IdOwner;

            List<Business.Entities.rent> listRent = new List<rent>();
            if (idOwner.HasValue)
            {
                listRent = RepoRent.FindAll().Where(x => x.id_owner == idOwner.Value).ToList();
            }

            expense dbItem = RepoExpense.FindByPk(id);
            //expense_item expenseItem = new expense_item();
            List<expense_item> expenseItemList = dbItem.expense_item.Where(x => x.id_expense == dbItem.id).ToList();

            CostFormStub detailStub = new CostFormStub(dbItem, listRent);

            //mobil
            var listItemMobil = expenseItemList.Where(x => x.category == ExpenseItemCategory.VEHICLE.ToString());
            detailStub.ValueVehicle = listItemMobil.Select(x => x.value).FirstOrDefault();
            //supir
            var listItemSupir = expenseItemList.Where(x => x.category == ExpenseItemCategory.DRIVER.ToString());
            detailStub.ValueDriver = listItemSupir.Select(x => x.value).FirstOrDefault();
            //Bensin
            var listItemBBM = expenseItemList.Where(x => x.category == ExpenseItemCategory.GAS.ToString());
            detailStub.ValueGas = listItemBBM.Select(x => x.value).FirstOrDefault();
            //Tol
            var listItemTol = expenseItemList.Where(x => x.category == ExpenseItemCategory.TOLL.ToString());
            detailStub.ValueToll = listItemTol.Select(x => x.value).FirstOrDefault();
            //Parkir
            var listItemParkir = expenseItemList.Where(x => x.category == ExpenseItemCategory.PARKING.ToString());
            detailStub.ValueParking = listItemParkir.Select(x => x.value).FirstOrDefault();
            //BiayaLain
            var listItemBiaya = expenseItemList.Where(x => x.category == ExpenseItemCategory.OTHER.ToString());
            detailStub.ValueOther = listItemBiaya.Select(x => x.value).FirstOrDefault();
            detailStub.Description = expenseItemList.Select(x => x.description).FirstOrDefault();

            return View("Form", detailStub);
        }

        [HttpPost]
        public ActionResult Edit(CostFormStub model, bool print = false)
        {
            //bool isNameExist = RepoCar.Find().Where(p => p.name == model.Name).Count() > 0;

            if (ModelState.IsValid)
            {
                expense dbItem = RepoExpense.FindByPk(model.Id);
                //List<expense_item> expenseItemList = dbItem.expense_item.ToList();
                List<expense_item> expenseItemList = dbItem.expense_item.Where(n => n.id_expense == dbItem.id).ToList();
                dbItem = model.SetDbObject(dbItem, (User as CustomPrincipal).Identity.Name);

                try
                {
                    RepoExpense.Save(dbItem);
                    foreach (expense_item items in expenseItemList)
                    {
                        if (items.category.Equals(ExpenseItemCategory.VEHICLE.ToString()))
                        {
                            items.value = model.ValueVehicle;
                            RepoExpense.SaveItem(items);
                        }
                        if (items.category.Equals(ExpenseItemCategory.DRIVER.ToString()))
                        {
                            items.value = model.ValueDriver;
                            RepoExpense.SaveItem(items);
                        }
                        if (items.category.Equals(ExpenseItemCategory.GAS.ToString()))
                        {
                            items.value = model.ValueGas;
                            RepoExpense.SaveItem(items);
                        }
                        if (items.category.Equals(ExpenseItemCategory.TOLL.ToString()))
                        {
                            items.value = model.ValueToll;
                            RepoExpense.SaveItem(items);
                        }
                        if (items.category.Equals(ExpenseItemCategory.PARKING.ToString()))
                        {
                            items.value = model.ValueParking;
                            RepoExpense.SaveItem(items);
                        }
                        if (items.category.Equals(ExpenseItemCategory.OTHER.ToString()))
                        {
                            items.value = model.ValueOther;
                            RepoExpense.SaveItem(items);
                        }

                        items.description = model.Description;
                        RepoExpense.SaveItem(items);
                    }

                    //foreach()

                }
                catch (Exception e)
                {
                    Guid idOwner = (User as CustomPrincipal).IdOwner.Value;
                    List<Business.Entities.rent> listRent = RepoRent.FindAll().Where(x => x.id_owner == idOwner).ToList();
                    model.FillRentOptions(listRent);
                    model.FillCategoryOptions();
                    return View("Form", model);
                }

                //message
                DisplayFormatHelper dfh = new DisplayFormatHelper();
                string template = HttpContext.GetGlobalResourceObject("MyGlobalMessage", "EditSuccess").ToString();
                this.SetMessage("Pengeluaran " + model.Date.ToString(dfh.FullDateFormat), template);

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
                model.FillCategoryOptions();
                return View("Form", model);
            }
        }

        public JsonResult GetBooking(Guid id)
        {
            List<string> result = new List<string>();
            if (id != null)
            {
                rent r = RepoRent.FindByPk(id);
                result.Add(r.customer.name);
                result.Add(r.start_rent.ToString(new DisplayFormatHelper().FullDateFormat) + " s/d " + r.finish_rent.ToString(new DisplayFormatHelper().FullDateFormat));
                result.Add(r.price.ToString());
                result.Add(r.code);
            }
            return Json(result);
        }

        //[HttpPost]
        //public JsonResult Delete(Guid id)
        //{
        //    expense dbItem = RepoExpense.FindByPk(id);
        //    List<expense_item> deleteItem = dbItem.expense_item.Where(x => x.id_expense == dbItem.id).ToList();      
        //    ResponseModel response = new ResponseModel(true);
        //    foreach (expense_item items in deleteItem)
        //    {
        //        if (items.category.Equals(ExpenseItemCategory.VEHICLE.ToString()))
        //        {
        //            RepoExpense.DeleteItem(items);
        //        }
        //        if (items.category.Equals(ExpenseItemCategory.DRIVER.ToString()))
        //        {
        //            RepoExpense.DeleteItem(items);
        //        }
        //        if (items.category.Equals(ExpenseItemCategory.GAS.ToString()))
        //        {
        //            RepoExpense.DeleteItem(items);
        //        }
        //        if (items.category.Equals(ExpenseItemCategory.TOLL.ToString()))
        //        {
        //            RepoExpense.DeleteItem(items);
        //        }
        //        if (items.category.Equals(ExpenseItemCategory.PARKING.ToString()))
        //        {
        //            RepoExpense.DeleteItem(items);
        //        }
        //        if (items.category.Equals(ExpenseItemCategory.OTHER.ToString()))
        //        {
        //            RepoExpense.DeleteItem(items);
        //        } 
        //    }
        //    RepoExpense.Delete(dbItem);
        //    string messages;

        //    return Json(response);
        //}

        [MvcSiteMapNode(Title = "Detail", ParentKey = "IndexCost")]
        public ViewResult Detail(Guid id)
        {
            expense dbItem = RepoExpense.FindByPk(id);
            //expense_item expenseItem = new expense_item();
            //List<expense_item> expenseItemList = dbItem.expense_item.Where(x => x.id_expense == dbItem.id).ToList();
            //expense_item dbItem = RepoExpense.FindItemByPk(id);
            CostPresentationStub model = new CostPresentationStub(dbItem);
            return View(model);
        }

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
    }
}
