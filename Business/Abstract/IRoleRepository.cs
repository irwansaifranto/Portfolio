using Business.Entities;
using Business.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IRoleRepository
    {
        roles FindByName(string roleName);
        void AddModuleAndAction(string[] modules,string role);
    }
}
