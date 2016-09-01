using Business.Entities;
using Business.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Enums;

namespace Business.Abstract
{
    public interface ILogRepository
    {
        void SaveLogWS(log_ws dbItem);
    }
}
