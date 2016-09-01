using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace WebUI.Infrastructure.Validation
{
    public class CheckDayAttribute : ValidationAttribute
    {
        public CheckDayAttribute(Day validDay, params string[] propertyNames)
        {
            this.ValidDay = validDay;
        }

        public Day ValidDay { get; private set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                DateTime date = DateTime.Parse((string)value);

                if (date == null)
                {
                    return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
                }
                else
                {
                    int dayOfWeek = (int)date.DayOfWeek;
                    if (dayOfWeek != (int)this.ValidDay)
                    {
                        return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
                    }
                }
            }
            return null;
        }
    }
}