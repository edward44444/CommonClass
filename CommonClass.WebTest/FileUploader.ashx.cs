using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace CommonClass.WebTest
{
    /// <summary>
    /// Summary description for FileUploader1
    /// </summary>
    public class FileUploader1 : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            foreach (string key in context.Request.Files.Keys)
            {
                HttpPostedFile file = context.Request.Files[key];
                if (file.ContentLength > 0)
                {
                    file.SaveAs(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetFileName(file.FileName)));
                }
            }
            context.Response.ContentType = "text/plain";
            context.Response.Write("Hello World");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}