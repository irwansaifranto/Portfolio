using Business.Entities;
using Business.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IModulesInRoleRepository
    {
        ModulesInRoles FindByRoleAndModule(int roleId, Guid moduleId);
        void RemoveAction(Guid moduleId,Guid actionId);
        void DeleteByModule(Guid moduleId);
        void DeleteByRole(int roleId);
    }
}
