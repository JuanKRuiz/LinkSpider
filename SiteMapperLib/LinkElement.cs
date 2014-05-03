using System.Diagnostics;

namespace LinkSpiderLib
{
    [DebuggerDisplay("url = {url} , explored = {explored}")]
    public class LinkElement
    {
        public string url = string.Empty;
        public bool explored = false;
    }
}
