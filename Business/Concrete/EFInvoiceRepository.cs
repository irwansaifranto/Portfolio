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
    public class EFInvoiceRepository : IInvoiceRepository
    {
		private Entities.Entities context = new Entities.Entities();

        private IQueryable<invoice> IQInvoice(List<SortingInfo> sortings = null, FilterInfo filters = null)
        {
            IQueryable<invoice> list = context.invoices;

            if (filters != null && (filters.Filters != null && filters.Filters.Count > 0))
            {
                filters.FormatFieldToUnderscore();
                GridHelper.ProcessFilters<invoice>(filters, ref list);
            }

            if (sortings != null && sortings.Count > 0)
            {
                foreach (var s in sortings)
                {
                    s.FormatSortOnToUnderscore();
                    list = list.OrderBy<invoice>(s.SortOn + " " + s.SortOrder);
                }
            }
            else
            {
                list = list.OrderBy<invoice>("id desc"); //default, wajib ada atau EF error
            }

            return list;
        }

        #region invoice

        public List<invoice> FindAll(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null)
        {
            IQueryable<invoice> list = IQInvoice(sortings, filters);

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
            List<invoice> result = takeList.ToList();
            return result;
        }

        public invoice FindByPk(System.Guid id)
        {
            return context.invoices.Find(id);
        }

        public int Count(FilterInfo filters = null)
        {
            IQueryable<invoice> items = IQInvoice(null, filters);

            return items.Count();
        }

        public void Save(invoice dbItem)
        {
            if (dbItem.id == Guid.Empty) //create
            {
                dbItem.id = Guid.NewGuid();
                invoice checkUnique = context.invoices.Where(x => x.id == dbItem.id).FirstOrDefault();
                while (checkUnique != null)
                {
                    dbItem.id = Guid.NewGuid();
                    checkUnique = context.invoices.Where(x => x.id == dbItem.id).FirstOrDefault();
                }
                context.invoices.Add(dbItem);
            }
            else //edit
            {
                var entry = context.Entry(dbItem);
                entry.State = EntityState.Modified;
            }
            context.SaveChanges();
        }

        public void Delete(invoice dbItem)
        {
            context.invoices.Remove(dbItem);
            context.SaveChanges();
        }

        #endregion

        #region item

        public void SaveItem(invoice_item dbItem)
        {
            if (dbItem.id == Guid.Empty) //create
            {
                dbItem.id = Guid.NewGuid();
                context.invoice_item.Add(dbItem);
            }
            else //edit
            {
                var entry = context.Entry(dbItem);
                entry.State = EntityState.Modified;
            }
            context.SaveChanges();
        }

        public void DeleteItem(invoice_item dbItem)
        {
            context.invoice_item.Remove(dbItem);
            context.SaveChanges();
        }

        #endregion
    }
}