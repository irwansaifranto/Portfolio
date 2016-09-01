using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace WebUI.Infrastructure.Validation
{
    public class PositiveIntegerAttribute : ValidationAttribute
    {
        public PositiveIntegerAttribute(params string[] propertyNames)
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                if (((int)value) < 0)
                {
                    return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
                }
            }
            return null;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(Resources.MyGlobalErrors.PositiveInteger, name);
        }
    }
}