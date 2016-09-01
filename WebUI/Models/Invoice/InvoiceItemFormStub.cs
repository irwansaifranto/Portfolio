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
    public class InvoiceItemFormStub
    {
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
		public Guid? Id { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public Guid? IdInvoice { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public string Category { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public int Value { get; set; }

		public InvoiceItemFormStub() { }

        public InvoiceItemFormStub(invoice_item dbItem)
            : this()
        {
            Id = dbItem.id;
            IdInvoice = dbItem.id_invoice;
            Category = dbItem.category;
            Value = dbItem.value;
        }

        public invoice_item GetDbObject(Guid idInvoice)
        {
            invoice_item dbItem = new invoice_item
            {
                category = Category,
                value = Value,
                id_invoice = idInvoice
            };

            return dbItem;
        }

        public void SetDbObject(invoice_item dbItem)
        {
            dbItem.category = Category;
            dbItem.value = Value;
        }
	}
}