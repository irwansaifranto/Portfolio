using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Business.Entities;
using Common.Enums;

namespace WebUI.Models.Customer
{
    public class CustomerFormStub
    {
		// Example model value from scaffolder script: 0
		[DisplayName("Id")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
		public System.Guid Id { get; set; }

		[DisplayName("Nama")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
		public string Name { get; set; }

		[DisplayName("Tipe")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
		public string CustomerType { get; set; }
        public string CustomerTitle { get; set; }

		[DisplayName("Nomor Telpon")]        
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        [StringLength(16, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
		public string PhoneNumber { get; set; }

        [DisplayName("Nomor Telpon 2")]
        [StringLength(16, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public string PhoneNumber2 { get; set; }
       
		[DisplayName("Alamat")]
        [DataType(DataType.MultilineText)]
		public string Address { get; set; }

		[DisplayName("Kota")]
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
		public string City { get; set; }

		[DisplayName("Email")]
		public string Email { get; set; }

		[DisplayName("Catatan")]
        [DataType(DataType.MultilineText)]
		public string Notes { get; set; }

		[DisplayName("Id Owner")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
		public System.Guid IdOwner { get; set; }

        [DisplayName("Perusahaan")]
        [StringLength(32, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public string Company { get; set; }
        
        [DisplayName("Upload Foto Tamu")]
        public string Photo { get; set; }

		public List<SelectListItem> OwnerOptions { get; set; }

        public List<SelectListItem> TypeOptions { get; set; }

        public List<SelectListItem> TitleOptions { get; set; }

        public List<SelectListItem> NameOptions
        {
            get
            {
                EnumHelper enumHelper = new EnumHelper();

                List<SelectListItem> options = new List<SelectListItem>();
                foreach (CustomerTitle item in enumHelper.EnumToList<CustomerTitle>().ToList())
                    options.Add(new SelectListItem { Value = (item).ToString(), Text = enumHelper.GetEnumDescription(item) });

                return options;
            }
            
        }

		public CustomerFormStub() { }

		public CustomerFormStub(List<Business.Entities.owner> listOwner)
			: this()
		{
			this.FillOwnerOptions(listOwner);
		}


		public CustomerFormStub(customer dbItem,List<Business.Entities.owner> listOwner)
			: this(listOwner)
		{
			this.Id = dbItem.id;
			this.Name = dbItem.name;
			this.CustomerType = dbItem.customer_type;			
			this.Address = dbItem.address;
			this.City = dbItem.city;
			this.Email = dbItem.email;
			this.Notes = dbItem.notes;
			this.IdOwner = dbItem.id_owner;
            this.Company = dbItem.company;
            this.Photo = dbItem.photo;
            this.PhoneNumber = dbItem.phone_number;
            //this.PhoneNumber2 = dbItem.phone_number;
            this.CustomerTitle = dbItem.title;

            if (dbItem.phone_number != null) {
                string[] words = dbItem.phone_number.Split(';');
                this.PhoneNumber = words[0];

                if (words.Count() > 1)
                {
                    this.PhoneNumber2 = words[1];
                }
            }
		}

        public customer GetDbObject(customer dbItem)
        {
            string ph1 = "", ph2 = "";

            if (this.PhoneNumber != null)
                ph1 = this.PhoneNumber.Replace('_', ' ');
            else if (this.PhoneNumber2 != null)
                ph2 = this.PhoneNumber2.Replace('_', ' ');

            dbItem.id = this.Id;
            dbItem.name = this.Name;
            dbItem.customer_type = this.CustomerType;
            dbItem.phone_number = ph1 + ";" + ph2;
            dbItem.address = this.Address;
            dbItem.city = this.City;
            dbItem.email = this.Email;
            dbItem.notes = this.Notes;
            dbItem.id_owner = this.IdOwner;
            dbItem.company = this.Company;
            dbItem.photo = this.Photo;
            dbItem.title = this.CustomerTitle;

            return dbItem;
        }

		#region options

		public void FillOwnerOptions(List<Business.Entities.owner> list)
		{
			OwnerOptions = new List<SelectListItem>();
			OwnerOptions.Add(new SelectListItem { Text = "Choose One", Value = "" });
			foreach (Business.Entities.owner item in list)
            {
                OwnerOptions.Add(new SelectListItem { Text = item.name, Value = item.id.ToString() });
            }
		
		}

        public void FillTypeOptions()
        {
            TypeOptions = new List<SelectListItem>();
            TypeOptions.Add(new SelectListItem { Text = "Pilih", Value = "" });
            EnumHelper enumHelper = new EnumHelper();
            foreach (CustomerType item in enumHelper.EnumToList<CustomerType>().ToList())
            {
                TypeOptions.Add(new SelectListItem { Value = item.ToString(), Text = enumHelper.GetEnumDescription(item) });
            }

        }

        public void FillTitleOptions()
        {
            TitleOptions = new List<SelectListItem>();
            TitleOptions.Add(new SelectListItem { Text = "Pilih", Value = "" });
            EnumHelper enumHelper = new EnumHelper();
            foreach (CustomerTitle item in enumHelper.EnumToList<CustomerTitle>().ToList())
            {
                TitleOptions.Add(new SelectListItem { Value = item.ToString(), Text = enumHelper.GetEnumDescription(item) });
            }

        }
		#endregion
	}
}

