using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using Common.Enums;
using Business.Infrastructure;
using Business.Entities;

namespace Business.Abstract
{
    public interface ICarModelRepository
    {
        List<car_model> FindAll(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null);
        car_model Find(List<SortingInfo> sortings = null, FilterInfo filters = null);
        car_model FindByPk(System.Guid id);
        int Count(FilterInfo filters = null);
        void Save(car_model dbItem);
        void Delete(car_model dbItem);


	}
}