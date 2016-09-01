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
    public interface IInvoiceRepository
    {
        List<invoice> FindAll(int? skip = null, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null);
        invoice FindByPk(Guid id);
        int Count(FilterInfo filters = null);
        void Save(invoice dbItem);
        void Delete(invoice dbItem);

        void SaveItem(invoice_item dbItem);
        void DeleteItem(invoice_item dbItem);
    }
}