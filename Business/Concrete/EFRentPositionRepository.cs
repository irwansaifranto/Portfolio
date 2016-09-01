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
    public class EFRentPositionRepository : IRentPositionRepository
    {
        private Entities.Entities context = new Entities.Entities();

        private IQueryable<rent_position> IQRentPos(List<SortingInfo> sortings = null, FilterInfo filters = null)
        {
            //kamus
            IQueryable<rent_position> list = context.rent_position;
            string dateParam = null;
            FilterInfo copyFilters = null;

            //algoritma
            if (filters != null)
                copyFilters = filters.Clone();

            if (copyFilters != null)
            {
                copyFilters.FormatFieldToUnderscore();

                dateParam = copyFilters.RemoveFilterField("filter_date");

                GridHelper.ProcessFilters<rent_position>(copyFilters, ref list);
            }

            //menangani date filter
            if (dateParam != null)
            {
                DateTime dt = DateTime.Parse(dateParam);
                dt = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
                DateTimeOffset dto = dt;

                int diffHours = TimeZoneInfo.Local.BaseUtcOffset.Hours;
                int minutes = diffHours * 60;


                //list = list.Where(m =>
                //    dto >= DbFunctions.TruncateTime(DbFunctions.AddMinutes(m.start_rent, minutes)) &&
                //    DbFunctions.TruncateTime(DbFunctions.AddMinutes(m.finish_rent, minutes)) >= dto);
            }

            if (sortings != null && sortings.Count > 0)
            {
                foreach (var s in sortings)
                {
                    s.FormatSortOnToUnderscore();
                    list = list.OrderBy<rent_position>(s.SortOn + " " + s.SortOrder);
                }
            }
            else
            {
                list = list.OrderBy<rent_position>("id desc"); //default, wajib ada atau EF error
            }

            return list;
        }



        public List<rent_position> FindAll(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null)
        {
            IQueryable<rent_position> list = IQRentPos(sortings, filters);

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
            List<rent_position> result = takeList.ToList();
            return result;
        }

        public rent_position FindByPk(System.Guid id)
        {
            return context.rent_position.Find(id);
        }

        public int Count(FilterInfo filters = null)
        {
            IQueryable<rent_position> items = IQRentPos(null, filters);

            return items.Count();
        }

        public void Save(rent_position dbItem)
        {

            if (dbItem.id == Guid.Empty) //create
            {
                dbItem.id = Guid.NewGuid();

                context.rent_position.Add(dbItem);
            }
            else //edit
            {
                context.rent_position.Attach(dbItem);

                var entry = context.Entry(dbItem);
                entry.State = EntityState.Modified;

                //field yang tidak ditentukan oleh user
                //entry.Property(e => e.is_delete).IsModified = false;
            }
            context.SaveChanges();
        }

    }  
}