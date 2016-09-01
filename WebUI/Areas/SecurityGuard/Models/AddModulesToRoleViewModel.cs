using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Security;

namespace SecurityGuard.ViewModels
{
    public class AddModulesToRoleViewModel
    {
        public string GUID { get; set; }
        public string RoleName { get; set; }

        public List<Business.Entities.Actions> Actions { get; set; }

        public string Render { get; set; }

        //public SelectList AvailableActions { get; set; }
        //public SelectList AddedActions { get; set; }
    }
}
