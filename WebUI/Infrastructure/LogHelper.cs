using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WebUI.Infrastructure
{
    public class LogHelper
    {
        const string FilePath = "~/App_Data/log.txt";

        public void Write(string message)
        {
            string fullPath = System.Web.Hosting.HostingEnvironment.MapPath(FilePath);
            File.AppendAllText(fullPath, message + "\n");
        }
    }
}