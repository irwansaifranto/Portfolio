using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Business.Entities;

namespace WebUI.Models.Owner
{
    public class OwnerFormStub
    {
        // Example model value from scaffolder script: 0
        [DisplayName("Id")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public System.Guid Id { get; set; }

        [DisplayName("Code")]
        [MaxLength(100)]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public string Code { get; set; }

        [DisplayName("Nama Perusahaan")]
        [MaxLength(100)]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public string Name { get; set; }

        [DisplayName("Created By")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public string CreatedBy { get; set; }

        [DisplayName("Created Time")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public System.DateTimeOffset CreatedTime { get; set; }

        [DisplayName("Updated By")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public string UpdatedBy { get; set; }
        // Kota
        [DisplayName("City")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public int? IdCity { get; set; }
        public string CityName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        [DisplayName("Updated Time")]
        public System.DateTimeOffset? UpdatedTime { get; set; }

        public OwnerFormStub() { }

        public OwnerFormStub(owner dbItem)
            : this()
        {
            this.Id = dbItem.id;
            this.Code = dbItem.code;
            this.Name = dbItem.name;
            this.CreatedBy = dbItem.created_by;
            this.CreatedTime = dbItem.created_time;
            this.UpdatedBy = dbItem.updated_by;
            this.UpdatedTime = dbItem.updated_time;
            this.IdCity = (int)dbItem.id_city;
        }

        public owner GetDbObject()
        {
            owner dbItem = new owner();

            dbItem.id = this.Id;
            dbItem.code = this.Code;
            dbItem.name = this.Name;
            dbItem.created_by = this.CreatedBy;
            dbItem.created_time = this.CreatedTime;
            dbItem.updated_by = this.UpdatedBy;
            dbItem.updated_time = this.UpdatedTime;
            dbItem.id_city = this.IdCity.Value;

			return dbItem;
		}

        public void UpdateDbObject(owner dbItem)
        {
            dbItem.id = this.Id;
            dbItem.code = this.Code;
            dbItem.name = this.Name;
            dbItem.updated_by = this.UpdatedBy;
            dbItem.updated_time = this.UpdatedTime;
        }

        #region options


        #endregion

    }
}

