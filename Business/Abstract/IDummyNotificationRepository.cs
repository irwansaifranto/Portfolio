using Business.Entities;
using Business.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IDummyNotificationRepository
    {
        List<d_notification> FindAll(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null);
        d_notification FindByPk(int id);
        int Count(FilterInfo filters = null);
        void Save(d_notification dbItem);
        void Delete(d_notification dbItem);

    }
}
