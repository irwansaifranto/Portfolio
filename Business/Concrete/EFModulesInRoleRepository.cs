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
    public class EFModulesInRoleRepository : IModulesInRoleRepository
    {
        private Entities.UserManagement context = new Entities.UserManagement();

        public ModulesInRoles FindByRoleAndModule(int roleId, Guid moduleId)
        {
            return context.ModulesInRoles.Where(x => x.RoleId == roleId).Where(x=>x.ModuleId == moduleId).FirstOrDefault();
        }

        public void RemoveAction(Guid moduleId, Guid actionId) {
            var modulesInRole = context.ModulesInRoles.Where(x => x.ModuleId == moduleId).ToList();
            foreach (ModulesInRoles mr in modulesInRole) {
                mr.Actions.Remove(context.Actions.Find(actionId));
            }
            context.SaveChanges();
        }

        /*
         * Delete all ModulesInRole if doesn't have actions
         * 
         * @param moduleId
         */
        public void DeleteByModule(Guid moduleId) {
            var modulesInRole = context.ModulesInRoles.Where(x => x.ModuleId == moduleId).ToList();
            foreach (ModulesInRoles mr in modulesInRole)
            {
                if (mr.Actions.Count <= 0) {
                    context.ModulesInRoles.Remove(mr);
                }
                
            }
            context.SaveChanges();
        }

        /*
         * Delete all ModulesInRole by Role
         * 
         * 
         * @param moduleId
         */
        public void DeleteByRole(int roleId)
        {
            var modulesInRole = context.ModulesInRoles.Where(x => x.RoleId == roleId).ToList();
            foreach (ModulesInRoles mr in modulesInRole)
            {
                if (mr.Actions.Count > 0)
                {
                    mr.Actions.Clear();
                }
                context.ModulesInRoles.Remove(mr);

            }
            context.SaveChanges();
        }
        
    }
}
