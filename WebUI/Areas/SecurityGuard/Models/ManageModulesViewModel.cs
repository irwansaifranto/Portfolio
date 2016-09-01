using System.Collections.Generic;
using System.Web.Mvc;

namespace SecurityGuard.ViewModels
{
    public class ManageModulesViewModel
    {
        public SelectList Modules { get; set; }
        public string[] ModuleList { get; set; }
    }
}
