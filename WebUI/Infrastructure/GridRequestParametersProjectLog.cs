using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;
using ProjectLog;
using ProjectLog.Infrastructure;

namespace WebUI.Infrastructure
{
    public class GridRequestParametersProjectLog
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Skip { get; set; }
        public int? Take { get; set; }
        //public string FilterLogic { get; set; }

        public IEnumerable<ProjectLog.Infrastructure.SortingInfo> Sortings { get; set; }
        public ProjectLog.Infrastructure.FilterInfo Filters { get; set; }

        public static GridRequestParametersProjectLog Current
        {
            get
            {
                var p = new GridRequestParametersProjectLog();
                p.Populate();
                return p;
            }
        }

        //TODO: pull default values from config
        internal void Populate()
        {
            if (HttpContext.Current != null)
            {
                HttpRequest curRequest = HttpContext.Current.Request;
                if (curRequest["page"] != null)
                    this.Page = int.Parse(curRequest["page"]);
                if (curRequest["pageSize"] != null)
                    this.PageSize = int.Parse(curRequest["pageSize"]);
                if (curRequest["skip"] != null)
                    this.Skip = int.Parse(curRequest["skip"]);
                if (curRequest["take"] != null)
                    this.Take = int.Parse(curRequest["take"]);
                //this.FilterLogic = curRequest["filter[logic]"];

                //build sorting objects from services
                var sorts = new List<ProjectLog.Infrastructure.SortingInfo>();
                var x = 0;
                //List<string> sortList = sort.Split(',').ToList();
                //foreach (string s in sortList)
                //{
                //    var jsonResult = JObject.Parse(s);
                //    sorts.Add(new SortingInfo { SortOn = jsonResult["field"].ToString(), SortOrder = jsonResult["dir"].ToString() });
                //}

                //while (x < 20)
                //{
                //    var sortDirection = curRequest["sort[" + x + "][dir]"];
                //    if (sortDirection == null)
                //    {
                //        break;
                //    }
                //    var sortOn = curRequest["sort[" + x + "][field]"];
                //    if (sortOn != null)
                //    {
                //        sorts.Add(new SortingInfo { SortOn = sortOn, SortOrder = sortDirection });
                //    }
                //    x++;
                //}
                //Sortings = sorts;

                //build sorting objects
                //sorts = new List<SortingInfo>();
                x = 0;
                while (x < 20)
                {
                    var sortDirection = curRequest["sort[" + x + "][dir]"];
                    if (sortDirection == null)
                    {
                        break;
                    }
                    var sortOn = curRequest["sort[" + x + "][field]"];
                    if (sortOn != null)
                    {
                        sorts.Add(new ProjectLog.Infrastructure.SortingInfo { SortOn = sortOn, SortOrder = sortDirection });
                    }
                    x++;
                }
                Sortings = sorts;

                //build filter objects
                var filters = new ProjectLog.Infrastructure.FilterInfo();
                var logic = curRequest["filter[logic]"];
                filters.Logic = logic;

                bool stopLevel1 = false; int level1 = 0;
                while (!stopLevel1)
                {
                    FilterInfo fi = new FilterInfo();
                    var field1 = curRequest["filter[filters][" + level1 + "][field]"];
                    if (field1 == null)
                    {
                        field1 = curRequest["filter[filters][" + level1 + "][filters][0][field]"];
                        if (field1 == null)
                            stopLevel1 = true;
                        else //level 1
                        {
                            fi.Logic = curRequest["filter[filters][" + level1 + "][logic]"];

                            bool stopLevel2 = false; int level2 = 0;
                            while (!stopLevel2)
                            {
                                var field2 = curRequest["filter[filters][" + level1 + "][filters][" + level2 + "][field]"];
                                if (field2 == null)
                                    stopLevel2 = true;
                                else
                                {
                                    var fi2 = new ProjectLog.Infrastructure.FilterInfo();
                                    fi2.Field = curRequest["filter[filters][" + level1 + "][filters][" + level2 + "][field]"];
                                    fi2.Operator = curRequest["filter[filters][" + level1 + "][filters][" + level2 + "][operator]"];
                                    fi2.Value = curRequest["filter[filters][" + level1 + "][filters][" + level2 + "][value]"];

                                    if (fi.Filters == null)
                                        fi.Filters = new List<ProjectLog.Infrastructure.FilterInfo>();
                                    fi.Filters.Add(fi2);

                                    ++level2;
                                }
                            }
                        }
                    }
                    else
                    {
                        fi.Field = field1;
                        fi.Operator = curRequest["filter[filters][" + level1 + "][operator]"];
                        fi.Value = curRequest["filter[filters][" + level1 + "][value]"];
                    }

                    if (!stopLevel1)
                    {
                        if (filters.Filters == null)
                            filters.Filters = new List<ProjectLog.Infrastructure.FilterInfo>();
                        filters.Filters.Add(fi);

                        ++level1;
                    }
                }

                Filters = filters;
            }
        }
    }
}