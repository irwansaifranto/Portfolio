using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.Models.Service.Parameter
{
    public class DriverPositionParam
    {
        public Guid Id { get; set; }
        public double Lat { get; set; }
        public double Long { get; set; }
    }
}