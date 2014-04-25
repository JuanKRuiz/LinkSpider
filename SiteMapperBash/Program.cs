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
            //var hc = new HttpClient();
            //var htmlFragment = hc.GetStringAsync("http://juank.io").Result;

            ls.ExploreSite( "http://juank.io");

            
            Console.ReadLine();
        }
    }
}
