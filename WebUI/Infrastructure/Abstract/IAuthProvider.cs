using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebUI.Infrastructure.Abstract
{

    public interface IAuthProvider
    {
        string Message { get; }
        bool Authenticate(string username, string password);
        bool Logout();
    }
}
