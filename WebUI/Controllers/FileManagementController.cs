using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebUI.Infrastructure;

namespace WebUI.Controllers
{
    public class FileManagementController : Controller
    {
        private const string FILE_DIRECTORY = "/Uploads/";

        /**
         * save file using kendo file uploader
         * menangani kalau nama file sama
         * The Name of the Upload component is "files"
         * currently not support multiple uploads
         * 
         * @return filepath ex "~/Uploads/rekadia.jpg"
         */
        public string Save(IEnumerable<HttpPostedFileBase> files)
        {
            //kamus lokal
            string fileName = "", savedFilename = "", physicalPath = "", imagePath = "", friendlyFilename = "", absoluteFile = "";

            //algoritma
            if (files != null)
            {
                foreach (var file in files)
                {
                    var identifier = Guid.NewGuid();
                    bool fileDirectory = Directory.Exists(Server.MapPath(FILE_DIRECTORY));
                    if (fileDirectory == false)
                    {
                        Directory.CreateDirectory(Server.MapPath(FILE_DIRECTORY));
                    }
                    fileName = Path.GetFileNameWithoutExtension(file.FileName);
                    friendlyFilename = new DisplayFormatHelper().URLFriendly(fileName) + Path.GetExtension(file.FileName);
                    savedFilename = identifier + "_" + friendlyFilename;

                    //save file
                    physicalPath = Path.Combine(Server.MapPath(FILE_DIRECTORY), savedFilename);
                    file.SaveAs(physicalPath);
                }
            }

            imagePath = FILE_DIRECTORY + savedFilename;
            absoluteFile = Url.Content(imagePath);
            return new JavaScriptSerializer().Serialize(new { filepath = imagePath, filename = friendlyFilename, absolutepath = absoluteFile });
        }

        /**
         * remove file using kendo file uploader
         */
        public ActionResult Remove(string[] fileNames)
        {
            // The parameter of the Remove action must be called "fileNames"

            if (fileNames != null)
            {
                foreach (var fullName in fileNames)
                {
                    var fileName = Path.GetFileName(fullName);
                    var physicalPath = Path.Combine(Server.MapPath(FILE_DIRECTORY), fileName);

                    if (System.IO.File.Exists(physicalPath))
                    {
                        // The files are not actually removed in this demo
                        System.IO.File.Delete(physicalPath);
                    }
                }
            }

            // Return an empty string to signify success
            return Content("");
        }
    }
}
