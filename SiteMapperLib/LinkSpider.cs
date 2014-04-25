using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SiteMapperLib
{
    public class LinkSpider
    {
        readonly Regex rx = new Regex(@"<a.*?href=(""|')(?<href>.*?)(""|').*?>(?<value>.*?)</a>");
  
        public List<string> GetLinksFromHtmlFragment(string htmlFragment)
        {
            var lista = new List<string>();
            foreach (Match match in rx.Matches(htmlFragment))
            {
                lista.Add(match.Groups["href"].Value);
            }

            return lista;
        }

        public List<string> GetCompleteLinksFromHtmlFragment(string htmlFragment, string baseUrl)
        {
            UriBuilder uribldr = new UriBuilder(baseUrl);

            var lista = new List<string>();
            foreach (Match match in rx.Matches(htmlFragment))
            {
                var link = match.Groups["href"].Value;
                
                if(link.StartsWith("/") || !link.Contains("://"))
                {
                    uribldr.Path = link;
                    uribldr.Query = string.Empty;
                    lista.Add(uribldr.Uri.AbsoluteUri);
                }
                else if (link.StartsWith(baseUrl))
                {
                    var uri = new Uri(link);
                    uribldr.Path = uri.AbsolutePath;
                    uribldr.Query = string.Empty;
                    lista.Add(uribldr.Uri.AbsoluteUri);
                }
            }

            return lista;
        }

    }
}
