using Business.Abstract;
using Business.Entities;
using Business.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Business.Linq;

namespace Business.Concrete
{
    public class EFCarExpenseRepository : ICarExpenseRepository
    {

        private Entities.Entities context = new Entities.Entities();

        #region car_expense

        private IQueryable<car_expense> IQCarExpense(List<SortingInfo> sortings = null, FilterInfo filters = null)
        {
            IQueryable<car_expense> list = context.car_expense;

            if (filters != null && (filters.Filters != null && filters.Filters.Count > 0))
            {
                filters.FormatFieldToUnderscore();
                GridHelper.ProcessFilters<car_expense>(filters, ref list);
            }

            if (sortings != null && sortings.Count > 0)
            {
                foreach (var s in sortings)
                {
                    s.FormatSortOnToUnderscore();
                    list = list.OrderBy<car_expense>(s.SortOn + " " + s.SortOrder);
                }
            }
            else
            {
                list = list.OrderBy<car_expense>("id desc"); //default, wajib ada atau EF error
            }

            return list;
        }

        public List<car_expense> FindAll(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null)
        {
            IQueryable<car_expense> list = IQCarExpense(sortings, filters);

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
            List<car_expense> result = takeList.ToList();
            return result;
        }

        public car_expense FindByPk(System.Guid id)
        {
            return context.car_expense.Find(id);
        }

        public int Count(FilterInfo filters = null)
        {
            IQueryable<car_expense> items = IQCarExpense(null, filters);

            return items.Count();
        }

        public void Save(car_expense dbItem)
        {
            if (dbItem.id == Guid.Empty) //create
            {
                dbItem.id = Guid.NewGuid();

                context.car_expense.Add(dbItem);
            }
            else //edit
            {
                var entry = context.Entry(dbItem);
                entry.State = EntityState.Modified;
            }
            context.SaveChanges();
        }

        public void Delete(car_expense dbItem)
        {
            context.car_expense.Remove(dbItem);
            context.SaveChanges();
        }

        #endregion
    }
}
