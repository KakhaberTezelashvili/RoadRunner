namespace TDOC.Common.Extensions;

public static class LocalizationExtensions
{
    public static string LocalizeDate(this DateTime value) => value.ToString("dd-MM-yyyy");

    public static string LocalizeDateTime(this DateTime value) => value.ToString("dd-MM-yyyy HH:mm:ss");

    public static string LocalizeShortDateTime(this DateTime value) => value.ToString("dd-MM-yyyy HH:mm");
}