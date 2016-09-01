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
    public class EFDriverRepository : IDriverRepository
    {
		private Entities.Entities context = new Entities.Entities();

        #region driver

        public List<driver> FindAll(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null)
        {
            IQueryable<driver> list = context.drivers;

            if (filters != null && (filters.Filters != null && filters.Filters.Count > 0))
            {
                filters.FormatFieldToUnderscore();
                GridHelper.ProcessFilters<driver>(filters, ref list);
            }

            if (sortings != null && sortings.Count > 0)
            {
                foreach (var s in sortings)
                {
                    s.FormatSortOnToUnderscore();
                    list = list.OrderBy<driver>(s.SortOn + " " + s.SortOrder);
                }
            }
            else
            {
                list = list.OrderBy<driver>("id desc"); //default, wajib ada atau EF error
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
            List<driver> result = takeList.ToList();
            return result;
        }

        public driver FindByPk(System.Guid id)
        {
            return context.drivers.Find(id);
        }

        public int Count(FilterInfo filters = null)
        {
            IQueryable<driver> items = context.drivers;

            if (filters != null && (filters.Filters != null && filters.Filters.Count > 0))
            {
                GridHelper.ProcessFilters<driver>(filters, ref items);
            }

            return items.Count();
        }

        public void Save(driver dbItem)
        {
            if (dbItem.id == Guid.Empty) //create
            {
                dbItem.id = Guid.NewGuid();
                driver checkUnique = context.drivers.Where(x => x.id == dbItem.id).FirstOrDefault();
                while (checkUnique != null)
                {
                    dbItem.id = Guid.NewGuid();
                    checkUnique = context.drivers.Where(x => x.id == dbItem.id).FirstOrDefault();
                }
                context.drivers.Add(dbItem);
            }
            else //edit
            {
                var entry = context.Entry(dbItem);
                entry.State = EntityState.Modified;
            }
            context.SaveChanges();
        }

        public void Delete(driver dbItem)
        {
            context.drivers.Remove(dbItem);
            context.SaveChanges();
        }


        #endregion
	}
}