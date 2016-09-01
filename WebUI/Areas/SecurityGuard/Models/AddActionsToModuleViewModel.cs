using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Security;

namespace SecurityGuard.ViewModels
{
    public class AddActionsToModuleViewModel
    {
        public string GUID { get; set; }
        public string ModuleName { get; set; }
        public SelectList AvailableActions { get; set; }
        public SelectList AddedActions { get; set; }
    }
}
