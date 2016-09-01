using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using WebUI.Infrastructure.Abstract;

namespace WebUI.Infrastructure.Concrete
{
    public class DummyAuthProvider : IAuthProvider
    {
        public string Message
        {
            get;
            internal set;
        }

        public bool Authenticate(string username, string password)
        {
            //kamus
            bool isSuccess = false;

            if ((username == "admin") && (password == "admin"))
                isSuccess = true;

            return isSuccess;
        }

        public bool Logout()
        {
            return true;
        }
    }
}