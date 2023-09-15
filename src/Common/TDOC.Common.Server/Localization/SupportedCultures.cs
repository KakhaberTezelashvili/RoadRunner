using System.Globalization;

namespace TDOC.Common.Server.Localization
{
    /// <summary>
    /// Identifies all cultures supported by T-DOC.
    /// </summary>
    public static class SupportedCultures
    {
        public static string DefaultCultureName => "en-US";

        /// <summary>
        /// Retrieves all cultures (languages) supported by T-DOC.
        /// </summary>
        /// <returns>Collection of <see cref="CultureInfo"/> instances.</returns>
        public static List<CultureInfo> AsList()
        {
            var cultureList = new List<CultureInfo>
            {
                new(DefaultCultureName),
                new("da-DK"),
                new("sv-SE"),
                new("nn-NO"),
                new("fi-FI"),
                new("de-DE"),
                new("fr-FR"),
                new("es-ES"),
                new("it-IT"),
                new("nl-NL"),
                new("pl-PL"),
                new("ja-JP"),
                new("zh-Hans"),
                new("ru-RU"),
                new("bg-BG"),
                new("hu-HU"),
                new("pt-BR"),
                new("sl-SI"),
                new("ro-RO"),
                new("th-TH"),
                new("tr-TR"),
                new("el-GR"),
                new("ms-MY"),
                new("lt-LT"),
                new("cs-CZ"),
                new("et-EE"),
                new("lv-LV"),
                new("sk-SK"),
                new("ko-KR"),
                new("pt-PT"),
                new("zh-Hant")
            };

            try
            {
                // TODO: Further investigate potential issues with inexistent Cultures (Task #20274)
                cultureList.Add(new CultureInfo("es-419"));
            }
            catch (Exception)
            {
                // we will "eat" the exception
            }
            return cultureList;
        }
    }
}