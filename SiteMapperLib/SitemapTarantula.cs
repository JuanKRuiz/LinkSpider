using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace LinkSpiderLib
{
    public class SitemapTarantula
    {
        IEnumerable<string> _URLList;
        IEnumerable<string> _urlSitemapFilter;
        public int Count { get; set; }


        public SitemapTarantula(IEnumerable<string> urlList)
        {
            _URLList = urlList;
        }

        public SitemapTarantula(IEnumerable<string> urlList, IEnumerable<string> urlfilter)
        {
            _URLList = urlList;
            _urlSitemapFilter = urlfilter;
        }

        public XDocument CreateXMLDocumentSitemap()
        {
            var xmlns = (XNamespace)"http://www.sitemaps.org/schemas/sitemap/0.9";
            var xsi = (XNamespace)"http://www.w3.org/2001/XMLSchema-instance";
            var schemaLocation = (XNamespace)"http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd";
            var changefreq = new XElement(xmlns + "changefreq", "daily");
            var priority = new XElement(xmlns + "priority", "0.5");

            XDocument xdoc = new XDocument(new XDeclaration("1.0", "utf-8", null));
            var urlset = new XElement(xmlns + "urlset",
                    new XAttribute(XNamespace.Xmlns + "xsi", xsi.ToString()),
                    new XAttribute(xsi + "schemaLocation", schemaLocation.ToString()));

            xdoc.Add(urlset);

            foreach (var link in _URLList)
            {
                if (!MatchWithFilter(link))
                {
                    var loc = new XElement(xmlns + "loc", link);
                    var url = new XElement(xmlns + "url", loc, changefreq, priority);
                    Count++;
                    urlset.Add(url);
                }
            }
            return xdoc;
        }

        private bool MatchWithFilter(string link)
        {
            bool rta = false;

            if (_urlSitemapFilter == null)
                rta = false;
            else
            {
                foreach (var item in _urlSitemapFilter)
                {
                    if (link.Contains(item))
                    {
                        rta = true;
                        break;
                    }
                }
            }

            return rta;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Returns UTF 16 Encoding</remarks>
        /// <param name="changeDeclarationTextToUTF8">Change xml declaration text, but string still being UTF 16</param>
        /// <returns></returns>
        public string CreateStringSiteMap(bool changeDeclarationTextToUTF8 = false)
        {
            StringWriter sw = new StringWriter();

            var xdoc = CreateXMLDocumentSitemap();
            xdoc.Save(sw);
            return sw.ToString();
        }

        public byte[] CreateByteArrSiteMapUTF8()
        {
            StringWriterUTF8 sw = new StringWriterUTF8();

            var xdoc = CreateXMLDocumentSitemap();
            xdoc.Save(sw);

            var utf16string = sw.ToString();
            return Encoding.Convert(Encoding.Unicode, Encoding.UTF8, Encoding.Unicode.GetBytes(utf16string));
        }
    }
}
