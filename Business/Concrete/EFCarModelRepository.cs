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
    public class EFCarModelRepository : ICarModelRepository
    {
        private Entities.Entities context = new Entities.Entities();

        private IQueryable<car_model> IQCarModel(List<SortingInfo> sortings = null, FilterInfo filters = null)
        {
            IQueryable<car_model> list = context.car_model;

            if (filters != null && (filters.Filters != null && filters.Filters.Count > 0))
            {
                filters.FormatFieldToUnderscore();
                GridHelper.ProcessFilters<car_model>(filters, ref list);
            }

            if (sortings != null && sortings.Count > 0)
            {
                foreach (var s in sortings)
                {
                    s.FormatSortOnToUnderscore();
                    list = list.OrderBy<car_model>(s.SortOn + " " + s.SortOrder);
                }
            }
            else
            {
                list = list.OrderBy<car_model>("id desc"); //default, wajib ada atau EF error
            }

            return list;
        }

        #region car_model

        public List<car_model> FindAll(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null)
        {
            IQueryable<car_model> list = IQCarModel(sortings, filters);

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
            List<car_model> result = takeList.ToList();
            return result;
        }

        public car_model Find(List<SortingInfo> sortings = null, FilterInfo filters = null)
        {
            IQueryable<car_model> list = IQCarModel(sortings, filters);

            return list.FirstOrDefault();
        }

        public car_model FindByPk(System.Guid id)
        {
            return context.car_model.Find(id);
        }

        public int Count(FilterInfo filters = null)
        {
            IQueryable<car_model> items = context.car_model;

            if (filters != null && (filters.Filters != null && filters.Filters.Count > 0))
            {
                GridHelper.ProcessFilters<car_model>(filters, ref items);
            }

            return items.Count();
        }

        public void Save(car_model dbItem)
        {
            if (dbItem.id == Guid.Empty) //create
            {
                dbItem.id = Guid.NewGuid();
                car_model checkUnique = context.car_model.Where(x => x.id == dbItem.id).FirstOrDefault();
                while (checkUnique != null)
                {
                    dbItem.id = Guid.NewGuid();
                    checkUnique = context.car_model.Where(x => x.id == dbItem.id).FirstOrDefault();
                }
                context.car_model.Add(dbItem);
            }
            else //edit
            {
                var entry = context.Entry(dbItem);
                entry.State = EntityState.Modified;
            }
            context.SaveChanges();
        }

        public void Delete(car_model dbItem)
        {
            context.car_model.Remove(dbItem);
            context.SaveChanges();
        }



        #endregion
	}
}