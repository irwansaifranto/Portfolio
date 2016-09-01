using System.ComponentModel.DataAnnotations;
using Business.Entities;
using System.Collections.Generic;
using System;

namespace SecurityGuard.ViewModels
{
    public class ModuleViewModel
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Module Name is required.")]
        public string ModuleName { get; set; }
        public string ParentModule { get; set; }
        public bool HasChildren { get; set; }
        public string Description { get; set; }
        public int Level { get; set; }
        public string Space { get; set; }

        public List<string> Actions { get; set; }

        public ModuleViewModel() { }

        public ModuleViewModel(Modules m, int level) {
            Id = m.Id;
            this.ModuleName = m.ModuleName;
            this.Level = level;
            //this.Space = "".PadLeft(m.lvl, ' ');
            this.Space = "".PadLeft(Level, ' ');
        }

        public List<ModuleViewModel> MapRecursive(List<Modules> dbItems, string parentModule = null, int level = 1)
        {
            List<ModuleViewModel> result = new List<ModuleViewModel> { };
            List<Modules> modules = dbItems.FindAll(m => m.ParentModule == parentModule);
            ModuleViewModel mvm;
            int childLevel = level + 1;

            foreach (Modules row in modules)
            {
                mvm = new ModuleViewModel(row, level);
                mvm.Actions = new List<string>();

                var enumerator = row.Actions.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    mvm.Actions.Add(enumerator.Current.ActionName);
                }

                result.Add(mvm);
                result.AddRange(this.MapRecursive(dbItems, row.ModuleName, childLevel));
            }

            return result;
        }
    }
}
