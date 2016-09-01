using Business.Abstract;
using Business.Entities;
using Business.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Linq;
using System.Text.RegularExpressions;

namespace Business.Concrete
{
    public class EFUserRepository : IUserRepository
    {
        private Entities.UserManagement context = new Entities.UserManagement();

        public int Count()
        {
            return context.users.Count();
        }
    }
}
