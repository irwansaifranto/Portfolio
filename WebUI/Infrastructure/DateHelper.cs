using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.Infrastructure
{
    public class DateHelper
    {
        /**
         * mengambil date paling kecil
         * @param dates mungkin list kosong
         * @return null kalau dates adalah list kosong
         */
        public DateTime? MinDate(List<DateTime> dates)
        {
            DateTime? min = null;

            foreach (DateTime date in dates)
            {
                if (min == null)
                    min = date;
                else
                {
                    if (date < min)
                        min = date;
                }
            }

            return min;
        }

        /**
         * mengambil date paling besar
         * @param dates mungkin list kosong
         * @return null kalau dates adalah list kosong
         */
        public DateTime? MaxDate(List<DateTime> dates)
        {
            DateTime? max = null;

            foreach (DateTime date in dates)
            {
                if (max == null)
                    max = date;
                else
                {
                    if (date > max)
                        max = date;
                }
            }

            return max;
        }
    }
}