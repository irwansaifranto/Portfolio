using Business.Entities;
using Common.Enums;
using NPOI.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.WebPages.Html;
using WebUI.Infrastructure;

namespace WebUI.Models.Cost
{
    public class CostFormStub
    {
        
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
		public System.Guid Id { get; set; }

        [DisplayName("Kode Booking")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public System.Guid? IdRent{ get; set; }

        public System.Guid IdExpenseItem { get; set; }

        [DisplayName("Tamu")]
		public string CustomerName { get; set; }

		[DisplayName("Tanggal")]
		public string StartRent { get; set; }

        public string Category { get; set; }
        [DisplayName("Tanggal")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public DateTime Date { get; set; }

        [DisplayName("Mobil")]
        public int? Car { get; set; }

        [DisplayName("Supir")]
        public int? Driver { get; set; }

        [DisplayName("BBM")]
        public int? Gas { get; set; }

        [DisplayName("Tol")]
        public int? Toll { get; set; }

        [DisplayName("Parkir")]
        public int? Parking { get; set; }

        [DisplayName("Biaya Lain")]
        public int? Other { get; set; }

        public int ValueVehicle { get; set; }
        public int ValueDriver { get; set; }
        public int ValueGas { get; set; }
        public int ValueToll { get; set; }
        public int ValueParking { get; set; }
        public int ValueOther { get; set; }

        [DisplayName("Keterangan")]
        [DataType(DataType.MultilineText)]
        [System.Web.Mvc.AllowHtml]
        [MaxLength(100)]
        public string Description { get; set; }

        //invoice
        public string RentCode { get; set; }

		private List<SelectListItem> RentOptions { get; set; }

        private List<SelectListItem> CategoryOptions { get; set; }

		public CostFormStub() { }

		public CostFormStub(List<Business.Entities.rent> listRent)
			: this()
		{
            this.FillRentOptions(listRent);
            this.FillCategoryOptions();
		}


        public CostFormStub(expense dbItem, List<Business.Entities.rent> listRent)
			: this(listRent)
		{
            DisplayFormatHelper dfh = new DisplayFormatHelper();

			this.Id = dbItem.id;
			this.IdRent = dbItem.id_rent;
			this.CustomerName = dbItem.rent.customer.name;
            this.StartRent = dbItem.rent.start_rent.ToString(dfh.FullDateTimeFormat) + " s/d " + dbItem.rent.finish_rent.ToString(dfh.FullDateTimeFormat);
            this.Date = dbItem.date;
            this.RentCode = dbItem.rent.code;
            List<expense_item> expenseItemList = dbItem.expense_item.ToList();
            

		}

        public CostFormStub(expense dbItem)
        {
            DisplayFormatHelper dfh = new DisplayFormatHelper();

            this.Id = dbItem.id;
            this.IdRent = dbItem.id_rent;
            this.CustomerName = dbItem.rent.customer.name;
            this.StartRent = dbItem.rent.start_rent.ToString(dfh.FullDateTimeFormat) + " s/d " + dbItem.rent.finish_rent.ToString(dfh.FullDateTimeFormat);
            this.Date = dbItem.date;
            this.RentCode = dbItem.rent.code;
            List<expense_item> expenseItemList = dbItem.expense_item.ToList();


        }

        public expense GetDbObject(string user)
        {
            expense dbItem = new expense();
            dbItem.id = this.Id;
            dbItem.id_rent = this.IdRent.Value;
            dbItem.date = this.Date;
            dbItem.created_by = user;
            dbItem.created_time = DateTimeOffset.Now;
            return dbItem;
        }

        public expense SetDbObject(expense dbItem, string user)
        {
            dbItem.id_rent = this.IdRent.Value;
            dbItem.date = this.Date;
            dbItem.updated_by = user;
            dbItem.updated_time = DateTimeOffset.Now;
            return dbItem;
        }

		#region options

		public void FillRentOptions(List<Business.Entities.rent> list)
		{
			RentOptions = new List<SelectListItem>();
			foreach (Business.Entities.rent item in list)
            {
                RentOptions.Add(new SelectListItem { Text = item.code, Value = item.id.ToString() });
            }
		
		}

        public void FillCategoryOptions()
        {
            CategoryOptions = new List<SelectListItem>();
            EnumHelper enumHelper = new EnumHelper();
            foreach (ExpenseItemCategory item in enumHelper.EnumToList<ExpenseItemCategory>().ToList())
            {
                CategoryOptions.Add(new SelectListItem { Value = item.ToString(), Text = enumHelper.GetEnumDescription(item) });
            }
        }

        public string GetRentOptions() {
            return new JavaScriptSerializer().Serialize(RentOptions);
        }

        public string GetCategoryOptions()
        {
            CategoryOptions = CategoryOptions.OrderBy(x => x.Text).ToList();
            return new JavaScriptSerializer().Serialize(CategoryOptions);
        }

		#endregion

	}
}
