using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Business.Entities;
using Common.Enums;
using System.Web.Script.Serialization;
using WebUI.Infrastructure.Concrete;
using WebUI.Infrastructure;
using WebUI.Infrastructure.Validation;

namespace WebUI.Models.Invoice
{
    public class InvoiceFormStub
    {
		// Example model value from scaffolder script: 0
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
		public System.Guid Id { get; set; }

        [DisplayName("Kode Booking")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public System.Guid? IdRent{ get; set; }

        [DisplayName("Tamu")]
		public string CustomerName { get; set; }

		[DisplayName("Tanggal")]
		public string StartRent { get; set; }

        //invoice
        [DisplayName("Invoice Num")]
        [MaxLength(50)]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public string Code { get; set; }
        public string RentCode { get; set; }

        [DisplayName("Tanggal Invoice")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public DateTime InvoiceDate { get; set; }

        [DisplayName("Harga")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public int Price { get; set; }

        [DisplayName("Status")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public string Status { get; set; }

        [DisplayName("Catatan Pembatalan")]
        [CheckNote("Status",ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        public List<InvoiceItemFormStub> AdditionalItem { get; set; }

        [DisplayName("Tambahan Item")]
        public string AdditionalItemText { get; set; }

        [DisplayName("PPN")]
        public bool PPN { get; set; }

		private List<SelectListItem> RentOptions { get; set; }

        private List<SelectListItem> StatusOptions { get; set; }

        public InvoiceFormStub()
        {
            PPN = false;
        }

		public InvoiceFormStub(List<Business.Entities.rent> listRent)
			: this()
		{
            this.FillRentOptions(listRent);
            this.FillStatusOptions();
            this.InvoiceDate = DateTime.Now.AddDays(1);
		}


        public InvoiceFormStub(invoice dbItem, List<Business.Entities.rent> listRent)
			: this(listRent)
		{
            DisplayFormatHelper dfh = new DisplayFormatHelper();

			this.Id = dbItem.id;
			this.IdRent = dbItem.id_rent;
			this.CustomerName = dbItem.rent.customer.name;
            this.StartRent = dbItem.rent.start_rent.ToString(dfh.FullDateTimeFormat) + " s/d " + dbItem.rent.finish_rent.ToString(dfh.FullDateTimeFormat);
            this.Code = dbItem.code;
            this.InvoiceDate = dbItem.invoice_date;
            this.Price = dbItem.price;
            this.Status = dbItem.status;
            this.Notes = dbItem.cancel_notes;
            this.RentCode = dbItem.rent.code;
            this.PPN = dbItem.ppn;

            AdditionalItem = new List<InvoiceItemFormStub>();
            foreach (invoice_item single in dbItem.invoice_item)
            {
                AdditionalItem.Add(new InvoiceItemFormStub(single));                
            }
            AdditionalItemText = new JavaScriptSerializer().Serialize(AdditionalItem);
		}

        public invoice GetDbObject(string user)
        {
            invoice dbItem = new invoice();

            dbItem.id = this.Id;
            dbItem.id_rent = this.IdRent.Value;
            dbItem.code = this.Code;
            dbItem.invoice_date = this.InvoiceDate;
            dbItem.price = this.Price;
            dbItem.status = this.Status;
            dbItem.cancel_notes = this.Notes;
            dbItem.created_by = user;
            dbItem.created_time = DateTime.Now;
            dbItem.ppn = PPN;
            dbItem.total = CalculateTotal();

            return dbItem;
        }

        public invoice SetDbObject(invoice dbItem, string user)
        {
            dbItem.id_rent = this.IdRent.Value;
            dbItem.code = this.Code;
            dbItem.invoice_date = this.InvoiceDate;
            dbItem.price = this.Price;
            dbItem.status = this.Status;
            dbItem.cancel_notes = this.Notes;
            dbItem.updated_by = user;
            dbItem.updated_time = DateTime.Now;
            dbItem.ppn = PPN;
            dbItem.total = CalculateTotal();
            
            return dbItem;
        }

        private int CalculateTotal()
        {
            int total = 0;

            total = this.Price + (AdditionalItem != null ? this.AdditionalItem.Sum(m => m.Value) : 0);

            if (PPN)
                total = total + (int)Math.Round(total * 0.1);

            return total;
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

        public void FillStatusOptions()
        {
            StatusOptions = new List<SelectListItem>();
            EnumHelper enumHelper = new EnumHelper();
            foreach (InvoiceStatus item in enumHelper.EnumToList<InvoiceStatus>().ToList())
            {
                StatusOptions.Add(new SelectListItem { Value = item.ToString(), Text = enumHelper.GetEnumDescription(item) });
            }
        }

        public string GetRentOptions() {
            return new JavaScriptSerializer().Serialize(RentOptions);
        }

        public string GetStatusOptions()
        {
            return new JavaScriptSerializer().Serialize(StatusOptions);
        }

		#endregion

	}
}