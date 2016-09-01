using Business.Abstract;
using Business.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebUI.Controllers
{
    public class TestController : Controller
    {
        private ILogRepository RepoLog;

        public TestController(ILogRepository repoLog)
        {
            RepoLog = repoLog;
        }

        public int Test1()
        {
            //Business.Infrastructure.FilterInfo filters = new Business.Infrastructure.FilterInfo
            //{
            //    Field = "application",
            //    Operator = "eq",
            //    Value = "Test"
            //};

            //Business.Infrastructure.FilterInfo filters = new Business.Infrastructure.FilterInfo
            //{
            //    Filters = new List<Business.Infrastructure.FilterInfo>
            //    {
            //        new Business.Infrastructure.FilterInfo { Field = "application", Operator = "eq", Value = "Test"},
            //        new Business.Infrastructure.FilterInfo { Field = "user", Operator = "eq", Value = "chandra"}
            //    },
            //    Logic = "or"
            //};

            //Business.Infrastructure.FilterInfo filters = new Business.Infrastructure.FilterInfo
            //{
            //    Filters = new List<Business.Infrastructure.FilterInfo>
            //    {
            //        new Business.Infrastructure.FilterInfo
            //        {
            //            Filters = new List<Business.Infrastructure.FilterInfo>
            //            {
            //                new Business.Infrastructure.FilterInfo { Field = "user", Operator = "eq", Value = "chandra"},
            //                new Business.Infrastructure.FilterInfo { Field = "user", Operator = "eq", Value = "yosef"}
            //            },
            //            Logic = "or"
            //        },                    
            //        new Business.Infrastructure.FilterInfo { Field = "application", Operator = "eq", Value = "Minibank"}
            //    },
            //    Logic = "and"
            //};

            //Business.Infrastructure.FilterInfo filters = new Business.Infrastructure.FilterInfo
            //{
            //    Filters = new List<Business.Infrastructure.FilterInfo>
            //    {
            //        new Business.Infrastructure.FilterInfo
            //        {
            //            Filters = new List<Business.Infrastructure.FilterInfo>
            //            {
            //                new Business.Infrastructure.FilterInfo { Field = "user", Operator = "eq", Value = "chandra"},
            //                new Business.Infrastructure.FilterInfo { Field = "user", Operator = "eq", Value = "yosef"}
            //            },
            //            Logic = "or"
            //        },
            //        new Business.Infrastructure.FilterInfo
            //        {
            //            Filters = new List<Business.Infrastructure.FilterInfo>
            //            {
            //                new Business.Infrastructure.FilterInfo { Field = "application", Operator = "eq", Value = "Test"},
            //                new Business.Infrastructure.FilterInfo { Field = "application", Operator = "eq", Value = "Test1"}
            //            },
            //            Logic = "or"
            //        }
            //    },
            //    Logic = "and"
            //};

            Business.Infrastructure.FilterInfo filters = new Business.Infrastructure.FilterInfo
            {
                Filters = new List<Business.Infrastructure.FilterInfo>
                {
                    new Business.Infrastructure.FilterInfo
                    {
                        Filters = new List<Business.Infrastructure.FilterInfo>
                        {
                            new Business.Infrastructure.FilterInfo { Field = "user", Operator = "eq", Value = "chandra"},
                            new Business.Infrastructure.FilterInfo { Field = "application", Operator = "eq", Value = "Test"}
                        },
                        Logic = "and"
                    },
                    new Business.Infrastructure.FilterInfo
                    {
                        Filters = new List<Business.Infrastructure.FilterInfo>
                        {
                            new Business.Infrastructure.FilterInfo { Field = "user", Operator = "eq", Value = "yosef"},
                            new Business.Infrastructure.FilterInfo { Field = "application", Operator = "eq", Value = "Minibank"}
                        },
                        Logic = "and"
                    }
                },
                Logic = "or"
            };

            List<log> l = RepoLog.FindAll(null, null, null, filters);

            return l.Count();
        }

    }
}
