using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using WebUI.Models;

namespace WebUI.Infrastructure.Abstract
{
    interface ICustomPrincipal : IPrincipal
    {
        List<ModuleAction> Modules { get; set; }
        bool HasAccess(params string[] moduleNames);
    }
}