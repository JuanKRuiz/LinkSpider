using LinkSpiderLib;
using Mono.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace LinkSpiderConsole
{
    class Program
    {
        static Uri _url = null;
        static string _sitemapFile = "sitemap.xml";
        static string _plainFile = "plain.txt";
        static bool _show_help = false;
        static bool _useUnicode = false;
        static List<string> _urlNavFilter = new List<string>();
        static List<string> _urlSitemapFilter = new List<string>();

        static void Main(string[] args)
        {
            ConsoleExt.WriteTitle(" LinkSpider Console by @JuanKRuiz ", true);


            var paramsOptions = ParseParameters(args);
            if (args.Length == 0)
            {
                ConsoleExt.WriteTitle(" What you wanna do? ");
                Console.WriteLine(" Try --help for more information.");
            }
            else
            {
                Stopwatch sw = new Stopwatch();

                LinkSpider ls = new LinkSpider(_url.AbsoluteUri);

                ls.URLExplorationFilter.AddRange(_urlNavFilter);
                Console.WriteLine("Starting to Weave Web");
                Console.WriteLine("Exploring and Bulding...");
                sw.Start();
                ls.WeaveWeb();
                sw.Stop();
                Console.WriteLine("An incredible Web has been Weaved");
                Console.WriteLine("Total Links: {0}", ls.FullUrlList.Count());
                Console.WriteLine("Total External Links: {0}", ls.ExternalUrlList.Count());
                Console.WriteLine("Total Broken Links: {0}", ls.BrokenUrlList.Count());
                Console.WriteLine("Time: {0}", sw.Elapsed);

                var sortedList = ls.FullUrlList.ToList();
                ConsoleExt.WriteTitle("Generating files...");
                BuildPlainFile(_plainFile, sortedList);
                Console.WriteLine("{0} OK", _plainFile);
                BuildPlainFile("brokenLinks.txt", ls.BrokenUrlList);
                Console.WriteLine("{0} OK", "brokenLinks.txt");
                int counter;
                BuildSiteMap(_sitemapFile, _useUnicode, sortedList, _urlSitemapFilter, out counter);
                Console.WriteLine("{0} en formato Unicode: {1} OK", _sitemapFile, _useUnicode);
                Console.WriteLine("--> Total Sitema Links: {0}", counter);
            }

#if DEBUG
            ConsoleExt.WriteTitle(" PRESIONE ENTER PARA SALIR ", true);
            Console.ReadLine();
#endif
            ConsoleExt.WriteTitle("  F  I  N  ", true);
        }

        private static void BuildSiteMap(string sitemapFile, bool useUnicode, List<string> sortedList, List<string> filter, out int count)
        {
            var tarantula = new SitemapTarantula(sortedList, filter);

            string sitemap;

            if (useUnicode)
            {
                sitemap = tarantula.CreateStringSiteMap();
                File.WriteAllText(sitemapFile, sitemap);
            }
            else// UTF8
            {
                sitemap = tarantula.CreateStringSiteMap(changeDeclarationTextToUTF8: true);
                File.WriteAllText(sitemapFile, sitemap, Encoding.UTF8);
            }

            count = tarantula.Count;
        }

        private static void BuildPlainFile(string plainFile, IEnumerable<string> sortedList)
        {
            if (sortedList.Count() > 0)
                File.WriteAllLines(plainFile, sortedList);
        }

        private static void ShowHelp(OptionSet paramsOptions)
        {
            ConsoleExt.WriteTitle("  H E L P  ");
            paramsOptions.WriteOptionDescriptions(Console.Out);
            ConsoleExt.WriteTitle("           ");
        }

        private static OptionSet ParseParameters(string[] args)
        {
            var paramsOptions = new OptionSet() {
                    { "u|url=", "The site {URL} to start link exploration",
                      u =>{
                          try { 
                          _url = new Uri(u);
                          }catch(Exception ex)
                          {
                              throw new OptionException("Invalid Url", "url", ex);
                          }
                      }
                    },
                    { "s|sitemap=", "Sitemap with this name must be generated {FileName}",
                      s => _sitemapFile = s 
                    },
                    { "p|plain=",  "Textfile including a link list must be generated {FileName}",
                       p =>_plainFile = p
                    },
                    { "n|navfilter=",  "Comma separated string with paths must be exclued in exploration\nexample: '/tag/,/pages'.Be carefull with case sensitive and white spaces",
                       n =>{
                           AddCommaStringToList(_urlNavFilter,n);
                       }
                    },
                    { "m|smapfilter=",  "Comma separated string with paths must be exclued in sitemap\nexample: '/tag/,/pages'.Be carefull with case sensitive and white spaces",
                       sf =>{
                           AddCommaStringToList(_urlSitemapFilter,sf);
                       }
                    },
                    { "c|unicode",  "Use unicode Encoding for sitemap", 
                      u => _useUnicode = (u != null )
                    },
                    { "h|?|help",  "Show this message and exit", 
                      v => _show_help = (v != null )
                    }
            };

            try
            {
                paramsOptions.Parse(args);
            }
            catch (OptionException e)
            {
                ConsoleExt.WriteTitle("ERROR");
                Console.WriteLine("Parameter: {0} - {1}", e.OptionName, e.Message);
                Console.WriteLine("Try --help for more information.");
                throw;
            }

            if (_show_help)
                ShowHelp(paramsOptions);

            return paramsOptions;
        }

        private static void AddCommaStringToList(List<string> list, string f)
        {
            var parts = f.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            list.AddRange(parts);
        }
    }
}
