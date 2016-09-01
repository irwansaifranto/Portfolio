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
    public interface IRentPositionRepository
    {
        List<rent_position> FindAll(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null);
        rent_position FindByPk(Guid id);
        int Count(FilterInfo filters = null);
        void Save(rent_position dbItem);
      


	}
}