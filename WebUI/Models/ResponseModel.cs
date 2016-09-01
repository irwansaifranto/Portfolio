using Business.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Common.Enums;
using WebUI.Infrastructure.Validation;

namespace WebUI.Models
{
    /// <summary>
    /// model untuk mengembalikan response ke client side via ajax
    /// </summary>
    public class ResponseModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public ResponseModel() { }

        public ResponseModel(bool success)
        {
            Success = success;
        }

        public void SetFail(string message)
        {
            Success = false;
            Message = message;
        }
    }
}