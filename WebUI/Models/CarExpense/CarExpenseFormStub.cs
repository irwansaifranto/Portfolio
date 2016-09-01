using Business.Entities;
using Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUI.Infrastructure;
using WebUI.Infrastructure.Validation;

namespace WebUI.Models.CarExpense
{
    public class CarExpenseFormStub
    {
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public System.Guid Id { get; set; }

        [DisplayName("Tanggal Pembayaran")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public DateTime ExpenseDate { get; set; }
        public System.Guid IdCar { get; set; }
        public System.Guid CarBrandName { get; set; }
        public System.Guid CarModelName { get; set; }

        [DisplayName("Kategori")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public string ExpenseType { get; set; }

        [DisplayName("Lain-lain")]
        [StringLength(256, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public string Other { get; set; }

        [DisplayName("Km")]
        [Range(1, int.MaxValue, ErrorMessageResourceName = "Range", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public int? Distance { get; set; }

        [DisplayName("Biaya")]
        [Range(1, int.MaxValue)]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public int? ExpenseValue { get; set; }

        [DisplayName("Attachment")]
        public string Attachment { get; set; }

        [DisplayName("Catatan")]
        [DataType(DataType.MultilineText)]
        [AllowHtml]
        public string Notes { get; set; }

        [DisplayName("Plat Nomor")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public string LicensePlate { get; set; }
        private List<SelectListItem> CarOptions { get; set; }
        public List<SelectListItem> CarExpenseTypeOptions
        {
            get
            {
                EnumHelper enumHelper = new EnumHelper();

                List<SelectListItem> options = new List<SelectListItem>();
                foreach (CarExpenseType item in enumHelper.EnumToList<CarExpenseType>().ToList())
                    options.Add(new SelectListItem { Value = (item).ToString(), Text = enumHelper.GetEnumDescription(item) });

                return options;
            }
        }
        public CarExpenseFormStub() { }

        public CarExpenseFormStub(car_expense dbItem)
        {
            DisplayFormatHelper dfh = new DisplayFormatHelper();

            this.Id = dbItem.id;
            this.ExpenseType = dbItem.expense_type;
            this.ExpenseDate = dbItem.expense_date;
            this.IdCar = dbItem.id_car;
            this.Other = dbItem.other;
            this.Distance = dbItem.distance;
            this.ExpenseValue = dbItem.expense_value;
            this.Attachment = dbItem.attachment;
            this.LicensePlate = dbItem.car.license_plate;
            this.CarBrandName = dbItem.car.car_model.car_brand.id;
            this.Notes = dbItem.notes;
        }

        public car_expense GetDbObject(string username)
        {
            car_expense dbItem = new car_expense();

            dbItem.expense_date = ExpenseDate;
            dbItem.id_car = IdCar;
            dbItem.other = Other;
            dbItem.expense_type = ExpenseType;
            dbItem.distance = Distance;
            dbItem.expense_value = ExpenseValue.Value;
            dbItem.attachment = Attachment;
            dbItem.notes = Notes;
            dbItem.created_by = username;
            dbItem.created_time = DateTimeOffset.Now;

            return dbItem;
        }

        public car_expense UpdateObject(car_expense dbItem, string user)
        {
            dbItem.id = Id;
            dbItem.expense_date = ExpenseDate;
            dbItem.id_car = IdCar;
            dbItem.other = Other;
            dbItem.expense_type = ExpenseType;
            dbItem.distance = Distance;
            dbItem.expense_value = ExpenseValue.Value;
            dbItem.attachment = Attachment;
            dbItem.notes = Notes;
            dbItem.updated_by = user;
            dbItem.updated_time = DateTimeOffset.Now;

            return dbItem;
        }

        public bool IsAttachmentImage()
        {
            List<string> strList = Attachment.Split('.').ToList();
            string fileType = strList.LastOrDefault();
            bool isImage = false;
            List<string> image = new List<string> { "jpg", "jpeg", "png", "bmp" };

            if (fileType != null)
            {
                fileType = fileType.ToLower();
                if (image.Contains(fileType))
                    isImage = true;
            }

            return isImage;
        }

        #region LicensePlate

        public void FillCarOptions(List<Business.Entities.car> list)
        {
            CarOptions = new List<SelectListItem>();
            foreach (Business.Entities.car item in list)
            {
                CarOptions.Add(new SelectListItem { Text = item.license_plate, Value = item.id.ToString() });
            }
        }

        #endregion

    }
}
