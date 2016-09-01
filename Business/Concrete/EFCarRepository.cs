using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Net;
using Business.Infrastructure;
using Business.Linq;
using Business.Entities;
using Business.Abstract;

namespace Business.Concrete
{
    public class EFCarRepository : ICarRepository
    {
		private Entities.Entities context = new Entities.Entities();

        private IQueryable<car> IQCar(List<SortingInfo> sortings = null, FilterInfo filters = null)
        {
            //kamus
            IQueryable<car> list = context.cars;
            FilterInfo copyFilters = null;
            string sort = "";
            List<string> sortArr = new List<string>();

            //algoritma
            if (filters != null)
                copyFilters = filters.Clone();

            if (copyFilters != null && (copyFilters.Field != null || copyFilters.Filters != null))
            {
                copyFilters.FormatFieldToUnderscore();

                GridHelper.ProcessFilters<car>(copyFilters, ref list);
            }

            if (sortings != null && sortings.Count > 0)
            {
                foreach (var s in sortings)
                {
                    s.FormatSortOnToUnderscore();
                    sortArr.Add(s.SortOn + " " + s.SortOrder);
                }

                sort = string.Join(",", sortArr);
                list = list.OrderBy<car>(sort);
            }
            else
            {
                list = list.OrderBy<car>("id desc"); //default, wajib ada atau EF error
            }

            return list;
        }

        #region car

        public List<car> FindAll(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null)
        {
            IQueryable<car> list = IQCar(sortings, filters);

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
            List<car> result = takeList.ToList();
            return result;
        }

        public car Find(List<SortingInfo> sortings = null, FilterInfo filters = null)
        {
            IQueryable<car> list = IQCar(sortings, filters);

            //return result
            //var sql = takeList.ToString();

            return list.FirstOrDefault();
        }

        public car FindByPk(System.Guid id)
        {
            return context.cars.Find(id);
        }

        public int Count(FilterInfo filters = null)
        {
            IQueryable<car> items = IQCar(null, filters);

            return items.Count();
        }

        public void Save(car dbItem)
        {
            if (dbItem.id == Guid.Empty) //create
            {
                dbItem.id = Guid.NewGuid();
                car checkUnique = context.cars.Where(x => x.id == dbItem.id).FirstOrDefault();
                while (checkUnique != null)
                {
                    dbItem.id = Guid.NewGuid();
                    checkUnique = context.cars.Where(x => x.id == dbItem.id).FirstOrDefault();
                }
                context.cars.Add(dbItem);
            }
            else //edit
            {
                var entry = context.Entry(dbItem);
                entry.State = EntityState.Modified;
            }
            context.SaveChanges();
        }

        public void Delete(car dbItem)
        {
            context.cars.Remove(dbItem);
            context.SaveChanges();
        }

        #endregion
	}
}