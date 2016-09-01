using Business.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Web.Mvc;
using ProjectLog.Entities;

namespace WebUI.Models
{
    public class LogActivityViewModel
    {
        public long Id { get; set; }
        public string Timestamp { get; set; }
        public string Application { get; set; }
        public string Ip { get; set; }
        public string User { get; set; }
        public string Action { get; set; }
        public string Data { get; set; }

        public LogActivityViewModel() { }

        public LogActivityViewModel(log l)
        {
            this.Id = l.id;
            this.Timestamp = l.timestamp.ToString();
            this.Application = l.application;
            this.Ip = l.ip;
            this.User = l.user;
            this.Action = l.action;
            this.Data = l.data;
        }

        public List<LogActivityViewModel> MapList(List<log> listLog)
        {
            List<LogActivityViewModel> result = new List<LogActivityViewModel>();
            foreach (log a in listLog)
            {
                LogActivityViewModel listViewModel = new LogActivityViewModel(a);
                result.Add(listViewModel);
            }
            return result;
        }
    }
}