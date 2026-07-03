using System;
using System.Text.RegularExpressions;

namespace LinkSpiderConsole;

internal static class ConsoleExt
{
    public static void WriteTitle(string title, bool emphasis = false)
    {
        var line = Regex.Replace(title, ".", "=");
        Console.WriteLine();
        if (emphasis)
        {
            Console.WriteLine(line);
        }

        Console.WriteLine(title);
        Console.WriteLine(line);
        Console.WriteLine();
    }
}
