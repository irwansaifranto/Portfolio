using Business.Entities;
using Business.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IExpenseRepository
    {
        #region expense

        List<expense> FindAll(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null);
        expense FindByPk(System.Guid id);
        int Count(FilterInfo filters = null);
        void Save(expense dbItem);
        void Delete(expense dbItem);

        #endregion

        #region expense item

        List<expense_item> FindAllItem(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null);
        expense_item FindItemByPk(System.Guid id);
        int CountItem(FilterInfo filters = null);
        void SaveItem(expense_item dbItem);
        void DeleteItem(expense_item dbItem);

        #endregion

        #region report

        List<DriverExpenseReport> GetDriverReport(Guid idOwner, DateTime startDate, DateTime endDate);
        List<VehicleExpenseReport> GetVehicleReport(Guid idOwner, DateTime startDate, DateTime endDate);
        List<DetailVehicleExpenseReport> GetDetailVehicleReport(Guid idOwner, DateTime startDate, DateTime endDate, Guid idCar);

        #endregion
    }
}
