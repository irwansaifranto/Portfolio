using Business.Entities;
using Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace WebUI.Models.CarExpense
{
    public class CarExpensePresentationStub
    {
        public System.Guid Id { get; set; }
        public System.Guid CarBrandName { get; set; }
        public System.Guid CarModelName { get; set; }
        public System.Guid IdCar { get; set; }
        [DisplayName("Tanggal Pembayaran")]
        public DateTime ExpenseDate { get; set; }
        [DisplayName("Plat Nomor")]
        public string LicensePlate { get; set; }
        [DisplayName("Kategori")]
        public string ExpenseType { get; set; }
        [DisplayName("Lain-lain")]
        public string Other { get; set; }
        [DisplayName("Km")]
        public int? Distance { get; set; }
        [DisplayName("Nilai")]
        public int? ExpenseValue { get; set; }
        public string Attachment { get; set; }
        [DisplayName("Catatan")]
        public string Notes { get; set; }

        public CarExpenseType ExpenseTypeEnum { get; set; }

        public CarExpensePresentationStub() { }

        public CarExpensePresentationStub(car_expense dbItem)
        {
            EnumHelper eh = new EnumHelper();

            this.Id = dbItem.id;
            this.IdCar = dbItem.id_car;
            this.ExpenseDate = dbItem.expense_date;
            this.LicensePlate = dbItem.car.license_plate;
            this.Other = dbItem.other;
            this.Distance = dbItem.distance;
            this.ExpenseValue = dbItem.expense_value;
            this.Attachment = dbItem.attachment;
            this.Notes = dbItem.notes;

            if (dbItem.expense_type != null && dbItem.expense_type != "")
            {
                ExpenseTypeEnum = (CarExpenseType)Enum.Parse(typeof(CarExpenseType), dbItem.expense_type);
                ExpenseType = eh.GetEnumDescription(ExpenseTypeEnum);
            }
        }

        public List<CarExpensePresentationStub> MapList(List<car_expense> dbItems)
        {
            List<CarExpensePresentationStub> retList = new List<CarExpensePresentationStub>();

            foreach (car_expense dbItem in dbItems)
                retList.Add(new CarExpensePresentationStub(dbItem));

            return retList;
        }

    }
}