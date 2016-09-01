using Business.Abstract;
using Business.Entities;
using Business.Infrastructure;
using Business.Linq;
using Common.Enums;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class EFLogRepository : ILogRepository
    {
        private Entities.Entities context = new Entities.Entities();

       

        public void SaveLogWS(log_ws dbItem)
        {
            if (dbItem.id == Guid.Empty)
            {
                dbItem.id = Guid.NewGuid();
                context.log_ws.Add(dbItem);
            }

            context.SaveChanges();
        }
        
    }
}
