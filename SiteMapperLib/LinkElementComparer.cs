using System.Collections.Generic;

namespace LinkSpiderLib;

public class LinkElementComparer : IEqualityComparer<LinkElement>
{
    public bool Equals(LinkElement? x, LinkElement? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;
        return string.Equals(x.Url, y.Url, StringComparison.OrdinalIgnoreCase);
    }

    public int GetHashCode(LinkElement obj) => 
        StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Url);
}
