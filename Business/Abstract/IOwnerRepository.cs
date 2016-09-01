using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using Common.Enums;
using Business.Infrastructure;
using Business.Entities;

namespace Business.Abstract
{
    public interface IOwnerRepository
    {
        #region owner

        List<owner> FindAll(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null);
        owner FindByPk(Guid id);
        int Count(FilterInfo filters = null);
        void Save(owner dbItem);
        void Delete(owner dbItem);

        bool CheckCodeUniqueness(string code);

        owner FindOwnerByUserName(string username);

        #endregion

        #region owner user
        List<owner_user> FindAllUser(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null);
        owner_user FindByPkUser(Guid id);
        int CountUser(FilterInfo filters = null);
        void DeleteUser(owner_user dbItem);
        owner_user SaveUser(owner_user dbItem);

        bool CheckUsername(string username, Guid idOwner);

        #endregion

    }
}