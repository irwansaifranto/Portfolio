using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Business.Entities;

namespace WebUI.Models.Owner
{
    public class OwnerPresentationStub
    {
		// Example model value from scaffolder script: 0
		public System.Guid Id { get; set; }
		public string Code { get; set; }
		public string Name { get; set; }
		public string CreatedBy { get; set; }
		public System.DateTimeOffset CreatedTime { get; set; }
		public string UpdatedBy { get; set; }
		public System.DateTimeOffset? UpdatedTime { get; set; }

        public string Username { get; set; }
        public Guid IdOwner { get; set; }
        public int IdCity { get; set; }
        public string CityName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
		public OwnerPresentationStub() { }

		public OwnerPresentationStub(owner dbItem) { 
			this.Id = dbItem.id;
			this.Code = dbItem.code;
			this.Name = dbItem.name;
			this.CreatedBy = dbItem.created_by;
			this.CreatedTime = dbItem.created_time;
			this.UpdatedBy = dbItem.updated_by;
			this.UpdatedTime = dbItem.updated_time;
            
            // kota
            this.CityName = dbItem.city.name;
            this.Latitude = dbItem.city.latitude;
            this.Longitude = dbItem.city.longitude;
            //this.IdCity = dbItem.id_city;
		}

        public OwnerPresentationStub(owner_user dbItem)
        {
            this.Id = dbItem.id;
            this.IdOwner = dbItem.id_owner;
            this.Username = dbItem.username;
        }

        public OwnerPresentationStub(city dbItem)
        {
            this.IdCity = dbItem.id;
            this.CityName = dbItem.name;
            this.Latitude = dbItem.latitude;
            this.Longitude = dbItem.longitude;
        }

        public OwnerPresentationStub(System.Web.Security.MembershipUser dbItem)
        {
            this.Username = dbItem.UserName;
        }

		public List<OwnerPresentationStub> MapList(List<owner> dbItems)
        {
            List<OwnerPresentationStub> retList = new List<OwnerPresentationStub>();

            foreach (owner dbItem in dbItems)
                retList.Add(new OwnerPresentationStub(dbItem));

            return retList;
        }

        public List<OwnerPresentationStub> MapListCity(List<city> dbItems)
        {
            List<OwnerPresentationStub> retList = new List<OwnerPresentationStub>();
            foreach (city dbItem in dbItems)
                retList.Add(new OwnerPresentationStub(dbItem));
            return retList;
        }

        public List<OwnerPresentationStub> MapListUser(List<owner_user> dbItems)
        {
            List<OwnerPresentationStub> retList = new List<OwnerPresentationStub>();

            foreach (owner_user dbItem in dbItems)
                retList.Add(new OwnerPresentationStub(dbItem));

            return retList;
        }

        public List<OwnerPresentationStub> MapListUsername(System.Web.Security.MembershipUserCollection dbItems)
        {
            List<OwnerPresentationStub> retList = new List<OwnerPresentationStub>();

            foreach (System.Web.Security.MembershipUser dbItem in dbItems)
                retList.Add(new OwnerPresentationStub(dbItem));

            return retList;
        }
	}
}

