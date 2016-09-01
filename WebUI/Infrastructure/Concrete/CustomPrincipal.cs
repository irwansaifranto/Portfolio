using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using WebUI.Infrastructure.Abstract;
using WebUI.Models;

namespace WebUI.Infrastructure.Concrete
{
    public class CustomPrincipal : ICustomPrincipal
    {
        public IIdentity Identity { get; set; }

        public CustomPrincipal(string username)
        {
            this.Identity = new GenericIdentity(username);
        }

        public bool IsInRole(string role)
        {
            return Identity != null && Identity.IsAuthenticated &&
              !string.IsNullOrWhiteSpace(role) && Roles.IsUserInRole(Identity.Name, role);
        }

        public bool HasAccess(params string[] moduleNames)
        {
            //kamus
            bool isHas = false;

            //algoritma
            if (this.Modules != null)
            {
                isHas = this.Modules.Where(m => moduleNames.Contains(m.ModuleName)).Count() > 0;
            }

            return isHas;
        }

        public void Parse(CustomPrincipalSerializeModel serializeModel)
        {
            this.IdOwner = serializeModel.IdOwner;
            this.OwnerName = serializeModel.OwnerName;
            this.Modules = serializeModel.Modules;
        }

        public Guid? IdOwner { get; set; }
        public string OwnerName { get; set; }
        public List<ModuleAction> Modules { get; set; }
    }
}