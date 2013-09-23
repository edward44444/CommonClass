using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Web;

namespace CommonClass.ConsoleTest
{
    class Program
    {
        private static System.Web.Caching.Cache _cahe = HttpRuntime.Cache;

        static void Main(string[] args)
        {
            //HttpHelper httpHelper = new HttpHelper("GET");
            //Console.WriteLine(httpHelper.GetResponse("http://www.baidu.com/"));

            //WebPageCapturer caputer = new WebPageCapturer(1024, 768);
            //Bitmap image = caputer.Capture("http://www.baidu.com/",3000);
            //image.Save("1.jpg");

            //CustomCacheDependencyManager.Start();
            //new Thread(() => {
            //    int i=0;
            //    while (true)
            //    {
            //        if (_cahe.Get("DateTime") == null)
            //        {
            //            CustomCacheDependency dep = new CustomCacheDependency("edward");
            //            Console.WriteLine(dep.HasChanged);
            //            Console.WriteLine(dep.UtcLastModified.ToString("yyyy-MM-dd HH:mm:ss"));
            //            _cahe.Insert("DateTime", DateTime.Now,dep );
            //        }
            //        if (_cahe.Get("DateTime") != null)
            //        {
            //            Console.WriteLine(((DateTime)_cahe.Get("DateTime")).ToString("yyyyMMddHHmmss"));
            //        }
            //        Thread.Sleep(1000);
            //        i++;
            //    }
            //}).Start();

            new Thread(() =>
            {
                int i = 0;
                while (true)
                {
                    if (_cahe.Get("DateTime") == null)
                    {
                        UserConfigCacheDependency dep = new UserConfigCacheDependency("edward");
                        Console.WriteLine(dep.HasChanged);
                        Console.WriteLine(dep.UtcLastModified.ToString("yyyy-MM-dd HH:mm:ss"));
                        CacheHelper.Insert("DateTime", DateTime.Now, dep);
                    }
                    if (_cahe.Get("DateTime") != null)
                    {
                        Console.WriteLine(((DateTime)_cahe.Get("DateTime")).ToString("yyyyMMddHHmmss"));
                    }
                    Thread.Sleep(1000);
                    i++;
                }
            }).Start();
            Console.Read();
        }
    }
}
