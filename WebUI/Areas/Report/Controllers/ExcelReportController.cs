using Business.Abstract;
using Business.Entities;
using MvcSiteMapProvider;
using MvcSiteMapProvider.Web.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebUI.Areas.Report.Models;
using WebUI.Controllers;
using WebUI.Infrastructure;
using WebUI.Infrastructure.Concrete;

namespace WebUI.Areas.Report.Controllers
{
    [AuthorizeUser(ModuleName = "Excel Report")]
    public class ExcelReportController : MyController
    {
        private IRentRepository RepoRent;

        public ExcelReportController(ILogRepository repoLog, IRentRepository repoRent)
            : base(repoLog)
        {
            RepoRent = repoRent;
        }

        //[MvcSiteMapNode(Title = "Penugasan", ParentKey = "Dashboard", Key = "IndexAssignment")]
        [SiteMapTitle("Breadcrumb")]
        public ActionResult Index()
        {
            //kamus
            Guid? idOwner = (User as CustomPrincipal).IdOwner;
            ExcelReportFilterModel model = new ExcelReportFilterModel();
            List<ExcelReportFilterModel> result = new List<ExcelReportFilterModel>();
            List<rent> rents = RepoRent.FindAll().Where(n => n.id_owner == idOwner).ToList();
            //algoritma
            result = model.MapList(rents);
            byte[] excel = new ExcelReportFilterModel().GenerateExcelReport(result);
            string filename = string.Format("Report Data {0}.xlsx", DateTime.Now.ToString("ddMMyyyy"));

            return File(excel, "application/vns.ms-excel", filename);
        }
    }
}
