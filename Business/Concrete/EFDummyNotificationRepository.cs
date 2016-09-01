using Business.Abstract;
using Business.Entities;
using Business.Infrastructure;
using Business.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class EFDummyNotificationRepository : IDummyNotificationRepository
    {
        private Entities.Entities context = new Entities.Entities();

        #region customer

        public List<d_notification> FindAll(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null)
        {
            IQueryable<d_notification> list = context.d_notification;

            if (filters != null && (filters.Filters != null && filters.Filters.Count > 0))
            {
                filters.FormatFieldToUnderscore();
                GridHelper.ProcessFilters<d_notification>(filters, ref list);
            }

            if (sortings != null && sortings.Count > 0)
            {
                foreach (var s in sortings)
                {
                    s.FormatSortOnToUnderscore();
                    list = list.OrderBy<d_notification>(s.SortOn + " " + s.SortOrder);
                }
            }
            else
            {
                list = list.OrderBy<d_notification>("id desc"); //default, wajib ada atau EF error
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
            List<d_notification> result = takeList.ToList();
            return result;
        }

        public d_notification FindByPk(int id)
        {
            return context.d_notification.Find(id);
        }

        public int Count(FilterInfo filters = null)
        {
            IQueryable<d_notification> items = context.d_notification;

            if (filters != null && (filters.Filters != null && filters.Filters.Count > 0))
            {
                GridHelper.ProcessFilters<d_notification>(filters, ref items);
            }

            return items.Count();
        }

        public void Save(d_notification dbItem)
        {
            if (dbItem.id == 0) //create
            {
                //penanggulangan data null
                if (context.d_notification.Count() > 0)
                    dbItem.id = context.d_notification.Max(n => n.id)+1;
                else
                    dbItem.id = 1;
                context.d_notification.Add(dbItem);
            }
            else //edit
            {
                var entry = context.Entry(dbItem);
                entry.State = EntityState.Modified;
            }
            context.SaveChanges();
        }

        public void Delete(d_notification dbItem)
        {
            context.d_notification.Remove(dbItem);
            context.SaveChanges();
        }

        #endregion

    }
}
