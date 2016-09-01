using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using WebUI.Models.Booking;

namespace WebUI.Areas.Report.Models
{
    public class DailyFilterModel
    {
        public Guid? Id { get; set; }
        [DisplayName("Tanggal")]
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string CarModelName { get; set; }
        public string DriverName { get; set; }
        public string LicensePlate { get; set; }
        //public BookingPresentationStub Base { get; set; }


        public DailyFilterModel()
        {
            StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            EndDate = DateTime.Now;
        }

        public DailyFilterModel(DateTime startDate, DateTime endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}