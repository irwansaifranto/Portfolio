using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebUI.Extension
{
    public static class ControllerExtensions
    {
        private const string KEY = "message";

        /**
         * mengembalikan string message yang sudah diformat
         */
        public static string GetMessage(this Controller instance)
        {
            string result = "";
            if (instance.TempData[KEY] != null)
            {
                result = "<div class=\"alert alert-success\"><a class=\"close\" data-dismiss=\"alert\">×</a>" + instance.TempData[KEY].ToString() + "</div>";
            }

            return result;
        }

        /**
         * set message dan template yang digunakan
         * @param template {0} berhasil diubah
         */
        public static void SetMessage(this Controller instance, string value, string template = null, string responseMessage = null)
        {
            string message = "";
            if (template == null)
            {
                message = value;
            }
            else
            {
                message = string.Format(template, value);
            }
            instance.TempData[KEY] = message;
        }
    }
}