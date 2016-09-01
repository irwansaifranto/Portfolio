using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Business.Entities;
using Common.Enums;
using SecurityGuard.ViewModels;

namespace WebUI.Models.Driver
{
    public class DriverPasswordFormStub
    {
        
        [StringLength(100, ErrorMessage = "Harus terdiri dari 6 karakter", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password Baru")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public string NewPassword { get; set; }

        
        [DataType(DataType.Password)]
        [Display(Name = "Ulangi Password")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        [System.Web.Mvc.Compare("NewPassword", ErrorMessage = "Password baru dan ulangi password tidak cocok.")]
        public string ConfirmPassword { get; set; }

        public Guid Id { get; set; }
    }
}

