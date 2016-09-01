using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Common.Enums;
using System.ComponentModel.DataAnnotations;
using WebUI.Models;

namespace WebUI.Infrastructure.Validation
{
    public class DateOffsetGreaterAttribute : ValidationAttribute
    {
        public string SmallDate { get; private set; }
        private string SmallDateDisplayName { get; set; }

        public DateOffsetGreaterAttribute(string smallDate, params string[] propertyNames)
        {
            this.SmallDate = smallDate;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                DateTimeOffset bigDate = (DateTimeOffset)value;
                //DateTime smallDate = DateTime.Parse(((TaskFormStub)validationContext.ObjectInstance).StartDate);
                var basePropertyInfo = validationContext.ObjectType.GetProperty(SmallDate);
                DateTimeOffset smallDate = (DateTimeOffset)basePropertyInfo.GetValue(validationContext.ObjectInstance, null);

                if (bigDate < smallDate)
                {
                    var displayAttribute = basePropertyInfo.GetCustomAttributes(typeof(System.ComponentModel.DisplayNameAttribute), true).
                        FirstOrDefault() as System.ComponentModel.DisplayNameAttribute;
                    if (displayAttribute != null)
                        this.SmallDateDisplayName = displayAttribute.DisplayName;

                    return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
                }
            }
            return null;
        }

        public override string FormatErrorMessage(string name)
        {
            string displayName;
            if (this.SmallDateDisplayName == null)
                displayName = SmallDate;
            else
                displayName = SmallDateDisplayName;

            return string.Format(Resources.MyGlobalErrors.DateGreater, name, displayName);
        }
    }
}