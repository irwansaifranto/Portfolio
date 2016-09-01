using Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebUI.Areas.Report.Models
{
    public class DailyMonthlyFilterModel : DailyFilterModel
    {
        [DisplayName("Tipe")]
        public ReportType ReportType { get; set; }
        public DateTime StartMonth { get; set; }
        public DateTime EndMonth { get; set; }

        #region options

        public List<SelectListItem> GetReportTypeOptions()
        {
            EnumHelper enumHelper = new EnumHelper();

            List<SelectListItem> options = new List<SelectListItem>();
            foreach (ReportType item in enumHelper.EnumToList<ReportType>().ToList())
                options.Add(new SelectListItem { Value = (item).ToString(), Text = enumHelper.GetEnumDescription(item) });

            options = options.OrderBy(m => m.Text).ToList();

            return options;
        }

        #endregion

        public DailyMonthlyFilterModel() : base()
        {
            ReportType = Common.Enums.ReportType.MONTHLY;
            DateTime now = DateTime.Now;
            EndMonth = new DateTime(now.Year, now.Month, 1);
            
            StartMonth = new DateTime(now.Year, now.Month, 1);
            StartMonth = StartMonth.AddMonths(-3);
        }

        public DailyMonthlyFilterModel(DateTime startDate, DateTime endDate) : base (startDate, endDate)
        {
        }
    }
}