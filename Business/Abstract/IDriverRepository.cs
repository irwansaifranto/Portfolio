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
    public interface IDriverRepository
    {
		List<driver> FindAll(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null);
        driver FindByPk(Guid id);
        int Count(FilterInfo filters = null);
        void Save(driver dbItem);
        void Delete(driver dbItem);


	}
}