using System.IO;
using System.Text;

namespace LinkSpiderLib;

internal sealed class StringWriterUTF8 : StringWriter
{
    public override Encoding Encoding => Encoding.UTF8;
}
