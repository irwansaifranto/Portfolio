using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Business.Entities;
using Common.Enums;

namespace WebUI.Models.Invoice
{
    public class StatisticPresentationStub
    {
		// Example model value from scaffolder script: 0
		public int Total { get; set; }
		public int Month { get; set; }
        public int TotalPaid { get; set; }
		public int TotalUnpaid { get; set; }
		public float PersentasePaid { get; set; }
		public float PersentaseUnpaid { get; set; }

        public DateTimeOffset CreatedTime { get; set; }
		
		public StatisticPresentationStub() { }

        public StatisticPresentationStub(List<invoice> dbItems)
        {
            Month = DateTime.Now.Month;
            TotalPaid = dbItems.Where(x => x.status == InvoiceStatus.PAID.ToString() && x.invoice_date.Month == Month).Sum(x => x.price);
            TotalUnpaid = dbItems.Where(x => x.status == InvoiceStatus.UNPAID.ToString()).Sum(x => x.price);
            Total = TotalPaid + TotalUnpaid;
            PersentasePaid = 0;
            PersentaseUnpaid = 0;
            if (Total > 0)
            {
                PersentasePaid = ((float)TotalPaid * 100) / (float)Total;
                PersentaseUnpaid = ((float)TotalUnpaid * 100) / (float)Total;
            }
        }
	}
}

