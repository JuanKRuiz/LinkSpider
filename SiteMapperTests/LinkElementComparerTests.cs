using LinkSpiderLib;
using Xunit;

namespace SiteMapperTests;

public class LinkElementComparerTests
{
    [Fact]
    public void Equals_SameUrls_ReturnsTrue()
    {
        // Arrange
        var comparer = new LinkElementComparer();
        var item1 = new LinkElement { Url = "https://example.com/page1", Explored = false };
        var item2 = new LinkElement { Url = "https://example.com/page1", Explored = true };

        // Act & Assert
        Assert.True(comparer.Equals(item1, item2));
    }

    [Fact]
    public void Equals_CaseInsensitiveUrls_ReturnsTrue()
    {
        // Arrange
        var comparer = new LinkElementComparer();
        var item1 = new LinkElement { Url = "https://example.com/PAGE1", Explored = false };
        var item2 = new LinkElement { Url = "https://example.com/page1", Explored = false };

        // Act & Assert
        Assert.True(comparer.Equals(item1, item2));
    }

    [Fact]
    public void GetHashCode_SameUrls_ReturnsSameHashCode()
    {
        // Arrange
        var comparer = new LinkElementComparer();
        var item1 = new LinkElement { Url = "https://example.com/page1", Explored = false };
        var item2 = new LinkElement { Url = "https://example.com/PAGE1", Explored = false };

        // Act & Assert
        Assert.Equal(comparer.GetHashCode(item1), comparer.GetHashCode(item2));
    }
}
