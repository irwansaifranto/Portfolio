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
    public class EFCarBrandRepository : ICarBrandRepository
    {
        private Entities.Entities context = new Entities.Entities();

        #region car_brand

        public List<car_brand> FindAll(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null)
        {
            IQueryable<car_brand> list = context.car_brand;

            if (filters != null && (filters.Filters != null && filters.Filters.Count > 0))
            {
                filters.FormatFieldToUnderscore();
                GridHelper.ProcessFilters<car_brand>(filters, ref list);
            }

            if (sortings != null && sortings.Count > 0)
            {
                foreach (var s in sortings)
                {
                    s.FormatSortOnToUnderscore();
                    list = list.OrderBy<car_brand>(s.SortOn + " " + s.SortOrder);
                }
            }
            else
            {
                list = list.OrderBy<car_brand>("id desc"); //default, wajib ada atau EF error
            }

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
            List<car_brand> result = takeList.ToList();
            return result;
        }

        public car_brand FindByPk(System.Guid id)
        {
            return context.car_brand.Find(id);
        }

        public int Count(FilterInfo filters = null)
        {
            IQueryable<car_brand> items = context.car_brand;

            if (filters != null && (filters.Filters != null && filters.Filters.Count > 0))
            {
                GridHelper.ProcessFilters<car_brand>(filters, ref items);
            }

            return items.Count();
        }

        public void Save(car_brand dbItem)
        {
            if (dbItem.id == Guid.Empty) //create
            {
                dbItem.id = Guid.NewGuid();
                car_brand checkUnique = context.car_brand.Where(x=>x.id == dbItem.id).FirstOrDefault();
                while (checkUnique != null)
                {
                    dbItem.id = Guid.NewGuid();
                    checkUnique = context.car_brand.Where(x => x.id == dbItem.id).FirstOrDefault();
                }
                context.car_brand.Add(dbItem);
            }
            else //edit
            {
                var entry = context.Entry(dbItem);
                entry.State = EntityState.Modified;
            }
            context.SaveChanges();
        }

        public void Delete(car_brand dbItem)
        {
            context.car_brand.Remove(dbItem);
            context.SaveChanges();
        }


        #endregion
	}
}