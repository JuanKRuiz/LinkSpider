using System.Collections.Generic;
using System.Linq;
using LinkSpiderLib;
using Xunit;

namespace SiteMapperTests;

public class SitemapTarantulaTests
{
    [Fact]
    public void CreateXMLDocumentSitemap_GeneratesValidSitemap()
    {
        // Arrange
        var urls = new List<string>
        {
            "https://example.com/page1",
            "https://example.com/page2",
            "https://example.com/page3"
        };
        var tarantula = new SitemapTarantula(urls);

        // Act
        var doc = tarantula.CreateXMLDocumentSitemap();

        // Assert
        Assert.NotNull(doc);
        Assert.Equal(3, tarantula.Count);

        var root = doc.Root;
        Assert.NotNull(root);
        Assert.Equal("urlset", root.Name.LocalName);

        var urlElements = root.Elements();
        Assert.Equal(3, urlElements.Count());
    }

    [Fact]
    public void CreateXMLDocumentSitemap_FiltersCorrectly()
    {
        // Arrange
        var urls = new List<string>
        {
            "https://example.com/page1",
            "https://example.com/tag/noticias",
            "https://example.com/page2"
        };
        var filters = new List<string> { "/tag/" };
        var tarantula = new SitemapTarantula(urls, filters);

        // Act
        var doc = tarantula.CreateXMLDocumentSitemap();

        // Assert
        Assert.NotNull(doc);
        Assert.Equal(2, tarantula.Count); // "/tag/" should be filtered out
    }
}
