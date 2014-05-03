using SiteMapperLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SiteMapperBash
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();

            LinkSpider ls = new LinkSpider("http://juank.io");
            //var hc = new HttpClient();
            //var htmlFragment = hc.GetStringAsync("http://juank.io").Result;

            sw.Start();
            ls.WeaveWeb();
            sw.Stop();
            Console.WriteLine("----A S Y N C ----");
            Console.WriteLine("Time Running: {0}", sw.Elapsed);

            Console.WriteLine("Total Links: {0}", ls.FullUrlList.Count);
            Console.WriteLine("Total Visited Links: {0}", ls.FullUrlList.Where(le=> le.explored==true).Count());
            Console.WriteLine("Total External Links: {0}", ls.ExternalUrlList.Count);
            Console.WriteLine("Total Broken Links: {0}", ls.BrokenUrlList.Count);


            /*foreach (var linkItem in ls.FullUrlList.OrderBy(linkItem => linkItem.url))
            {
                Console.WriteLine(linkItem.url);
            }*/

            Console.ReadLine();
        }
    }
}
