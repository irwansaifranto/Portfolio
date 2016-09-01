using WebUI.Infrastructure;
using WebUI.Models;
using ProjectLog.Entities;
using ProjectLog.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectLog;

namespace WebUI.Controllers
{
    [LogActionFilter]
    [AuthorizeAdmin]
    public class LogActivityController : Controller
    {
        //
        // GET: /LogActivity/

        public ActionResult Index()
        {
            return View();
        }

        public virtual JsonResult Binding()
        {
            //WebUI.Infrastructure.GridRequestParameters param = WebUI.Infrastructure.GridRequestParameters.Current;
            //ProjectLog.LogItem li = new ProjectLog.LogItem();


            //List<ProjectLog.Infrastructure.SortingInfo> sortList = new List<SortingInfo>();
            
            //if (param.Sortings != null)
            //{
            //    var temp = param.Sortings.ToList();
            //    foreach (Business.Infrastructure.SortingInfo a in temp)
            //    {
            //        sortList.Add(new SortingInfo() {
            //            SortOn = a.SortOn,
            //            SortOrder = a.SortOrder
            //        });
            //    }
            //}

            //ProjectLog.Infrastructure.FilterInfo filterMain = new ProjectLog.Infrastructure.FilterInfo();
            //List<ProjectLog.Infrastructure.FilterInfo> filterList = new List<ProjectLog.Infrastructure.FilterInfo>();
            //if(param.Filters != null)
            //{
            //    var temp2 = param.Filters.Filters.ToList();
            //    foreach (Business.Infrastructure.FilterInfo d in temp2)
            //    {
            //        filterList.Add(new ProjectLog.Infrastructure.FilterInfo() { 
            //            Field = d.Field,
            //            Filters = null,
            //            Logic = d.Logic,
            //            Operator = d.Operator,
            //            Value = d.Value
            //        });
            //    }
            //}
            
            //if (param.Filters != null)
            //{
            //    filterMain.Field = param.Filters.Field;
            //    filterMain.Filters = (filterList.Count > 0?filterList:null);
            //    filterMain.Logic = param.Filters.Logic;
            //    filterMain.Operator = param.Filters.Operator;
            //    filterMain.Value = param.Filters.Value;

            //}



            //List<log> items = li.Find(param.Skip, param.Take, (sortList.Count >0 ?sortList:null), (filterMain != null ? filterMain : null));
            //int total = li.Count((filterMain != null ? filterMain : null));

            //return Json(new { total = total, data = new LogActivityViewModel().MapList(items) });
            GridRequestParametersProjectLog param = GridRequestParametersProjectLog.Current;
            ProjectLog.LogItem li = new ProjectLog.LogItem();


            List<log> items = li.Find(param.Skip, param.Take, (param.Sortings != null ? param.Sortings.ToList() : null), (param.Filters != null ? param.Filters : null));
            int total = li.Count((param.Filters != null ? param.Filters : null));

            return Json(new { total = total, data = new LogActivityViewModel().MapList(items) });
        }


    }

}
