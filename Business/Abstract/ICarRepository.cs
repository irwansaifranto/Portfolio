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
    public interface ICarRepository
    {
        List<car> FindAll(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null);
        car Find(List<SortingInfo> sortings = null, FilterInfo filters = null);
        car FindByPk(Guid id);
        int Count(FilterInfo filters = null);
        void Save(car dbItem);
        void Delete(car dbItem);


	}
}