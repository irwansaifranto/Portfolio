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
    public class EFCustomerRepository : ICustomerRepository
    {
		private Entities.Entities context = new Entities.Entities();

        #region customer

        public List<customer> FindAll(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null)
        {
            IQueryable<customer> list = context.customers;

            if (filters != null && (filters.Filters != null && filters.Filters.Count > 0))
            {
                filters.FormatFieldToUnderscore();
                GridHelper.ProcessFilters<customer>(filters, ref list);
            }

            if (sortings != null && sortings.Count > 0)
            {
                foreach (var s in sortings)
                {
                    s.FormatSortOnToUnderscore();
                    list = list.OrderBy<customer>(s.SortOn + " " + s.SortOrder);
                }
            }
            else
            {
                list = list.OrderBy<customer>("id desc"); //default, wajib ada atau EF error
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
            List<customer> result = takeList.ToList();
            return result;
        }

        public customer FindByPk(System.Guid id)
        {
            return context.customers.Find(id);
        }

        public int Count(FilterInfo filters = null)
        {
            IQueryable<customer> items = context.customers;

            if (filters != null && (filters.Filters != null && filters.Filters.Count > 0))
            {
                GridHelper.ProcessFilters<customer>(filters, ref items);
            }

            return items.Count();
        }

        public void Save(customer dbItem)
        {
            if (dbItem.id == Guid.Empty) //create
            {
                dbItem.id = Guid.NewGuid();
                customer checkUnique = context.customers.Where(x => x.id == dbItem.id).FirstOrDefault();
                while (checkUnique != null)
                {
                    dbItem.id = Guid.NewGuid();
                    checkUnique = context.customers.Where(x => x.id == dbItem.id).FirstOrDefault();
                }
                context.customers.Add(dbItem);
            }
            else //edit
            {
                var entry = context.Entry(dbItem);
                entry.State = EntityState.Modified;
            }
            context.SaveChanges();
        }

        public void Delete(customer dbItem)
        {
            context.customers.Remove(dbItem);
            context.SaveChanges();
        }


        #endregion
	}
}