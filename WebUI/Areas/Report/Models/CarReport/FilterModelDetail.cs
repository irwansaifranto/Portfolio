using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.Areas.Report.Models
{
    public class FilterModelDetail
    {
        public Guid? Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid IdCar {get;set;}
        public Guid IdInvoice { get; set; }

        public FilterModelDetail()
        {
            StartDate = DateTime.Now.AddDays(-30);
            EndDate = DateTime.Now;
        }

        public FilterModelDetail(DateTime startDate, DateTime endDate, Guid idCar)
        {
            StartDate = startDate;
            EndDate = endDate;
            IdCar = idCar;
            IdInvoice = IdInvoice;
        }
    }
}