using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectLog.Entities;
using ProjectLog.Infrastructure;
using ProjectLog.Linq;
using System.Text.RegularExpressions;

namespace ProjectLog
{
    public class LogItem
    {
        public string Application { get; set; }
        public DateTime DateTime { get; set; }
        public string IP { get; set; }
        public string Username { get; set; }
        public string Action { get; set; }
        public string Data { get; set; }

        private mobidigEntities context = new mobidigEntities();

        public LogItem() { 
        
        }

        public LogItem(string app, string ip, string username, string action, string data)
        {
            Application = app;
            IP = ip;
            Username = username;
            Action = action;
            Data = data;
            DateTime = DateTime.Now;

        }

        public bool SaveToDB()
        {
            
            log data = MapData();

            context.log.Add(data);
            int i = context.SaveChanges();

            return i > 0;
        }

        private log MapData() {
            log dbItem = new log
            {
                timestamp = this.DateTime,
                application = Application,
                ip = this.IP,
                user = this.Username,
                action = this.Action,
                data = this.Data,
            };
            return dbItem;
        }

        public List<log> Find(int skip = 0, int? take = null, List<SortingInfo> sortings = null, FilterInfo filters = null)
        {
            IQueryable<log> atributes = context.log;

            if (filters != null && (filters.Filters != null && filters.Filters.Count > 0))
            {
                filters.FormatFieldToUnderscore();
                GridHelper.ProcessFilters<log>(filters, ref atributes);
            }

            if (sortings != null && sortings.Count > 0)
            {
                foreach (var s in sortings)
                {
                    string sortOn = this.MapSort(s.SortOn);
                    atributes = atributes.OrderBy(sortOn + " " + s.SortOrder);
                }
            }
            else
            {
                atributes = atributes.OrderBy("id desc");
            }

            var takeActions = atributes;
            if (take != null)
            {
                takeActions = atributes.Skip(skip).Take((int)take);
            }

            List<log> actionList = takeActions.ToList();

            return actionList;
        }

        public string MapSort(string sortOn)
        {
            string mapSortOn = sortOn;
            mapSortOn = Regex.Replace(sortOn, @"(\p{Ll})(\p{Lu})", "$1_$2");

            return mapSortOn;
        }

        public int Count(FilterInfo filters = null)
        {
            IQueryable<log> items = context.log;

            if (filters != null && (filters.Filters != null && filters.Filters.Count > 0))
            {
                GridHelper.ProcessFilters<log>(filters, ref items);
            }

            return items.Count();
        }
    }
}
