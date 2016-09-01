using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Business.Entities;

namespace WebUI.Areas.Report.Models.DriverReport
{
    public class SummaryStub
    {
		// Example model value from scaffolder script: 0
		public Guid Id { get; set; }
		public string Name { get; set; }
        public double Amount { get; set; }
        public int Quantity { get; set; }
		
		public SummaryStub() { }

        public SummaryStub(driver dbItem)
        {
            this.Id = dbItem.id;
            this.Name = dbItem.name;
		}

		public List<SummaryStub> MapList(List<driver> drivers, List<rent> rents, List<expense_item> expenseItems)
        {
            List<SummaryStub> retList = new List<SummaryStub>();
            SummaryStub single;

            foreach (driver drive in drivers)
            {
                single = new SummaryStub(drive);
                single.Quantity = rents.Where(n => n.id_driver != null && n.id_driver == drive.id).Count();
                single.Amount = (double)expenseItems.Where(n => n.expense.rent.id_driver != null && n.expense.rent.id_driver.Value == drive.id).Sum(n => n.value);
                retList.Add(single);
            }

            return retList;
        }
	}
}

