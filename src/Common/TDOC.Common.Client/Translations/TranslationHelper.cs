namespace TDOC.Common.Client.Translations;

public static class TranslationHelper
{
    /// <summary>
    /// Returns name of the specified enum value.
    /// </summary>
    /// <param name="value">Enum value.</param>
    /// <param name="sharedLocalizer">Shared resources string localizer.</param>
    /// <param name="enumsLocalizer">Enumerations resources string localizer.</param>
    /// <returns>Name of the enum value if found, "Unknown" otherwise.</returns>
    public static string GetEnumValueName<T>(object value, IStringLocalizer sharedLocalizer, IStringLocalizer enumsLocalizer) where T : Enum =>
        EnumValueName(value, sharedLocalizer, enumsLocalizer, typeof(T));

    /// <summary>
    /// Returns name of the specified enum value.
    /// </summary>
    /// <param name="type">Enum type.</param>
    /// <param name="value">Enum value.</param>
    /// <param name="sharedLocalizer">Shared resources string localizer.</param>
    /// <param name="enumsLocalizer">Enumerations resources string localizer.</param>
    /// <returns>Name of the enum value if found, "Unknown" otherwise.</returns>
    public static string GetEnumValueNameFromString(string type, int value, IStringLocalizer sharedLocalizer, IStringLocalizer enumsLocalizer) =>
        EnumValueName(value, sharedLocalizer, enumsLocalizer, Type.GetType(type));

    private static string EnumValueName(object value, IStringLocalizer sharedLocalizer, IStringLocalizer enumsLocalizer, Type enumType)
    {
        string valueName = Enum.GetName(enumType, value);
        if (valueName != null)
            return enumsLocalizer != null ? enumsLocalizer[$"{enumType.Name}.{valueName}"] : valueName;

        return sharedLocalizer != null ? sharedLocalizer["unknown"] : "Unknown";
    }
}