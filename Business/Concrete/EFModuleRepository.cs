using Business.Abstract;
using Business.Entities;
using Business.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Linq;
using System.Text.RegularExpressions;

namespace Business.Concrete
{
    public class EFModuleRepository : IModuleRepository
    {
        private Entities.UserManagement context = new Entities.UserManagement();

        public void Create(Modules module)
        {
            context.Modules.Add(module);
            context.SaveChanges();
        }

        public void Delete(string moduleName, bool foreignKey)
        {
            List<Modules> delete = new List<Modules>();
            GetAllChildrenInModule(moduleName, ref delete, 0);

            for (int i = 0; i < delete.Count; ++i)
            {
                if (delete[i].Actions.Count > 0)
                {
                    delete[i].Actions.Clear();
                }

                if (!foreignKey)
                {
                    for (int j = 0; j < delete[i].ModulesInRoles.Count; j++)
                    {
                        delete[i].ModulesInRoles.ElementAt(j).Actions.Clear();
                    }
                    context.ModulesInRoles.RemoveRange(delete[i].ModulesInRoles);
                    context.SaveChanges();
                }
            }

            context.Modules.RemoveRange(delete);
            context.SaveChanges();
        }

        public List<Modules> Find(int skip = 0, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null, string filterLogic = null)
        {
            IQueryable<Modules> modules = context.Modules;

            if (filters != null && (filters.Filters != null && filters.Filters.Count > 0))
            {
                filters.FormatFieldToUnderscore();
                GridHelper.ProcessFilters<Business.Entities.Modules>(filters, ref modules);
            }

            if (sortings != null && sortings.Count > 0)
            {
                foreach (var s in sortings)
                {
                    string sortOn = this.MapSort(s.SortOn);
                    modules = modules.OrderBy(sortOn + " " + s.SortOrder);
                }
            }
            else
            {
                modules = modules.OrderBy("ModuleName asc");
            }

            var takeModules = modules;
            if (take != null)
            {
                takeModules = modules.Skip(skip).Take((int)take);
            }

            List<Modules> moduleList = takeModules.ToList();

            return moduleList;
        }

        public Modules FindByName(string moduleName)
        {
            return context.Modules.Where(x => x.ModuleName == moduleName).SingleOrDefault();
        }

        public int Count(List<FilterInfo> filters, string filterLogic)
        {
            throw (new NotImplementedException());
        }

        public Modules FindByPk(Guid id)
        {
            return context.Modules.Find(id);
        }


        public List<Business.Entities.Actions> GetActionsInModule(string moduleName)
        {
            List<Business.Entities.Actions> result = context.Modules.Where(x => x.ModuleName == moduleName).First().Actions.ToList();
            for (int i = 0; i < result.Count; ++i)
            {
                result[i].Modules.Clear();
                result[i].ModulesInRoles.Clear();
            }

            return result;
        }

        public string MapSort(string sortOn)
        {
            string mapSortOn = sortOn;
            mapSortOn = Regex.Replace(sortOn, @"(\p{Ll})(\p{Lu})", "$1_$2");

            return mapSortOn;
        }

        public void GetAllChildrenInModule(string moduleName, ref List<Modules> result, int lvl)
        {
            lvl = lvl + 1;
            Modules m = context.Modules.Where(x => x.ModuleName == moduleName).FirstOrDefault();
            //m.lvl = lvl;
            List<Modules> childrens = context.Modules.Where(x => x.ParentModule == moduleName).ToList();
            if (m != null)
            {
                result.Add(m);
            }

            foreach (Modules x in childrens)
            {
                GetAllChildrenInModule(x.ModuleName, ref result, lvl);
            }
        }

        public void addAction(Guid moduleId, Guid actionId)
        {
            Modules m = context.Modules.Find(moduleId);
            Business.Entities.Actions a = context.Actions.Find(actionId);
            if (!m.Actions.Contains(a))
            {
                m.Actions.Add(a);
                context.SaveChanges();
            }
        }

        public void removeAction(Guid moduleId, Guid actionId)
        {
            Modules m = context.Modules.Find(moduleId);
            Business.Entities.Actions a = context.Actions.Find(actionId);
            if (m.Actions.Contains(a))
            {
                m.Actions.Remove(a);
                context.SaveChanges();
            }
        }
    }
}
