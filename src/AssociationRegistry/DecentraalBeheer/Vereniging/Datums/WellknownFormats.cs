namespace AssociationRegistry.Formats;

using System.Globalization;

public static class WellknownFormats
{
    public const string TimeOnly = "HH:mm:ss";
    public const string DateOnly = "yyyy-MM-dd";
    public const string DateAndTime = "yyyy-MM-dd HH:mm";
    public static readonly CultureInfo België = CultureInfo.GetCultureInfo("nl-BE");
}
