using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Business.Entities;

namespace WebUI.Models.Log
{
    public class LogPresentationStub
    {
		// Example model value from scaffolder script: 0
		public long Id { get; set; }
		public System.DateTime Timestamp { get; set; }
		public string Application { get; set; }
		public string Ip { get; set; }
		public string User { get; set; }
		public string Action { get; set; }
		public string Data { get; set; }
		
		public LogPresentationStub() { }

		public LogPresentationStub(log dbItem) { 
			this.Id = dbItem.id;
			this.Timestamp = dbItem.timestamp;
			this.Application = dbItem.application;
			this.Ip = dbItem.ip;
			this.User = dbItem.user;
			this.Action = dbItem.action;
			this.Data = dbItem.data;
		}

		public List<LogPresentationStub> MapList(List<log> dbItems)
        {
            List<LogPresentationStub> retList = new List<LogPresentationStub>();

            foreach (log dbItem in dbItems)
                retList.Add(new LogPresentationStub(dbItem));

            return retList;
        }
	}
}

