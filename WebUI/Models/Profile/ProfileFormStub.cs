using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Business.Entities;

namespace WebUI.Models.Profile
{
    public class ProfileFormStub
    {
        // Example model value from scaffolder script: 0
        [DisplayName("Owner Id")]
        //[MaxLength(50)] digunakan untuk batas pada tipe string
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public System.Guid Id { get; set; }


        [DisplayName("Logo")]       
        public string Logo { get; set; }

        [DisplayName("Kontak Perusahaan")]       
        [DataType(DataType.MultilineText)]
        [AllowHtml]
        public string Contact { get; set; }

        [DisplayName("Terms & condition")]        
        [DataType(DataType.MultilineText)]
        [AllowHtml]
        public string Terms { get; set; }
      
    


        public ProfileFormStub() { }

    	public ProfileFormStub(owner dbItem)
			
        {
            Id = dbItem.id;
            Logo = dbItem.logo;
            Contact = dbItem.contact;
            Terms = dbItem.terms;
           
        }

        public owner GetProfile(owner dbItem, Guid idOwner)
        {
            dbItem.id = this.Id;
            dbItem.logo = this.Logo;
            dbItem.contact = this.Contact;
            dbItem.terms = this.Terms;
            
            return dbItem;
            
        }

        #region options


        #endregion

    }
}

