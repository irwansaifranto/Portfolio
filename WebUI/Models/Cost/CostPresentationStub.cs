using Business.Entities;
using Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUI.Infrastructure;

namespace WebUI.Models.Cost
{
    public class CostPresentationStub
    {
        // Example model value from scaffolder script: 0
        public System.Guid Id { get; set; }

        [DisplayName("Kode Booking")]
        public System.Guid IdRent { get; set; }
        public System.Guid IdCustomer { get; set; }
        public string Code { get; set; }
        public string InvoiceDate { get; set; }
        public string RentCode { get; set; }
        public string CustomerName { get; set; }
        public int Price { get; set; }
        public string Status { get; set; }

        public string Category { get; set; }
        public int Value { get; set; }
        [DisplayName("Keterangan")]
        public string Description { get; set; }

        [DisplayName("Tanggal")]
        public DateTime Date { get; set; }

        public DateTimeOffset CreatedTime { get; set; }

        public System.Guid IdExpense { get; set; }

        public System.Guid IdExpenseItem { get; set; }

        public virtual expense Expense { get; set; }

        //booking
        public string PhoneCustomer { get; set; }
        public string EmailCustomer { get; set; }
        public string CarModel { get; set; }
        public string Destination { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset FinishDate { get; set; }

        // atribut detail
        [DisplayName("Mobil")]
        public string Car { get; set; }

        [DisplayName("Supir")]
        public string Driver { get; set; }

        [DisplayName("Bensin")]
        public string Gas { get; set; }

        [DisplayName("Tol")]
        public string Toll { get; set; }

        [DisplayName("Parkir")]
        public string Parking { get; set; }

        [DisplayName("Biaya Lain")]
        public string Other { get; set; }
        public int ValueVehicle { get; set; }
        public int ValueDriver { get; set; }
        public int ValueGas { get; set; }
        public int ValueToll { get; set; }
        public int ValueParking { get; set; }
        public int ValueOther { get; set; }

        public CostPresentationStub() { }

        public CostPresentationStub(expense dbItem)
        {
            List<expense_item> expenseItemList = dbItem.expense_item.ToList();
            this.Id = dbItem.id;
            this.IdRent = dbItem.id_rent;
            this.IdCustomer = dbItem.rent.id_customer;
            this.RentCode = dbItem.rent.code;
            this.CustomerName = dbItem.rent.customer.name;
            this.CreatedTime = dbItem.created_time;
            this.Date = dbItem.date;

            //booking data
            this.PhoneCustomer = dbItem.rent.customer.phone_number;
            this.EmailCustomer = dbItem.rent.customer.email;
            this.CarModel = dbItem.rent.car_model.name;
            this.Destination = dbItem.rent.pickup_location;
            this.StartDate = dbItem.rent.start_rent;
            this.FinishDate = dbItem.rent.finish_rent;

            //expense item
            foreach (expense_item r in expenseItemList.Where(n => n.id_expense == dbItem.id).ToList())
            {
                ExpenseItemCategory category = (ExpenseItemCategory)Enum.Parse(typeof(Common.Enums.ExpenseItemCategory), r.category);
                // value mobil
                if (category == ExpenseItemCategory.VEHICLE)
                {
                    this.Car = ExpenseItemCategory.VEHICLE.ToString();
                    this.ValueVehicle = expenseItemList.Where(x => x.category == ExpenseItemCategory.VEHICLE.ToString()).Select(x => x.value).FirstOrDefault();
                }

                if (category == ExpenseItemCategory.DRIVER)
                {
                    this.Driver = ExpenseItemCategory.DRIVER.ToString();
                    this.ValueDriver = expenseItemList.Where(x => x.category == ExpenseItemCategory.DRIVER.ToString()).Select(x => x.value).FirstOrDefault();
                }

                if (category == ExpenseItemCategory.GAS)
                {
                    this.Gas = ExpenseItemCategory.GAS.ToString();
                    this.ValueGas = expenseItemList.Where(x => x.category == ExpenseItemCategory.GAS.ToString()).Select(x => x.value).FirstOrDefault();
                }

                if(category == ExpenseItemCategory.TOLL)
                {
                    this.Toll = ExpenseItemCategory.TOLL.ToString();
                    this.ValueToll = expenseItemList.Where(x => x.category == ExpenseItemCategory.TOLL.ToString()).Select(x => x.value).FirstOrDefault();
                }

                if (category == ExpenseItemCategory.PARKING)
                {
                    this.Parking = ExpenseItemCategory.PARKING.ToString();
                    this.ValueParking = expenseItemList.Where(x => x.category == ExpenseItemCategory.PARKING.ToString()).Select(x => x.value).FirstOrDefault();
                }

                if (category == ExpenseItemCategory.OTHER)
                {
                    this.Other = ExpenseItemCategory.OTHER.ToString();
                    this.ValueOther = expenseItemList.Where(x => x.category == ExpenseItemCategory.OTHER.ToString()).Select(x => x.value).FirstOrDefault();
                }

                this.Description = r.description;
                
            }

        }


        public CostPresentationStub(expense_item dbItem)
        {
            ExpenseItemCategory category = (ExpenseItemCategory)Enum.Parse(typeof(Common.Enums.ExpenseItemCategory), dbItem.category);
            this.IdExpenseItem = dbItem.id;
            this.IdExpense = dbItem.id_expense;
            this.Category = new EnumHelper().GetEnumDescription(category);

            this.Value = dbItem.value;
            if (category == ExpenseItemCategory.OTHER)
                this.Description = dbItem.description;

            this.Id = dbItem.expense.id;
            this.IdRent = dbItem.expense.id_rent;
            this.IdCustomer = dbItem.expense.rent.id_customer;
            this.StartDate = dbItem.expense.rent.start_rent;
            this.FinishDate = dbItem.expense.rent.finish_rent;
        }

        public List<CostPresentationStub> MapList(List<expense> dbItems)
        {
            List<CostPresentationStub> retList = new List<CostPresentationStub>();

            foreach (expense dbItem in dbItems)
                retList.Add(new CostPresentationStub(dbItem));

            return retList;
        }

        public List<CostPresentationStub> MapListItem(List<expense_item> dbItems)
        {
            List<CostPresentationStub> retList = new List<CostPresentationStub>();

            foreach (expense_item dbItem in dbItems)
                retList.Add(new CostPresentationStub(dbItem));

            return retList;
        }

        public List<SelectListItem> FillCategoryOption(expense dbItems)
        {
            List<SelectListItem> itemList = new List<SelectListItem>();
            EnumHelper eh = new EnumHelper();
            foreach (ExpenseItemCategory item in eh.EnumToList<ExpenseItemCategory>().ToList())
            {
                int price = dbItems.expense_item.Where(x => x.id_expense == dbItems.id && x.category == item.ToString()).First().value;
                itemList.Add(new SelectListItem { Value = price.ToString(), Text = item.ToString() });
            }
            return itemList;

        }
    }
}