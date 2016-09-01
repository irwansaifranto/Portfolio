using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Common.Enums;
using System.ComponentModel.DataAnnotations;
using WebUI.Models;

namespace WebUI.Infrastructure.Validation
{
    public class IntegerNotEqualAttribute : ValidationAttribute
    {
        public string Comparator { get; private set; }
        private string ComparatorDisplayName { get; set; }

        public IntegerNotEqualAttribute(string comparator, params string[] propertyNames)
        {
            this.Comparator = comparator;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                int? myValue = value as int?;
                var basePropertyInfo = validationContext.ObjectType.GetProperty(Comparator);
                int? comparator = basePropertyInfo.GetValue(validationContext.ObjectInstance, null) as int?;
                if (comparator != null)
                {
                    if (myValue == comparator)
                    {
                        var displayAttribute = basePropertyInfo.GetCustomAttributes(typeof(System.ComponentModel.DisplayNameAttribute), true).
                            FirstOrDefault() as System.ComponentModel.DisplayNameAttribute;
                        if (displayAttribute != null)
                            this.ComparatorDisplayName = displayAttribute.DisplayName;

                        return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
                    }
                }
            }
            return null;
        }

        public override string FormatErrorMessage(string name)
        {
            string displayName;
            if (this.ComparatorDisplayName == null)
                displayName = Comparator;
            else
                displayName = ComparatorDisplayName;

            return string.Format(Resources.MyGlobalErrors.NotEqual, name, displayName);
        }
    }
}