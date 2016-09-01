using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebUI.Models;

namespace WebUI.Infrastructure
{
    public class OptionGenerator
    {
        #region select list item

        public List<SelectListItem> MonthSelectListItems()
        {
            List<SelectListItem> ret = new List<SelectListItem>();
            foreach (KeyValuePair<int, string> row in this.MonthOptions())
                ret.Add(new SelectListItem { Text = row.Value, Value = row.Key.ToString() });
            return ret;
        }

        public List<SelectListItem> YearSelectListItems()
        {
            List<SelectListItem> ret = new List<SelectListItem>();
            foreach (int row in this.YearOptions())
                ret.Add(new SelectListItem { Text = row.ToString(), Value = row.ToString() });
            return ret;
        }

        #endregion

        #region serialize
        
        public string YearOptionsStr
        {
            get
            {
                return new JavaScriptSerializer().Serialize(this.YearSelectListItems());
            }
        }

        public string MonthOptionsStr
        {
            get
            {
                return new JavaScriptSerializer().Serialize(this.MonthSelectListItems());
            }
        }

        #endregion

        /**
         * mengembalikan [1] => Januari
         */
        public Dictionary<int, string> MonthOptions()
        {
            DateTime date = DateTime.Parse("2000-01-01");
            Dictionary<int, string> options = new Dictionary<int, string>();

            while (date.Year == 2000)
            {
                options[date.Month] = date.ToString("MMMM");
                date = date.AddMonths(1);
            }

            return options;
        }

        /**
         * mengembalikan list dari now.year - 1 s/d now.year + 1
         */
        public List<int> YearOptions()
        {
            DateTime date = DateTime.Now;
            int lastYear = date.Year - 1;
            int nextYear = date.Year + 1;
            List<int> options = new List<int>();

            for (int i = lastYear; i <= nextYear; i++)
                options.Add(i);

            return options;
        }
    }
}