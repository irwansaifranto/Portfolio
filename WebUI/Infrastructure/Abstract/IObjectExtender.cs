using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebUI.Infrastructure.Abstract
{
    public interface IObjectExtender
    {
        object Extend(object obj1, object obj2);
    }
}
