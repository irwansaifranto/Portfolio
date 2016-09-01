using Business.Entities;
using Business.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface ICarPackageRepository
    {
        List<car_package> FindAll(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null);
        car_package FindByPk(System.Guid id);
        int Count(FilterInfo filters = null);
        void Save(car_package dbItem);
        void Delete(car_package dbItem);
    }
}
