﻿using System.Collections.Generic;

namespace LinkSpiderLib
{
                    
    public class LinkElementComparer: IEqualityComparer<LinkElement>
    {
        public bool Equals(LinkElement x, LinkElement y)
        {
            return x.url == y.url;
        }

        public int GetHashCode(LinkElement obj)
        {
            return obj.url.GetHashCode();
        }
    }
}
