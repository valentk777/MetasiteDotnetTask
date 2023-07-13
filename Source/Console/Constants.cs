﻿using System.Diagnostics.CodeAnalysis;

namespace MetaApp.MetaAppConsole;

[ExcludeFromCodeCoverage]
public static class Constants
{
    public const string AppSettingsFile = "appsettings.json";

    public static class Commands
    {
        public const string Weather = "weather";
        public const string CityParameter = "--city";
    }
}
