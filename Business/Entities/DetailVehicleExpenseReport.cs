using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Entities
{
    public class DetailVehicleExpenseReport
    {
        public Guid IdRent { get; set; }
        public string BookingCode { get; set; }
        public DateTime Date { get; set; }
        public double Value { get; set; }

    }
}
