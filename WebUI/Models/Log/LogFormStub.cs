using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Business.Entities;

namespace WebUI.Models.Log
{
    public class LogFormStub
    {
		// Example model value from scaffolder script: 0
		[DisplayName("Id")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
		public long Id { get; set; }

		[DisplayName("Timestamp")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
		public System.DateTime Timestamp { get; set; }

		[DisplayName("Application")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
		public string Application { get; set; }

		[DisplayName("Ip")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
		public string Ip { get; set; }

		[DisplayName("User")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
		public string User { get; set; }

		[DisplayName("Action")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
		public string Action { get; set; }

		[DisplayName("Data")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
		public string Data { get; set; }

		public LogFormStub() { }


		public LogFormStub(log dbItem)
			: this()
		{
			this.Id = dbItem.id;
			this.Timestamp = dbItem.timestamp;
			this.Application = dbItem.application;
			this.Ip = dbItem.ip;
			this.User = dbItem.user;
			this.Action = dbItem.action;
			this.Data = dbItem.data;
		}

		public log GetDbObject(log dbItem) {
			dbItem.id = this.Id;
			dbItem.timestamp = this.Timestamp;
			dbItem.application = this.Application;
			dbItem.ip = this.Ip;
			dbItem.user = this.User;
			dbItem.action = this.Action;
			dbItem.data = this.Data;
			return dbItem;
		}

		#region options


		#endregion

	}
}

