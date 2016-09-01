using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.Models.Service.Parameter
{
    public class CheckAvailabilityParam
    {
        public string City { get; set; }
        public DateTime? StartRent { get; set; }
        public DateTime? FinishRent { get; set; }
        public string CarBrandName { get; set; }
        public string CarModelName { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string PickupLocation { get; set; }
        public int? Capacity { get; set; }
    }
}