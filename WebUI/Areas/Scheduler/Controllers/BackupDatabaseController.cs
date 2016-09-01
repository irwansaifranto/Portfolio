using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.EntityClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace WebUI.Areas.Scheduler.Controllers
{
    public class BackupDatabaseController : Controller
    {
        public string Index()
        {
            string response = "Backup has been Successfull !!";
            string timestamp = DateTime.Now.ToString("yyyyMMdd");
            string file = String.Format("{0}.sql", timestamp);
            string savePath = Server.MapPath(Url.Content("~/Backup")) + "\\";
            string fullPath = savePath + file;

            string executablePath = WebConfigurationManager.AppSettings["PostgresPgDump"];
            string conn = ConfigurationManager.ConnectionStrings["Entities"].ConnectionString;
            EntityConnectionStringBuilder entityConn = new EntityConnectionStringBuilder(conn);
            string setup = entityConn.ProviderConnectionString;

            Dictionary<string, string> connStringParts = setup.Split(';')
                    .Select(t => t.Split(new char[] { '=' }, 2))
                    .ToDictionary(t => t[0].Trim(), t => t[1].Trim(), StringComparer.InvariantCultureIgnoreCase);

            string server = connStringParts["Host"];
            string port = connStringParts["Port"];
            string database = connStringParts["Database"];
            string password = connStringParts["Password"];
            string username = connStringParts["Username"];

            //string arguments = String.Format(" -h localhost -d karental -U postgres -f /Users/Backup/q.sql");
            string arguments = String.Format(" -h {1} -d {2} -U {3} -p{4} --column-inserts -f {0}", fullPath, server, database, username, port);

            Boolean result = false;
            try
            {
                Environment.SetEnvironmentVariable("PGPASSWRD", password);
                System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
                info.FileName = executablePath;
                info.Arguments = arguments;
                info.CreateNoWindow = true;
                info.UseShellExecute = false;
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = info;
                proc.Start();
                proc.WaitForExit();
                result = true;

            }
            catch (Exception ex)
            {
                response = ex.Message;

                return response;
            }

            return response;
        }
    }
}
