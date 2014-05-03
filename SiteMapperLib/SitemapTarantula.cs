using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LinkSpiderLib
{
    public class SitemapTarantula
    {
        IEnumerable<string> _URLList;
        public SitemapTarantula(IEnumerable<string> urlList)
        {
            _URLList = urlList;
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
                var loc = new XElement(xmlns + "loc", link);
                var url = new XElement(xmlns + "url", loc, changefreq, priority);
                urlset.Add(url);
            }
            return xdoc;
        }

        public string CreateStringSiteMap()
        {
            var xdoc = CreateXMLDocumentSitemap();
            return xdoc.ToString();
        }
    }
}
