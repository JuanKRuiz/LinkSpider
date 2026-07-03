using System.Diagnostics;

namespace LinkSpiderLib;

[DebuggerDisplay("Url = {Url} , Explored = {Explored}")]
public class LinkElement
{
    public required string Url { get; set; }
    public bool Explored { get; set; }
}
