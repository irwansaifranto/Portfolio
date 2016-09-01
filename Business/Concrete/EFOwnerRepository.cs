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
    public class EFOwnerRepository : IOwnerRepository
    {
        private Entities.Entities context = new Entities.Entities();

        private IQueryable<owner> IQOwner()
        {
            return context.owners;
        }

        #region owner

        public List<owner> FindAll(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null)
        {
            IQueryable<owner> list = IQOwner();

            if (filters != null && (filters.Filters != null && filters.Filters.Count > 0))
            {
                filters.FormatFieldToUnderscore();
                GridHelper.ProcessFilters<owner>(filters, ref list);
            }

            if (sortings != null && sortings.Count > 0)
            {
                foreach (var s in sortings)
                {
                    s.FormatSortOnToUnderscore();
                    list = list.OrderBy<owner>(s.SortOn + " " + s.SortOrder);
                }
            }
            else
            {
                list = list.OrderBy<owner>("id desc"); //default, wajib ada atau EF error
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
            List<owner> result = takeList.ToList();
            return result;
        }

        public owner FindByPk(System.Guid id)
        {
            IQueryable<owner> items = IQOwner();

            return items.Where(m => m.id == id).FirstOrDefault();
        }

        public int Count(FilterInfo filters = null)
        {
            IQueryable<owner> items = IQOwner();

            if (filters != null && (filters.Filters != null && filters.Filters.Count > 0))
            {
                GridHelper.ProcessFilters<owner>(filters, ref items);
            }

            return items.Count();
        }

        public void Save(owner dbItem)
        {
            if (dbItem.id == Guid.Empty) //create
            {
                dbItem.id = Guid.NewGuid();
                owner checkUnique = context.owners.Where(x => x.id == dbItem.id).FirstOrDefault();
                while (checkUnique != null)
                {
                    dbItem.id = Guid.NewGuid();
                    checkUnique = context.owners.Where(x => x.id == dbItem.id).FirstOrDefault();
                }
                context.owners.Add(dbItem);
            }
            else //edit
            {
                var entry = context.Entry(dbItem);
                entry.State = EntityState.Modified;
            }
            context.SaveChanges();
        }

        public void Delete(owner dbItem)
        {
            context.owners.Remove(dbItem);
            context.SaveChanges();
        }

        public bool CheckCodeUniqueness(string code)
        {
            bool val = false;
            var check = context.owners.Where(x => x.code == code).FirstOrDefault();
            if(check == null)
            {
                val = true;
            }
            return val;
        }

        public owner FindOwnerByUserName(string username)
        {
            IQueryable<owner> list = IQOwner();

            list = list.Where(m => m.owner_user.Any(ou => ou.username == username));

            return list.FirstOrDefault();
        }


        #endregion

        #region OwnerUser
        public List<owner_user> FindAllUser(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null)
        {
            IQueryable<owner_user> list = context.owner_user;

            if (filters != null && (filters.Filters != null && filters.Filters.Count > 0))
            {
                filters.FormatFieldToUnderscore();
                GridHelper.ProcessFilters<owner_user>(filters, ref list);
            }

            if (sortings != null && sortings.Count > 0)
            {
                foreach (var s in sortings)
                {
                    s.FormatSortOnToUnderscore();
                    list = list.OrderBy<owner_user>(s.SortOn + " " + s.SortOrder);
                }
            }
            else
            {
                list = list.OrderBy<owner_user>("id desc"); //default, wajib ada atau EF error
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
            List<owner_user> result = takeList.ToList();
            return result;
        }

        public owner_user FindByPkUser(System.Guid id)
        {
            IQueryable<owner_user> items = context.owner_user;

            return items.Where(m => m.id == id).FirstOrDefault();
        }

        public int CountUser(FilterInfo filters = null)
        {
            IQueryable<owner_user> items = context.owner_user;

            if (filters != null && (filters.Filters != null && filters.Filters.Count > 0))
            {
                GridHelper.ProcessFilters<owner_user>(filters, ref items);
            }

            return items.Count();
        }

        public owner_user SaveUser(owner_user dbItem)
        {
            if (dbItem.id == Guid.Empty) //create
            {
                dbItem.id = Guid.NewGuid();
                owner_user checkUnique = context.owner_user.Where(x => x.id == dbItem.id).FirstOrDefault();
                while (checkUnique != null)
                {
                    dbItem.id = Guid.NewGuid();
                    checkUnique = context.owner_user.Where(x => x.id == dbItem.id).FirstOrDefault();
                }
                context.owner_user.Add(dbItem);
            }
            else //edit
            {
                var entry = context.Entry(dbItem);
                entry.State = EntityState.Modified;
            }
            context.SaveChanges();
            return dbItem;
        }

        public void DeleteUser(owner_user dbItem)
        {
            context.owner_user.Remove(dbItem);
            context.SaveChanges();
        }


        public bool CheckUsername(string username, Guid idOwner)
        {
            bool result = false;
            var check = (from a in context.owner_user
                         where
                             a.username == username && a.id_owner == idOwner
                         select a).FirstOrDefault();
            if(check == null)
            {
                result = true;
            }

            return result;
        }

        #endregion
    }
}