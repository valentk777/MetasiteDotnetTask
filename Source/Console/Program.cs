using System.Diagnostics.CodeAnalysis;

namespace MetaApp.MetaAppConsole;

[ExcludeFromCodeCoverage]
public class Program
{
    public static void Main(string[] args)
    {
        WeatherApp.Run(args);

        Console.ReadLine();
    }
}
