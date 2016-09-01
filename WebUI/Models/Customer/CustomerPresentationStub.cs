using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Business.Entities;
using Common.Enums;

namespace WebUI.Models.Customer
{
    public class CustomerPresentationStub
    {
		// Example model value from scaffolder script: 0
		public System.Guid Id { get; set; }
		public string Name { get; set; }
		public string CustomerType { get; set; }
		public string PhoneNumber { get; set; }
        public string PhoneNumber2 { get; set; }
		public string Address { get; set; }
		public string City { get; set; }
		public string Email { get; set; }
		public string Notes { get; set; }
		public System.Guid IdOwner { get; set; }
		public string OwnerName { get; set; }
        public string TypeName { get; set; }
        public string Company { get; set; }
        public string Photo { get; set; }
        public string CustomerTitleEnum { get; set; }

        public string MergePhoneNumber
        {
            get
            {
                string pn = PhoneNumber + (PhoneNumber2 != null && PhoneNumber2 != "" ? " - " + PhoneNumber2 : "");

                return pn;
            }
        }
		
		public CustomerPresentationStub() { }

		public CustomerPresentationStub(customer dbItem) { 
			this.Id = dbItem.id;
			this.Name = dbItem.name;
			this.CustomerType = dbItem.customer_type;           
			this.Address = dbItem.address;
			this.City = dbItem.city;
			this.Email = dbItem.email;
			this.Notes = dbItem.notes;
			this.IdOwner = dbItem.id_owner;
			this.OwnerName = dbItem.owner != null ? dbItem.owner.name : "";
            this.Company = dbItem.company;
            this.PhoneNumber = dbItem.phone_number;
            this.PhoneNumber2 = dbItem.phone_number;

            if (dbItem.phone_number == null)
            {
                this.PhoneNumber = "";
                this.PhoneNumber2 = "";
            }
            else
            {
                string s = dbItem.phone_number;
                string[] words = s.Split(';');
                this.PhoneNumber = words[0];
                
                if (words.Count() > 1)
                {
                    this.PhoneNumber2 = words[1];
                }
            }

            if (dbItem.photo == null)
                this.Photo = VirtualPathUtility.ToAbsolute("~/Content/theme/noimage.png");
            else
                this.Photo = VirtualPathUtility.ToAbsolute(dbItem.photo);

            EnumHelper eh = new EnumHelper();
            if (dbItem.customer_type != null && dbItem.customer_type != "")
            {
                CustomerType type = (CustomerType)Enum.Parse(typeof(Common.Enums.CustomerType), dbItem.customer_type);
                TypeName = eh.GetEnumDescription(type);
            }

            if (dbItem.title != null && dbItem.title != "")
            {
                CustomerTitleEnum = dbItem.title.ToString();
            }
		}

		public List<CustomerPresentationStub> MapList(List<customer> dbItems)
        {
            List<CustomerPresentationStub> retList = new List<CustomerPresentationStub>();

            foreach (customer dbItem in dbItems)
                retList.Add(new CustomerPresentationStub(dbItem));

            return retList;
        }
	}
}

