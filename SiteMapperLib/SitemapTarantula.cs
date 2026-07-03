using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace LinkSpiderLib;

public class SitemapTarantula(IEnumerable<string> urlList, IEnumerable<string>? urlFilter = null)
{
    private const string UTF16_HEADER = """<?xml version="1.0" encoding="utf-16"?>""";
    private const string UTF8_HEADER = """<?xml version="1.0" encoding="utf-8"?>""";

    private readonly IEnumerable<string> _urlList = urlList;
    private readonly IEnumerable<string>? _urlSitemapFilter = urlFilter;

    public int Count { get; private set; }

    public XDocument CreateXMLDocumentSitemap()
    {
        XNamespace xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
        XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
        XNamespace schemaLocation = "http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd";
        
        var changefreq = new XElement(xmlns + "changefreq", "daily");
        var priority = new XElement(xmlns + "priority", "0.5");

        var xdoc = new XDocument(new XDeclaration("1.0", "utf-8", null));
        var urlset = new XElement(xmlns + "urlset",
            new XAttribute(XNamespace.Xmlns + "xsi", xsi.ToString()),
            new XAttribute(xsi + "schemaLocation", schemaLocation.ToString()));

        xdoc.Add(urlset);

        foreach (var link in _urlList)
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
        if (_urlSitemapFilter is null) return false;

        foreach (var item in _urlSitemapFilter)
        {
            if (link.Contains(item, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    public string CreateStringSiteMap(bool changeDeclarationTextToUTF8 = false)
    {
        using var sw = new StringWriter();
        var xdoc = CreateXMLDocumentSitemap();
        xdoc.Save(sw);

        var ret = sw.ToString();
        return changeDeclarationTextToUTF8 ? ret.Replace(UTF16_HEADER, UTF8_HEADER) : ret;
    }

    public byte[] CreateByteArrSiteMapUTF8()
    {
        using var sw = new StringWriterUTF8();
        var xdoc = CreateXMLDocumentSitemap();
        xdoc.Save(sw);

        var utf16string = sw.ToString();
        var utf8string = utf16string.Replace(UTF16_HEADER, UTF8_HEADER);

        return Encoding.Convert(Encoding.Unicode, Encoding.UTF8, Encoding.Unicode.GetBytes(utf8string));
    }
}
