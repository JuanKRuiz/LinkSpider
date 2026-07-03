using System.Diagnostics;

namespace LinkSpiderLib;

[DebuggerDisplay("Url = {Url} , Explored = {Explored}")]
public class LinkElement
{
    public required string Url
    {
        get => field;
        set => field = value?.Trim() ?? string.Empty;
    }
    public bool Explored { get; set; }
}
