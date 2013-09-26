using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Web;
using System.Collections.Specialized;

namespace CommonClass.ConsoleTest
{
    class Program
    {
        private static System.Web.Caching.Cache _cahe = HttpRuntime.Cache;

        static void Main(string[] args)
        {
            HttpHelperTest();
            //WebPageCapturer caputer = new WebPageCapturer(1024, 768);
            //Bitmap image = caputer.Capture("http://www.baidu.com/",3000);
            //image.Save("1.jpg");

            //CommonCacheDependencyTest();


            Console.Read();
        }

        private static void HttpHelperTest()
        {
            //HttpHelper httpHelper = new HttpHelper("GET");
            //Console.WriteLine(httpHelper.GetResponse("http://www.baidu.com/"));
            Dictionary<string, string> headers = new Dictionary<string, string>();
            HttpHelper httpHelper = new HttpHelper("POST", "multipart/form-data; boundary=----WebKitFormBoundaryYFFgMi35KsmiB8ki", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8", headers);
            httpHelper.KeepAlive = true;
            NameValueCollection form = new NameValueCollection();
            form.Add("__VIEWSTATE", "/wEPDwUKMTUyOTIzOTY3M2RklcwzkiuxMWwLbqK2j+X8v1uEapS2vVQi8rhcQKUbwZE=");
            form.Add("__EVENTVALIDATION", "/wEdAAIwq61+ETXzKZgFsCRK53u75vhDofrSSRcWtmHPtJVt4FCaPsncA3ve5V4+UUACO0qRaDoWAJ1vUeQbi2jS6ynN");
            NameValueCollection controlForm = new NameValueCollection();
            controlForm.Add("btnUpload", "Upload");
            UploadFileInfo[] files = new UploadFileInfo[]
            {
                new UploadFileInfo{FilePath=@"C:\Users\Public\Pictures\Sample Pictures\dog.jpg",ControlName="file1",ContentType="image/jpeg"},
                new UploadFileInfo{FilePath=@"C:\Users\Public\Pictures\Sample Pictures\123.jpg",ControlName="file2",ContentType="image/jpeg"}
            };
            Console.WriteLine(httpHelper.UploadFile("http://edward-pc:5566/FileUploader.aspx", form, controlForm, files));
        }

        private static void CommonCacheDependencyTest()
        {
            new Thread(() =>
            {
                int i = 0;
                while (true)
                {
                    if (_cahe.Get("DateTime") == null)
                    {
                        UserConfigCacheDependency dep = new UserConfigCacheDependency("edward");
                        Console.WriteLine(dep.HasChanged);
                        Console.WriteLine(dep.UtcLastModified);
                        CacheHelper.Insert("DateTime", DateTime.Now, dep);
                    }
                    if (_cahe.Get("DateTime") != null)
                    {
                        Console.WriteLine("edward" + ((DateTime)_cahe.Get("DateTime")).ToString("yyyyMMddHHmmss"));
                    }
                    Thread.Sleep(1000);
                    i++;
                }
            }).Start();
        }
    }
}
