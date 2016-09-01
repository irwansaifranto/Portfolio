using Business.Abstract;
using Business.Entities;
using Business.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Linq;
using System.Data.Entity;
using Common.Enums;

namespace Business.Concrete
{
    public class EFExpenseRepository : IExpenseRepository
    {
        private Entities.Entities context = new Entities.Entities();

        #region expense

        private IQueryable<expense> IQExpense(List<SortingInfo> sortings = null, FilterInfo filters = null)
        {
            IQueryable<expense> list = context.expenses;

            if (filters != null && (filters.Filters != null && filters.Filters.Count > 0))
            {
                filters.FormatFieldToUnderscore();
                GridHelper.ProcessFilters<expense>(filters, ref list);
            }

            if (sortings != null && sortings.Count > 0)
            {
                foreach (var s in sortings)
                {
                    s.FormatSortOnToUnderscore();
                    list = list.OrderBy<expense>(s.SortOn + " " + s.SortOrder);
                }
            }
            else
            {
                list = list.OrderBy<expense>("id desc"); //default, wajib ada atau EF error
            }

            return list;
        }

        public List<expense> FindAll(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null)
        {
            IQueryable<expense> list = IQExpense(sortings, filters);

            //take & skip
            var takeList = list;
            if (skip != null)
            {
                takeList = takeList.Skip(skip.Value);
            }
            if (take != null)
            {
                takeList = takeList.Take(take.Value);
            }

            //return result
            //var sql = takeList.ToString();
            List<expense> result = takeList.ToList();
            return result;
        }

        public expense FindByPk(System.Guid id)
        {
            return context.expenses.Find(id);
        }

        public int Count(FilterInfo filters = null)
        {
            IQueryable<expense> items = IQExpense(null, filters);

            return items.Count();
        }

        public void Save(expense dbItem)
        {
            if (dbItem.id == Guid.Empty) //create
            {
                dbItem.id = Guid.NewGuid(); 

                context.expenses.Add(dbItem);
            }
            else //edit
            {
                var entry = context.Entry(dbItem);
                entry.State = EntityState.Modified;
            }
            context.SaveChanges();
        }

        public void Delete(expense dbItem)
        {
            context.expenses.Remove(dbItem);
            context.SaveChanges();
        }

        #endregion

        #region expense item

        private IQueryable<expense_item> IQexpenseItem(List<SortingInfo> sortings = null, FilterInfo filters = null)
        {
            IQueryable<expense_item> list = context.expense_item;

            if (filters != null && (filters.Filters != null && filters.Filters.Count > 0))
            {
                filters.FormatFieldToUnderscore();
                GridHelper.ProcessFilters<expense_item>(filters, ref list);
            }

            if (sortings != null && sortings.Count > 0)
            {
                foreach (var s in sortings)
                {
                    s.FormatSortOnToUnderscore();
                    list = list.OrderBy<expense_item>(s.SortOn + " " + s.SortOrder);
                }
            }
            else
            {
                list = list.OrderBy<expense_item>("id desc"); //default, wajib ada atau EF error
            }

            return list;
        }

        public List<expense_item> FindAllItem(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null)
        {
            IQueryable<expense_item> list = IQexpenseItem(sortings, filters);

            //take & skip
            var takeList = list;
            if (skip != null)
            {
                takeList = takeList.Skip(skip.Value);
            }
            if (take != null)
            {
                takeList = takeList.Take(take.Value);
            }

            //return result
            //var sql = takeList.ToString();
            List<expense_item> result = takeList.ToList();
            return result;
        }

        public expense_item FindItemByPk(System.Guid id)
        {
            return context.expense_item.Find(id);
        }

        public int CountItem(FilterInfo filters = null)
        {
            IQueryable<expense_item> items = IQexpenseItem(null, filters);

            return items.Count();
        }

        public void SaveItem(expense_item dbItem)
        {
            if (dbItem.id == Guid.Empty) //create
            {
                dbItem.id = Guid.NewGuid();

                context.expense_item.Add(dbItem);
            }
            else //edit
            {
                var entry = context.Entry(dbItem);
                entry.State = EntityState.Modified;
            }
            context.SaveChanges();
        }

        public void DeleteItem(expense_item dbItem)
        {
            context.expense_item.Remove(dbItem);
            context.SaveChanges();
        }

        #endregion

        #region report

        public List<DriverExpenseReport> GetDriverReport(Guid idOwner, DateTime startDate, DateTime endDate)
        {
            var query = from ei in context.expense_item 
                        where ei.category == ExpenseItemCategory.DRIVER.ToString() && 
                            ei.expense.rent.id_owner == idOwner &&
                            ei.expense.date >= startDate && ei.expense.date <= endDate &&
                            ei.expense.rent.id_driver != null
                        group ei by ei.expense.rent.id_driver into s
                        select new DriverExpenseReport
                        {
                            Id = s.Key.Value,
                            Amount = s.Sum(q => q.value),
                            Quantity = s.Count()
                        };

            List<DriverExpenseReport> result = query.ToList();

            foreach (DriverExpenseReport single in result)
                single.Name = context.drivers.Where(m => m.id == single.Id).First().name;

            return result;
        }

        //public List<DriverExpenseReport> GetDriverReport(Guid id, DateTime startDate, DateTime endDate)
        //{
        //    var query = from ei in context.expense_item
        //                where ei.category == ExpenseItemCategory.DRIVER.ToString() &&
        //                    ei.expense.date >= startDate && ei.expense.date <= endDate &&
        //                    ei.expense.rent.id_driver == id
        //                select new DriverExpenseReport
        //                {
        //                    Id = s.Key.Value,
        //                    Amount = s.Sum(q => q.value),
        //                    Quantity = s.Count()
        //                };

        //    List<DriverExpenseReport> result = query.ToList();

        //    foreach (DriverExpenseReport single in result)
        //        single.Name = context.drivers.Where(m => m.id == single.Id).First().name;

        //    return result;
        //}

        public List<VehicleExpenseReport> GetVehicleReport(Guid idOwner, DateTime startDate, DateTime endDate)
        {
            var query = from ei in context.expense_item
                        where ei.category == ExpenseItemCategory.VEHICLE.ToString() && 
                            ei.expense.rent.id_owner == idOwner &&
                            ei.expense.date >= startDate && ei.expense.date <= endDate
                        group ei by ei.expense.rent.car.license_plate into s
                        select new VehicleExpenseReport
                        {
                            Name = s.Key,
                            Amount = s.Sum(q => q.value),
                            Quantity = s.Count()
                        };

            return query.ToList();
        }

        public List<DetailVehicleExpenseReport> GetDetailVehicleReport(Guid idOwner, DateTime startDate, DateTime endDate, Guid idCar)
        {
            var query = from ei in context.expense_item
                        where ei.category == ExpenseItemCategory.VEHICLE.ToString() &&
                            ei.expense.rent.id_owner == idOwner &&
                            ei.expense.rent.id_car == idCar &&
                            ei.expense.date >= startDate && ei.expense.date <= endDate
                        select new DetailVehicleExpenseReport
                        {
                            IdRent = ei.expense.rent.id,
                            BookingCode = ei.expense.rent.code,
                            Date = ei.expense.date,
                            Value = ei.value
                        };

            return query.ToList();
        }

        #endregion
    }
}
