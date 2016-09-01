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
    public class OwnerUserFormStub
    {
		// Example model value from scaffolder script: 0
		[DisplayName("Id")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
		public System.Guid Id { get; set; }


		[DisplayName("Nama Perusahaan")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
		public Guid IdOwner { get; set; }

        [DisplayName("Username")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public string Username { get; set; }

		public OwnerUserFormStub() { }


		public OwnerUserFormStub(owner_user dbItem)
			: this()
		{
			this.Id = dbItem.id;
            this.IdOwner = dbItem.id_owner;
            this.Username = dbItem.username;
		}

		public owner_user GetDbObject(owner_user dbItem) {
			dbItem.id = this.Id;
            dbItem.id_owner = this.IdOwner;
            dbItem.username = this.Username;
			return dbItem;
		}

		#region options


		#endregion

	}
}

