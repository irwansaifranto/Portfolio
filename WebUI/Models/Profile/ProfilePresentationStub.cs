using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Business.Entities;

namespace WebUI.Models.Car
{
    public class ProfilePresentationStub
    {
        // Example model value from scaffolder script: 0
        public System.Guid Id { get; set; }
        public string Logo { get; set; }
        public string Contact { get; set; }
        public string Terms { get; set; }
        
        

        public ProfilePresentationStub() { }

        public ProfilePresentationStub(owner dbItem)
        {
            this.Id = dbItem.id;
            this.Logo = dbItem.logo;
            this.Contact = dbItem.contact;
            this.Terms = dbItem.terms;
         
           
        }

        public List<ProfilePresentationStub> MapList(List<owner> dbItems)
        {
            List<ProfilePresentationStub> retList = new List<ProfilePresentationStub>();

            foreach (owner dbItem in dbItems)
                retList.Add(new ProfilePresentationStub(dbItem));

            return retList;
        }
    }
}

