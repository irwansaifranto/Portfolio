using Business.Entities;
using Business.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IModuleRepository
    {
        void Create(Modules module);
        void Delete(string moduleName, bool foreignKey);
        List<Modules> Find(int skip = 0, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null, string filterLogic = null);
        int Count(List<FilterInfo> filters, string filterLogic);
        Modules FindByPk(Guid id);
        Modules FindByName(string moduleName);

        void GetAllChildrenInModule(string moduleName, ref List<Modules> result, int lvl);

        void addAction(Guid moduleId, Guid actionId);
        void removeAction(Guid moduleId, Guid actionId);

        List<Business.Entities.Actions> GetActionsInModule(string moduleName);
    }
}
