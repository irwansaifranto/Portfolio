using Business.Abstract;
using Business.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebUI.Extension;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebUI.Controllers;
using WebUI.Infrastructure;
using Common.Enums;
using Business.Infrastructure;
using WebUI.Models;
using System.IO;

namespace WebUI.Controllers
{
    public class LogController : MyController
    {
        public LogController(ILogRepository repoLog)
            : base(repoLog)
        {
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Download()
        {
            DisplayFormatHelper dfh = new DisplayFormatHelper();
            List<log> logs = RepoLog.Find();
            LogPresentationStub stub = new LogPresentationStub();
            MemoryStream ms = stub.GenerateExcel(stub.MapList(logs));
            string filename = string.Format("log {0}.xlsx", DateTime.Now.ToString(dfh.SqlDateFormat));

            return File(ms.ToArray(), "application/vns.ms-excel", filename);
        }

        [HttpPost]
        public void Truncate()
        {
            RepoLog.Truncate();
        }

        #region binding

        public string Binding()
        {
            GridRequestParameters param = GridRequestParameters.Current;

            Business.Infrastructure.FilterInfo filters = new Business.Infrastructure.FilterInfo
            {
                Field = "Action",
                Operator = "startswith",
                Value = "/Log"
            };
            //param.Filters = filters;
            List<log> items = RepoLog.Find(param.Skip, param.Take, (param.Sortings != null ? param.Sortings.ToList() : null), (param.Filters != null ? param.Filters : null));
            int total = RepoLog.Count(param.Filters);

            return new JavaScriptSerializer().Serialize(new { total = total, data = new LogPresentationStub().MapList(items) });
        }

        #endregion
    }
}
