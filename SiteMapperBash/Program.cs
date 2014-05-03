using LinkSpiderLib;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace LinkSpiderConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();

            LinkSpider ls = new LinkSpider("http://juank.io/");

            ls.URLExplorationFilter.Add("/tag/");
            sw.Start();
            ls.WeaveWeb();
            sw.Stop();

            foreach (var linkItem in ls.FullUrlList.OrderBy(linkItem => linkItem.url))
            {
                Console.WriteLine(linkItem.url);
            }

            var listaTxt = from linkItem in ls.FullUrlList
                           where !linkItem.url.Contains("/tag/")
                           && !linkItem.url.Contains("/page/")
                           select linkItem.url;

            Console.WriteLine("---- LINK SPIDER ----");
            Console.WriteLine("Time Running: {0}", sw.Elapsed);

            Console.WriteLine("Total Links: {0}", ls.FullUrlList.Count);
            Console.WriteLine("Total Visited Links: {0}", ls.FullUrlList.Where(le=> le.explored==true).Count());
            Console.WriteLine("Total External Links: {0}", ls.ExternalUrlList.Count);
            Console.WriteLine("Total Broken Links: {0}", ls.BrokenUrlList.Count);

            var tarantula = new SitemapTarantula(listaTxt);

            var sitemap = tarantula.CreateStringSiteMap();
            File.WriteAllText("sitemap.xml", sitemap);


            Console.ReadLine();
        }
    }
}
