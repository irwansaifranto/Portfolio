using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.Models
{
    public class UserLogin
    {
        public string MobidigUserId { get; set; }
        public string Username { get; set; }
        public List<string> Roles { get; set; }
    }
}