using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using LinkSpiderLib;
using Mono.Options;

namespace LinkSpiderConsole;

internal static class Program
{
    private static Uri? _url;
    private static string _sitemapFile = "sitemap.xml";
    private static string _plainFile = "plain.txt";
    private static bool _showHelp;
    private static bool _useUnicode;
    private static bool _singleWebPage;
    private static readonly List<string> _urlNavFilter = [];
    private static readonly List<string> _urlSitemapFilter = [];

    private static void Main(string[] args)
    {
        DrawHeader();

        var paramsOptions = ParseParameters(args);
        if (args.Length == 0)
        {
            ConsoleExt.WriteTitle(" What you wanna do? ");
            Console.WriteLine(" Try --help for more information.");
        }
        else
        {
            if (_url is null)
            {
                Console.WriteLine("Error: URL was not specified.");
                return;
            }

            var sw = new Stopwatch();

            using var ls = new LinkSpider(_url.AbsoluteUri);
            ls.URLExplorationFilter.AddRange(_urlNavFilter);

            Console.WriteLine("Starting to Weave Web");
            Console.WriteLine("Exploring and Building...");
            sw.Start();

            if (_singleWebPage)
            {
                ls.WeaveSinglePage();
            }
            else
            {
                ls.WeaveWeb();
            }

            sw.Stop();
            Console.WriteLine("An incredible Web has been Weaved");
            Console.WriteLine($"Total Links: {ls.FullUrlList.Count()}");
            Console.WriteLine($"Total External Links: {ls.ExternalUrlList.Count()}");
            Console.WriteLine($"Total Broken Links: {ls.BrokenUrlList.Count()}");
            Console.WriteLine($"Time: {sw.Elapsed}");

            var sortedList = ls.FullUrlList.ToList();
            ConsoleExt.WriteTitle("Generating files...");
            BuildPlainFile(_plainFile, sortedList);
            Console.WriteLine($"{_plainFile} OK");
            BuildPlainFile("brokenLinks.txt", ls.BrokenUrlList);
            Console.WriteLine("brokenLinks.txt OK");
            BuildPlainFile("externalLinks.txt", ls.ExternalUrlList);
            Console.WriteLine("externalLinks.txt OK");

            BuildSiteMap(_sitemapFile, _useUnicode, sortedList, _urlSitemapFilter, out var counter);
            Console.WriteLine($"{_sitemapFile} in Unicode format: {_useUnicode} OK");
            Console.WriteLine($"--> Total Sitemap Links: {counter}");
        }

#if DEBUG
        ConsoleExt.WriteTitle(" PRESS ENTER TO EXIT ", true);
        Console.ReadLine();
#endif
        ConsoleExt.WriteTitle("  E  N  D  ", true);
    }

    private static void DrawHeader()
    {
        ConsoleExt.WriteTitle(" LinkSpider Console by @JuanKRuiz ", true);
        Console.WriteLine(" Licensed under MIT license");
        ConsoleExt.WriteTitle("    Contact Info     ", true);
        Console.WriteLine("Twitter: @JuanKRuiz");
        Console.WriteLine("Facebook: JuanKDev");
        Console.WriteLine("Blog: http://juank-io");
        ConsoleExt.WriteTitle("                     ");
    }

    private static void BuildSiteMap(string sitemapFile, bool useUnicode, List<string> sortedList, List<string> filter, out int count)
    {
        var tarantula = new SitemapTarantula(sortedList, filter);

        if (useUnicode)
        {
            var sitemap = tarantula.CreateStringSiteMap();
            File.WriteAllText(sitemapFile, sitemap);
        }
        else
        {
            var sitemap = tarantula.CreateStringSiteMap(changeDeclarationTextToUTF8: true);
            File.WriteAllText(sitemapFile, sitemap, Encoding.UTF8);
        }

        count = tarantula.Count;
    }

    private static void BuildPlainFile(string plainFile, IEnumerable<string> sortedList)
    {
        if (sortedList.Any())
        {
            File.WriteAllLines(plainFile, sortedList);
        }
    }

    private static void ShowHelp(OptionSet paramsOptions)
    {
        ConsoleExt.WriteTitle("  H E L P  ");
        paramsOptions.WriteOptionDescriptions(Console.Out);
        ConsoleExt.WriteTitle("           ");
    }

    private static OptionSet ParseParameters(string[] args)
    {
        var paramsOptions = new OptionSet
        {
            { "u|url=", "The site {URL} to start link exploration",
              u => {
                  try
                  { 
                      _url = new Uri(u);
                  }
                  catch (Exception ex)
                  {
                      throw new OptionException("Invalid Url", "url", ex);
                  }
              }
            },
            { "s|sitemap=", "Sitemap with this name must be generated {FileName}",
              s => _sitemapFile = s 
            },
            { "p|plain=", "Textfile including a link list must be generated {FileName}",
              p => _plainFile = p
            },
            { "n|navfilter=", "Comma separated string with paths must be excluded in exploration\nexample: '/tag/,/pages'. Be careful with case sensitivity and white spaces",
              n => AddCommaStringToList(_urlNavFilter, n)
            },
            { "m|smapfilter=", "Comma separated string with paths must be excluded in sitemap\nexample: '/tag/,/pages'. Be careful with case sensitivity and white spaces",
              sf => AddCommaStringToList(_urlSitemapFilter, sf)
            },
            { "c|unicode", "Use unicode Encoding for sitemap", 
              u => _useUnicode = u is not null
            },
            { "o|single", "Single web page scan", 
              o => _singleWebPage = o is not null
            },
            { "h|?|help", "Show this message and exit", 
              v => _showHelp = v is not null
            }
        };

        try
        {
            paramsOptions.Parse(args);
        }
        catch (OptionException e)
        {
            ConsoleExt.WriteTitle("ERROR");
            Console.WriteLine($"Parameter: {e.OptionName} - {e.Message}");
            Console.WriteLine("Try --help for more information.");
            throw;
        }

        if (_showHelp)
        {
            ShowHelp(paramsOptions);
        }

        return paramsOptions;
    }

    private static void AddCommaStringToList(List<string> list, string f)
    {
        var parts = f.Split([","], StringSplitOptions.RemoveEmptyEntries);
        list.AddRange(parts);
    }
}
