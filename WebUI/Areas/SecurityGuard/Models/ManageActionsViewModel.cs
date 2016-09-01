using System.Collections.Generic;
using System.Web.Mvc;

namespace SecurityGuard.ViewModels
{
    public class ManageActionsViewModel
    {
        public SelectList Actions { get; set; }
        public string[] ActionList { get; set; }
    }
}
