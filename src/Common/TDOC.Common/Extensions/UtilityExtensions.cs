namespace TDOC.Common.Extensions;

public static class UtilityExtensions
{
    public static bool In<T>(this T val, params T[] values) where T : struct => values.Contains(val);

    public static string FirstCharToLowerCase(this string value)
    {
        if (!string.IsNullOrEmpty(value) && char.IsUpper(value[0]))
            return value.Length == 1 ? char.ToLower(value[0]).ToString() : char.ToLower(value[0]) + value[1..];

        return value;
    }

    public static string FirstCharToLowerCase(this Enum value) => value?.ToString().FirstCharToLowerCase();
}