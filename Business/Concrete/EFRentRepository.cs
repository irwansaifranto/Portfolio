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
using Common.Enums;

namespace Business.Concrete
{
    public class EFRentRepository : IRentRepository
    {
		private Entities.Entities context = new Entities.Entities();

        /// <summary>
        /// ada kemungkinan penanganan datetime with offset disamakan dengan fungsi find all yang 1 lagi
        /// filter khusus
        ///     filter_date: mengambil data rent dengan start <= filter_date <= end
        ///     without_invoice: mengambil data rent yang belum memiliki invoice
        ///     include_id: mengambil data rent dengan id = ...
        /// </summary>
        /// <param name="sortings"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        private IQueryable<rent> IQRent(List<SortingInfo> sortings = null, FilterInfo filters = null)
        {
            //kamus
            IQueryable<rent> list = context.rents;
            string dateParam = null;
            string withoutInvoiceParam = null;
            string withoutExpenseParam = null;
            string idParam = null;
            FilterInfo copyFilters = null;

            //algoritma
            if (filters != null)
                copyFilters = filters.Clone();

            if (copyFilters != null && (copyFilters.Field != null || copyFilters.Filters != null))
            {
                copyFilters.FormatFieldToUnderscore();

                dateParam = copyFilters.RemoveFilterField("filter_date");
                withoutInvoiceParam = copyFilters.RemoveFilterField("without_invoice");
                withoutExpenseParam = copyFilters.RemoveFilterField("without_expense");
                idParam = copyFilters.RemoveFilterField("include_id");

                GridHelper.ProcessFilters<rent>(copyFilters, ref list);
            }

            //menangani date filter
            if (dateParam != null)
            {
                DateTime dt = DateTime.Parse(dateParam);
                dt = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
                DateTimeOffset dto = dt;

                int diffHours = TimeZoneInfo.Local.BaseUtcOffset.Hours;
                int minutes = diffHours * 60;

                list = list.Where(m =>
                    dto >= DbFunctions.TruncateTime(DbFunctions.AddMinutes(m.start_rent, minutes)) &&
                    DbFunctions.TruncateTime(DbFunctions.AddMinutes(m.finish_rent, minutes)) >= dto);
            }

            //menangani without invoice
            if (withoutInvoiceParam != null )
            {
                if (idParam == null)
                {
                    list = list.Where(m => m.invoices.Count() == 0);
                }
                else
                {
                    list = list.Where(m => m.id == new Guid(idParam) || m.invoices.Count() == 0);
                }
            }

            //menangani without expense
            if (withoutExpenseParam != null)
            {
                if (idParam == null)
                {
                    list = list.Where(m => m.expenses.Count() == 0);
                }
                else
                {
                    list = list.Where(m => m.id == new Guid(idParam) || m.expenses.Count() == 0);
                }
            }

            if (sortings != null && sortings.Count > 0)
            {
                foreach (var s in sortings)
                {
                    s.FormatSortOnToUnderscore();
                    list = list.OrderBy<rent>(s.SortOn + " " + s.SortOrder);
                }
            }
            else
            {
                list = list.OrderBy<rent>("id desc"); //default, wajib ada atau EF error
            }

            return list;
        }

        #region rent

        /// <summary>
        /// special param
        ///     filter_date
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="sortings"></param>
        /// <param name="copyFilters"></param>
        /// <returns></returns>
        public List<rent> FindAll(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null)
        {
            //kamus
            IQueryable<rent> list;

            //algoritma
            list = IQRent(sortings, filters);

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

            //string sql = takeList.ToString();
            //string fullPath = System.IO.Path.Combine(@"C:\", "/App_Data/debug.txt");
            //System.IO.File.AppendAllText(fullPath, sql + "\n");

            List<rent> result = takeList.ToList();
            return result;
        }

        /// <summary>
        /// input start & finish harus memperhatikan jam dan menit
        /// </summary>
        /// <param name="idOwner"></param>
        /// <param name="start"></param>
        /// <param name="finish"></param>
        /// <returns></returns>
        public List<rent> FindAll(Guid idOwner, DateTime start, DateTime finish, bool includeCancel = false, FilterInfo filters = null)
        {
            IQueryable<rent> list = IQRent(null, filters);

            int diffHours = TimeZoneInfo.Local.BaseUtcOffset.Hours;
            int minutes = diffHours * 60;

            DateTime dtStart = new DateTime(start.Year, start.Month, start.Day, start.Hour, start.Minute, start.Second);
            dtStart = DateTime.SpecifyKind(dtStart, DateTimeKind.Local);
            DateTimeOffset dtoStart = dtStart;

            DateTime dtEnd = new DateTime(finish.Year, finish.Month, finish.Day, finish.Hour, finish.Minute, finish.Second);
            dtEnd = DateTime.SpecifyKind(dtEnd, DateTimeKind.Local);
            DateTimeOffset dtoEnd = dtEnd;

            //list = list.Where(m =>
            //    dtoStart >= DbFunctions.TruncateTime(DbFunctions.AddMinutes(m.start_rent, minutes)) &&
            //    DbFunctions.TruncateTime(DbFunctions.AddMinutes(m.finish_rent, minutes)) >= dtoStart);

            minutes = 0;

            list = list.Where(m => m.id_owner == idOwner && (
                (m.start_rent >= start && m.start_rent <= finish) ||
                (m.finish_rent >= start && m.finish_rent <= finish) ||
                (m.start_rent < start && m.finish_rent > finish)
            ));

            //list = list.Where(m =>
            //    (m.start_rent >= start && m.start_rent <= finish) ||
            //    (m.finish_rent >= start && m.finish_rent <= finish) ||
            //    (m.start_rent < start && m.finish_rent > finish)
            //);           

            if (!includeCancel)
                list = list.Where(m => m.status != RentStatus.CANCEL.ToString());

            //List<DateTimeOffset> temp1 = context.rents.OrderByDescending(m => m.created_time).Select(m => m.start_rent).ToList();
            //List<DateTimeOffset?> temp2 = context.rents.OrderByDescending(m => m.created_time).Select(m => DbFunctions.AddMinutes(m.finish_rent, minutes)).ToList();

            return list.ToList();
        }

