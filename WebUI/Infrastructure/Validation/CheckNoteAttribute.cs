using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace WebUI.Infrastructure.Validation
{
    public class CheckNoteAttribute : ValidationAttribute
    {
        public CheckNoteAttribute(string status, params string[] propertyNames)
        {
            this.Status = status;
        }

        public string Status { get; private set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            Object instance = validationContext.ObjectInstance;
            Type type = instance.GetType();
            Object propertyvalue = type.GetProperty(Status).GetValue(instance, null);

            if (propertyvalue != null)
            {

                if (propertyvalue.ToString() == InvoiceStatus.CANCEL.ToString())
                {
                    if (value == null)
                    {
                        return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
                    }
                }
            }
            return null;
        }
    }


}