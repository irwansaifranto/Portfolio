using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Business.Entities;
using Common.Enums;
using System.ComponentModel;

namespace WebUI.Models.Driver
{
    public class DriverPresentationStub
    {
        // Example model value from scaffolder script: 0
        public System.Guid Id { get; set; }

        [DisplayName("Nama")]
        public string Name { get; set; }

        [DisplayName("Tipe")]
        public string DriverType { get; set; }

        [DisplayName("Nomor Telepon")]
        public string PhoneNumber { get; set; }

        [DisplayName("Alamat")]
        public string Address { get; set; }

        [DisplayName("Kota")]
        public string City { get; set; }

        [DisplayName("Email")]
        public string Email { get; set; }
        [DisplayName("Id Owner")]
        public System.Guid IdOwner { get; set; }
        public string OwnerName { get; set; }
        public string TypeName { get; set; }

        [DisplayName("Tanggal Mulai Kerja")]
        public DateTime? WorkStartDate { get; set; }
        [DisplayName("Foto")]
        public string Photo { get; set; }

        [DisplayName("Username")]
        public string Username { get; set; }
        public string DeviceId { get; set; }

        [DisplayName("SIM")]
        public string DriverLicense { get; set; }

        public DriverPresentationStub() { }

        public DriverPresentationStub(driver dbItem)
        {
            //kamus
            EnumHelper eh = new EnumHelper();

            //algoritma
            this.Id = dbItem.id;
            this.Name = dbItem.name;
            this.Username = dbItem.username;
            this.DriverType = dbItem.driver_type;
            this.PhoneNumber = dbItem.phone_number;
            this.Address = dbItem.address;
            this.City = dbItem.city;
            this.Email = dbItem.email;
            this.IdOwner = dbItem.id_owner;
            this.DeviceId = dbItem.device_id;
            this.OwnerName = dbItem.owner != null ? dbItem.owner.name : "";
            this.DriverLicense = dbItem.driver_license_file;
            this.WorkStartDate = dbItem.work_start_date;
            if (dbItem.photo == null)
                this.Photo = VirtualPathUtility.ToAbsolute("~/Content/theme/noimage.png");
            else
                this.Photo = VirtualPathUtility.ToAbsolute(dbItem.photo);

            DriverType type = (DriverType)Enum.Parse(typeof(Common.Enums.DriverType), dbItem.driver_type);
            TypeName = eh.GetEnumDescription(type);
        }

        public List<DriverPresentationStub> MapList(List<driver> dbItems)
        {
            List<DriverPresentationStub> retList = new List<DriverPresentationStub>();

            foreach (driver dbItem in dbItems)
                retList.Add(new DriverPresentationStub(dbItem));

            return retList;
        }
    }
}

