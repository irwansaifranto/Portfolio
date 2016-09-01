using Business.Entities;
using Business.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IApiRentRepository
    {
        List<api_rent> FindAll(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null);
        api_rent Find(FilterInfo filters);
        api_rent FindByPk(Guid id);
        int Count(FilterInfo filters = null);
        Guid Save(api_rent dbItem);
        void Delete(api_rent dbItem);
    }
}
