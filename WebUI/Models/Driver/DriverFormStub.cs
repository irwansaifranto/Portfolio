using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Business.Entities;
using Common.Enums;

namespace WebUI.Models.Driver
{
    public class DriverFormStub
    {
        // Example model value from scaffolder script: 0
        [DisplayName("Id")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public System.Guid Id { get; set; }

        [DisplayName("Nama")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public string Name { get; set; }

        [DisplayName("Username")]        
        public string Username { get; set; }

        [DisplayName("Tipe")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public string DriverType { get; set; }

        [DisplayName("Nomor Telpon")]
        public string PhoneNumber { get; set; }

        [DisplayName("Alamat")]
        [DataType(DataType.MultilineText)]
        public string Address { get; set; }

        [DisplayName("Kota")]
        public string City { get; set; }

        [DisplayName("Email")]
        public string Email { get; set; }

        [DisplayName("Id Owner")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public System.Guid IdOwner { get; set; }

        [DisplayName("Tanggal Mulai Kerja")]
        public DateTime? WorkStartDate { get; set; }

        [DisplayName("Upload Foto Supir")]
        public string Photo { get; set; }

        [DisplayName("SIM")]
        public string DriverLicenseFile { get; set; }

        public string DeviceId { get; set; }
       
        public List<SelectListItem> OwnerOptions { get; set; }

        public List<SelectListItem> TypeOptions { get; set; }

        public List<SelectListItem> WorkStartDateOptions { get; set; }

        public DriverFormStub() { }

        public DriverFormStub(List<Business.Entities.owner> listOwner)
            : this()
        {
            this.FillOwnerOptions(listOwner);
        }

        public DriverFormStub(driver dbItem, List<Business.Entities.owner> listOwner, List<Business.Entities.owner_user> listUser)
            : this(listOwner)
        {
            this.Id = dbItem.id;
            this.Name = dbItem.name;
            this.DriverType = dbItem.driver_type;
            this.PhoneNumber = dbItem.phone_number;
            this.Address = dbItem.address;
            this.City = dbItem.city;
            this.Email = dbItem.email;
            this.IdOwner = dbItem.id_owner;
            this.WorkStartDate = dbItem.work_start_date;
            this.Photo = dbItem.photo;
            this.Username = dbItem.username;
            this.DeviceId = dbItem.device_id;
            this.DriverLicenseFile = dbItem.driver_license_file;
        }

        public driver GetDbObject(driver dbItem)
        {
            dbItem.id = this.Id;
            dbItem.name = this.Name;
            dbItem.driver_type = this.DriverType;
            dbItem.phone_number = this.PhoneNumber;
            dbItem.address = this.Address;
            dbItem.city = this.City;
            dbItem.email = this.Email;
            dbItem.id_owner = this.IdOwner;
            dbItem.work_start_date = this.WorkStartDate;
            dbItem.photo = this.Photo;
            dbItem.username = this.Username;
            dbItem.device_id = this.DeviceId;
            dbItem.driver_license_file = this.DriverLicenseFile;
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
            foreach (DriverType item in enumHelper.EnumToList<DriverType>().ToList())
            {
                TypeOptions.Add(new SelectListItem { Value = item.ToString(), Text = enumHelper.GetEnumDescription(item) });
            }

        }

        public List<SelectListItem> UsernameOptions;
        public List<SelectListItem> GetUsernameOptions()
        {
            return UsernameOptions;
        }
        public void FillUsernameOptions(List<owner_user> dbItems)
        {
            UsernameOptions = new List<SelectListItem>();
            foreach (Business.Entities.owner_user item in dbItems)
            {
                UsernameOptions.Add(new SelectListItem { Text = item.username, Value = item.username });
            }

            UsernameOptions = UsernameOptions.OrderBy(m => m.Text).ToList();
        }

        #endregion

    }
}

