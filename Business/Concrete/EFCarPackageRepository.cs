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
    public class EFCarPackageRepository : ICarPackageRepository
    {
        private Entities.Entities context = new Entities.Entities();

        #region car_package

        private IQueryable<car_package> IQCarPackage(List<SortingInfo> sortings = null, FilterInfo filters = null)
        {
            IQueryable<car_package> list = context.car_package;

            if (filters != null && (filters.Filters != null && filters.Filters.Count > 0))
            {
                filters.FormatFieldToUnderscore();
                GridHelper.ProcessFilters<car_package>(filters, ref list);
            }

            if (sortings != null && sortings.Count > 0)
            {
                foreach (var s in sortings)
                {
                    s.FormatSortOnToUnderscore();
                    list = list.OrderBy<car_package>(s.SortOn + " " + s.SortOrder);
                }
            }
            else
            {
                list = list.OrderBy<car_package>("id desc"); //default, wajib ada atau EF error
            }

            return list;
        }

        public List<car_package> FindAll(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null)
        {
            IQueryable<car_package> list = IQCarPackage(sortings, filters);

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
            List<car_package> result = takeList.ToList();
            return result;
        }

        public car_package FindByPk(System.Guid id)
        {
            return context.car_package.Find(id);
        }

        public int Count(FilterInfo filters = null)
        {
            IQueryable<car_package> items = IQCarPackage(null, filters);

            return items.Count();
        }

        public void Save(car_package dbItem)
        {
            if (dbItem.id == Guid.Empty) //create
            {
                dbItem.id = Guid.NewGuid();

                context.car_package.Add(dbItem);
            }
            else //edit
            {
                var entry = context.Entry(dbItem);
                entry.State = EntityState.Modified;
            }
            context.SaveChanges();
        }

        public void Delete(car_package dbItem)
        {
            context.car_package.Remove(dbItem);
            context.SaveChanges();
        }

        #endregion
    }
}
