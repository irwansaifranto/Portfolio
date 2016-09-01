using Business.Entities;
using Business.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IActionRepository
    {
        void Create(Business.Entities.Actions module);
        void Delete(string actionName, bool fk);
        List<Business.Entities.Actions> Find(int skip = 0, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null, string filterLogic = null);
        int Count(List<FilterInfo> filters, string filterLogic);
        Business.Entities.Actions FindByPk(Guid id);

        List<Modules> GetModulesInAction(string actionName);

        
    }
}
