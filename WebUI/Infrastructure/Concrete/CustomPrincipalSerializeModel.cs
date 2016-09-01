using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebUI.Models;

namespace WebUI.Infrastructure.Concrete
{
    public class CustomPrincipalSerializeModel
    {
        public Guid? IdOwner { get; set; }
        public string OwnerName { get; set; }
        public List<ModuleAction> Modules { get; set; }
    }
}