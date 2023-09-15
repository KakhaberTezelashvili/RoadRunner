using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace TDOC.Common.Client.Extensions
{
    public static class NavigationManagerExtensions
    {
        /// <summary>
        /// Checks if the URI of <see cref="NavigationManager"/> contains the specified parameter.
        /// </summary>
        /// <typeparam name="T">Type of the specified parameter.</typeparam>
        /// <param name="navigation"><see cref="NavigationManager"/>.</param>
        /// <param name="key">Parameter name.</param>
        /// <param name="value">Parameter value if the parameter exists, or the default value if not.</param>
        /// <returns><c>true</c> if the URI contains the specified parameter and ; <c>false</c> otherwise.</returns>
        public static bool TryGetQueryString<T>(this NavigationManager navigation, string key, out T value)
        {
            Uri uri = navigation.ToAbsoluteUri(navigation.Uri);

            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue(key, out StringValues valueFromQueryString))
            {
                if (typeof(T) == typeof(int) && int.TryParse(valueFromQueryString, out int valueAsInt))
                {
                    value = (T)(object)valueAsInt;
                    return true;
                }

                if (typeof(T) == typeof(string))
                {
                    value = (T)(object)valueFromQueryString.ToString();
                    return true;
                }

                if (typeof(T) == typeof(decimal) && decimal.TryParse(valueFromQueryString, out decimal valueAsDecimal))
                {
                    value = (T)(object)valueAsDecimal;
                    return true;
                }
            }

            value = default;
            return false;
        }
    }
}
