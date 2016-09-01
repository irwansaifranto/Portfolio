using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Business.Entities;

namespace SecurityGuard.ViewModels
{
    public class ManageUserViewModel
    {
        public int guid { get; set; }
        public string userName { get; set; }

        public ManageUserViewModel() { }
        public ManageUserViewModel(users user)
        {
            this.guid = user.user_id;
            this.userName = user.user_name;
        }
    }
}