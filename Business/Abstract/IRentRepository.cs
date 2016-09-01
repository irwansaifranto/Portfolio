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
    public interface IRentRepository
    {
        #region rent

        List<rent> FindAll(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null);
        List<rent> FindAll(Guid idOwner, DateTime start, DateTime finish, bool includeCancel = false, FilterInfo filters = null);
        rent FindByPk(Guid id);
        rent Find(FilterInfo filters);
        int Count(FilterInfo filters = null);
        Guid Save(rent dbItem);
        void Delete(rent dbItem);
        List<rent_package> FindAllRentPackage(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null);
        void SaveListPackage(rent_package dbItem);
        void DeleteRentPackage(rent_package dbItem); 
        int CountUnassignedCar(FilterInfo filters = null);
        int CountUnassignedDriver(FilterInfo filters = null);

        #endregion

        #region rent code

        string GenerateRentCode(owner owner);


        #endregion
    }
}