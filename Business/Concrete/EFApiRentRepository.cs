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
    public class EFApiRentRepository : IApiRentRepository
    {
        private Entities.Entities context = new Entities.Entities();

        private IQueryable<api_rent> IQApiRent(List<SortingInfo> sortings = null, FilterInfo filters = null)
        {
            //kamus
            IQueryable<api_rent> list = context.api_rent;
            string dateParam = null;
            FilterInfo copyFilters = null;

            //algoritma
            if (filters != null)
                copyFilters = filters.Clone();

            if (copyFilters != null && (copyFilters.Field != null || copyFilters.Filters != null))
            {
                copyFilters.FormatFieldToUnderscore();

                GridHelper.ProcessFilters<api_rent>(copyFilters, ref list);
            }

            if (sortings != null && sortings.Count > 0)
            {
                foreach (var s in sortings)
                {
                    s.FormatSortOnToUnderscore();
                    list = list.OrderBy<api_rent>(s.SortOn + " " + s.SortOrder);
                }
            }
            else
            {
                list = list.OrderBy<api_rent>("id desc"); //default, wajib ada atau EF error
            }

            return list;
        }

        #region api_rent

        public List<api_rent> FindAll(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null)
        {
            IQueryable<api_rent> list = IQApiRent(sortings, filters);

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
            List<api_rent> result = takeList.ToList();
            return result;
        }

        public api_rent Find(FilterInfo filters)
        {
            IQueryable<api_rent> list = IQApiRent(null, filters);

            return list.FirstOrDefault();
        }

        public api_rent FindByPk(System.Guid id)
        {
            return context.api_rent.Find(id);
        }

        public int Count(FilterInfo filters = null)
        {
            IQueryable<api_rent> items = IQApiRent(null, filters);

            return items.Count();
        }

        public Guid Save(api_rent dbItem)
        {
            //kamus
            api_rent checkUnique;
            log_api_rent log;

            //algoritma
            if (dbItem.id == Guid.Empty) //create
            {
                dbItem.id = Guid.NewGuid();
                checkUnique = FindByPk(dbItem.id);

                while (checkUnique != null)
                {
                    dbItem.id = Guid.NewGuid();
                    checkUnique = FindByPk(dbItem.id);
                }

                context.api_rent.Add(dbItem);
            }
            else //edit
            {
                var entry = context.Entry(dbItem);
                entry.State = EntityState.Modified;
            }

            //save log_api_rent
            log = new log_api_rent
            {
                id = Guid.NewGuid(),
                id_api_rent = dbItem.id,
                created_time = DateTimeOffset.Now
            };
            if (dbItem.cancellation_status != null)
                log.status = dbItem.cancellation_status;
            else
                log.status = dbItem.status;

            context.log_api_rent.Add(log);

            context.SaveChanges();

            return dbItem.id;
        }

        public void Delete(api_rent dbItem)
        {
            context.api_rent.Remove(dbItem);
            context.SaveChanges();
        }

        #endregion
    }
}