        public rent FindByPk(System.Guid id)
        {
            return context.rents.Find(id);
        }

        public rent Find(FilterInfo filters)
        {
            IQueryable<rent> list = IQRent(null, filters);

            return list.FirstOrDefault();
        }

        public int Count(FilterInfo filters = null)
        {
            IQueryable<rent> list = IQRent(null, filters);

            return list.Count();
        }

        public Guid Save(rent dbItem)
        {
            if (dbItem.id == Guid.Empty) //create
            {
                dbItem.id = Guid.NewGuid();

                context.rents.Add(dbItem);
            }
            else //edit
            {
                context.rents.Attach(dbItem);

                var entry = context.Entry(dbItem);
                entry.State = EntityState.Modified;

                //field yang tidak ditentukan oleh user
                //entry.Property(e => e.is_delete).IsModified = false;
            }

            context.SaveChanges();

            return dbItem.id;
        }

        

        public void Delete(rent dbItem)
        {
            context.rents.Remove(dbItem);
            context.SaveChanges();
        }

        public int CountUnassignedCar(FilterInfo filters = null)
        {
            //kamus
            IQueryable<rent> list;

            //algoritma
            list = IQRent(null, filters);
            list = list.Where(m => m.id_car == null);

            return list.Count();
        }

        public int CountUnassignedDriver(FilterInfo filters = null)
        {
            //kamus
            IQueryable<rent> list;

            //algoritma
            list = IQRent(null, filters);
            list = list.Where(m => m.id_driver == null);

            return list.Count();
        }

        public List<car> FindAvailable(Guid idOwner, DateTime start, DateTime finish, string carBrandName, string carModelName, int? capacity)
        {
            //silakan dikoding

            return null;
        }

        #endregion

        #region rent code

        public string GenerateRentCode(owner owner)
        {
            //kamus
            string template = "{0}/{1}";
            string code = "";
            bool success = false;
            rent_code rc;

            //algoritma
            while (!success)
            {
                code = string.Format(template, owner.code, RandomString(5));
                rc = context.rent_code.Find(code);

                if (rc == null)
                {
                    rc = new rent_code { code = code };
                    CreateRentCode(rc);
                    
                    success = true;
                }
            }

            return code;
        }


        private void CreateRentCode(rent_code dbItem)
        {
            context.rent_code.Add(dbItem);
            context.SaveChanges();
        }

        private string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        #endregion

        #region rent_package
        private IQueryable<rent_package> IQRentPackage(List<SortingInfo> sortings = null, FilterInfo filters = null)
        {
            IQueryable<rent_package> list = context.rent_package;

            if (filters != null && (filters.Filters != null && filters.Filters.Count > 0))
            {
                filters.FormatFieldToUnderscore();
                GridHelper.ProcessFilters<rent_package>(filters, ref list);
            }

            if (sortings != null && sortings.Count > 0)
            {
                foreach (var s in sortings)
                {
                    s.FormatSortOnToUnderscore();
                    list = list.OrderBy<rent_package>(s.SortOn + " " + s.SortOrder);
                }
            }
            else
            {
                list = list.OrderBy<rent_package>("id desc"); //default, wajib ada atau EF error
            }

            return list;
        }
        public void SaveListPackage(rent_package dbItem)
        {

            if (dbItem.id == Guid.Empty) //create
            {
                dbItem.id = Guid.NewGuid();
                context.rent_package.Add(dbItem);
            }
            else //edit
            {

                var entry = context.Entry(dbItem);
                entry.State = EntityState.Modified;

                //field yang tidak ditentukan oleh user
                //entry.Property(e => e.is_delete).IsModified = false;
            }

            context.SaveChanges();
        }

        public void DeleteRentPackage(rent_package dbItem)
        {
            context.rent_package.Remove(dbItem);
            context.SaveChanges();
        }

        public List<rent_package> FindAllRentPackage(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null)
        {
            //kamus
            IQueryable<rent_package> list;

            //algoritma
            list = IQRentPackage(sortings, filters);

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

            //string sql = takeList.ToString();
            //string fullPath = System.IO.Path.Combine(@"C:\", "/App_Data/debug.txt");
            //System.IO.File.AppendAllText(fullPath, sql + "\n");

            List<rent_package> result = takeList.ToList();
            return result;
        }
        #endregion
    }
}