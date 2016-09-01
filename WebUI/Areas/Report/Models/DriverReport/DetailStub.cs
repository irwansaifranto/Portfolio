using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Business.Entities;

namespace WebUI.Areas.Report.Models.DriverReport
{
    public class DetailStub
    {
		// Example model value from scaffolder script: 0
		public Guid IdRent { get; set; }
        public string RentCode { get; set; }
		public DateTime Date { get; set; }
        public double Amount { get; set; }
		
		public DetailStub() { }

        public DetailStub(expense_item dbItem)
        {
            IdRent = dbItem.expense.id_rent;
            RentCode = dbItem.expense.rent.code;
            Date = dbItem.expense.date;
            Amount = dbItem.value;
		}

        public List<DetailStub> MapList(List<expense_item> dbItems)
        {
            List<DetailStub> retList = new List<DetailStub>();

            foreach (expense_item dbItem in dbItems)
                retList.Add(new DetailStub(dbItem));

            return retList;
        }
	}
}

