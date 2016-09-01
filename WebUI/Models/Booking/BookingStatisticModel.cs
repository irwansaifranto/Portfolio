using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.Models.Booking
{
    public class BookingStatisticModel
    {
        public int TomorrowDeparture { get; set; }
        public int UnassignedCar { get; set; }
        public int UnassignedDriver { get; set; }
        public int UnpaidInvoice { get; set; }
        public int CancelledRent { get; set; }
    }
}