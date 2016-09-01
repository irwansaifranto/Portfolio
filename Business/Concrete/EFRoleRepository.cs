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
    public class EFRoleRepository : IRoleRepository
    {
        private Entities.UserManagement context = new Entities.UserManagement();

        public void AddModuleAndAction(string[] modules, string role)
        {
            roles r = context.roles.Where(x => x.role_name == role).FirstOrDefault();
            IEnumerable<ModulesInRoles> listModule = r.ModulesInRoles;

            foreach (ModulesInRoles mInRole in listModule)
            {
                if (mInRole.Actions.Count > 0)
                {
                    var actions = mInRole.Actions.ToList();
                    foreach (var a in actions)
                    {
                        mInRole.Actions.Remove(a);
                    }
                }
            }

            context.ModulesInRoles.RemoveRange(listModule);
            context.SaveChanges();

            foreach (string s in modules)
            {
                string[] temp = s.Split(';');
                ModulesInRoles mr;
                Guid moduleId = new Guid(temp.First());
                Guid actionId = new Guid(temp.Last());

                ModulesInRoles available = context.ModulesInRoles.Where(x => x.RoleId == r.role_id).Where(x => x.ModuleId == moduleId).FirstOrDefault();
                Business.Entities.Actions a = context.Actions.Find(actionId);

                if (available != null)
                {
                    if (!available.Actions.Contains(a))
                    {
                        available.Actions.Add(a);
                        context.SaveChanges();
                    }
                }
                else
                {
                    mr = new ModulesInRoles()
                    {
                        Id = Guid.NewGuid(),
                        RoleId = r.role_id,
                        ModuleId = new Guid(temp.First())
                    };
                    mr.Actions.Add(a);
                    r.ModulesInRoles.Add(mr);
                    context.SaveChanges();
                }
            }
        }

        public roles FindByName(string roleName) {
            return context.roles.Where(x => x.role_name == roleName).FirstOrDefault();
        }
        
    }
}
