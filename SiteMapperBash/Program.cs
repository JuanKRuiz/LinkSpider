using SiteMapperLib;
using System;
using System.Collections.Generic;
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
            LinkSpider ls = new LinkSpider();
            var hc = new HttpClient();
            var htmlFragment = hc.GetStringAsync("http://juank.io").Result;

            var lista = ls.GetCompleteLinksFromHtmlFragment(htmlFragment, "http://juank.io");

            foreach (var link in lista)
            {
                Console.WriteLine( link);
            }
            Console.ReadLine();
        }
    }
}
