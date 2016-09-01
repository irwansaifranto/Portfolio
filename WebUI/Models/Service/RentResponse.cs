using Business.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.Models.Service
{
    public class RentResponse
    {
        public bool Success { get; set; }
        public string RentCode { get; set; }
        public string Message { get; set; }

        public RentResponse() { }
    }
}