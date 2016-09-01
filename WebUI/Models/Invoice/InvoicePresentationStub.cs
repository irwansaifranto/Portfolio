using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Business.Entities;
using Common.Enums;
using System.ComponentModel;
using System.Web.Script.Serialization;

namespace WebUI.Models.Invoice
{
    public class InvoicePresentationStub
    {
		// Example model value from scaffolder script: 0
		public System.Guid Id { get; set; }
		public System.Guid IdRent { get; set; }
        public System.Guid IdCustomer { get; set; }

        [DisplayName("Invoice Num")]
        public string Code { get; set; }

        [DisplayName("Tanggal Invoice")]
        public DateTime InvoiceDate { get; set; }

        [DisplayName("Booking ID")]
        public string RentCode { get; set; }

        [DisplayName("Tamu")]
        public string CustomerName { get; set; }

        [DisplayName("Harga")]
        public int Price { get; set; }

        public string Status { get; set; }
        public string StatusEnum { get; set; }

        public string CancelNotes { get; set; }

        [DisplayName("PPN")]
        public bool PPN { get; set; }

        public int PrePPNValue { get; set; }
        public int PPNValue { get; set; }

        [DisplayName("Total")]
        public int Total { get; set; }

        public List<InvoiceItemFormStub> AdditionalItem { get; set; }

        [DisplayName("Tambahan Item")]
        public string AdditionalItemText { get; set; }

        public DateTimeOffset CreatedTime { get; set; }

        //booking
        public string PhoneCustomer { get; set; }
        public string EmailCustomer { get; set; }
        public string CarModel { get; set; }
        public string Destination { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset FinishDate { get; set; }
        public List<rent_package> ListRentPackage { get; set; }
        public string Logo { get; set; }
        public string Contact { get; set; }
        public string Terms { get; set; }

        public string Notes { get; set; }
		
		public InvoicePresentationStub() { }

        public InvoicePresentationStub(invoice dbItem)
        {
            //kamus
            InvoiceStatus status = (InvoiceStatus)Enum.Parse(typeof(Common.Enums.InvoiceStatus), dbItem.status);
            int total = dbItem.price;

            //algoritma
            if (dbItem.invoice_item != null)
            {
                total += dbItem.invoice_item.Sum(m => m.value);
            }

            this.Id = dbItem.id;
            this.IdRent = dbItem.id_rent;
            this.IdCustomer = dbItem.rent.id_customer;
            this.Code = dbItem.code;
            this.InvoiceDate = dbItem.invoice_date;
            this.RentCode = dbItem.rent.code;
            this.CustomerName = dbItem.rent.customer.name;
            this.Price = dbItem.price;
            this.Status = new EnumHelper().GetEnumDescription(status);
            this.StatusEnum = status.ToString();
            this.CreatedTime = dbItem.created_time;
            this.Logo = dbItem.rent.owner.logo;
            this.Contact = dbItem.rent.owner.contact;
            this.Terms = dbItem.rent.owner.terms;
            this.CancelNotes = dbItem.cancel_notes;
            this.PPN = dbItem.ppn;
            this.Total = dbItem.total;
            this.Notes = dbItem.rent.notes;

            AdditionalItem = new List<InvoiceItemFormStub>();
            foreach (invoice_item single in dbItem.invoice_item)
            {
                AdditionalItem.Add(new InvoiceItemFormStub(single));
            }
            AdditionalItemText = new JavaScriptSerializer().Serialize(AdditionalItem);

            PrePPNValue = Price + AdditionalItem.Sum(m => m.Value);
            if (PPN)
            {
                PPNValue = (int)Math.Round(PrePPNValue * 0.1);
            }
           
            //booking data
            this.PhoneCustomer = dbItem.rent.customer.phone_number;
            this.EmailCustomer = dbItem.rent.customer.email;
            this.CarModel = dbItem.rent.car_model.name;
            this.Destination = dbItem.rent.pickup_location;
            this.StartDate = dbItem.rent.start_rent;
            this.FinishDate = dbItem.rent.finish_rent;
        }

        public InvoicePresentationStub(invoice dbItem,  List<rent_package> listRentPackage)
        {
            //kamus
            InvoiceStatus status = (InvoiceStatus)Enum.Parse(typeof(Common.Enums.InvoiceStatus), dbItem.status);
            int total = dbItem.price;

            //algoritma
            if (dbItem.invoice_item != null)
            {
                total += dbItem.invoice_item.Sum(m => m.value);
            }

            this.Id = dbItem.id;
            this.IdRent = dbItem.id_rent;
            this.IdCustomer = dbItem.rent.id_customer;
            this.Code = dbItem.code;
            this.InvoiceDate = dbItem.invoice_date;
            this.RentCode = dbItem.rent.code;
            this.CustomerName = dbItem.rent.customer.name;
            this.Price = dbItem.price;
            this.Status = new EnumHelper().GetEnumDescription(status);
            this.StatusEnum = status.ToString();
            this.CreatedTime = dbItem.created_time;
            this.Logo = dbItem.rent.owner.logo;
            this.Contact = dbItem.rent.owner.contact;
            this.Terms = dbItem.rent.owner.terms;
            this.CancelNotes = dbItem.cancel_notes;
            this.PPN = dbItem.ppn;
            this.Total = dbItem.total;
            this.Notes = dbItem.rent.notes;

            AdditionalItem = new List<InvoiceItemFormStub>();
            foreach (invoice_item single in dbItem.invoice_item)
            {
                AdditionalItem.Add(new InvoiceItemFormStub(single));
            }
            AdditionalItemText = new JavaScriptSerializer().Serialize(AdditionalItem);

            PrePPNValue = Price + AdditionalItem.Sum(m => m.Value);
            if (PPN)
            {
                PPNValue = (int)Math.Round(PrePPNValue * 0.1);
            }

            //booking data
            this.PhoneCustomer = dbItem.rent.customer.phone_number;
            this.EmailCustomer = dbItem.rent.customer.email;
            this.CarModel = dbItem.rent.car_model.name;
            this.Destination = dbItem.rent.pickup_location;
            this.StartDate = dbItem.rent.start_rent;
            this.FinishDate = dbItem.rent.finish_rent;
            this.ListRentPackage = listRentPackage;
        }


        public List<InvoicePresentationStub> MapList(List<invoice> dbItems)
        {
            List<InvoicePresentationStub> retList = new List<InvoicePresentationStub>();

            foreach (invoice dbItem in dbItems)
                retList.Add(new InvoicePresentationStub(dbItem));

            return retList;
        }
	}
}

