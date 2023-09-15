using TDOC.Common.Data.Enumerations.Messages;

namespace TDOC.Common.Client.Translations;

/// <summary>
/// Service provides methods to handle translations.
/// </summary>
public interface ITranslationService
{
    /// <summary>
    /// Prepares error message according to validation error.
    /// </summary>
    /// <param name="error">Validation error.</param>
    /// <param name="messageType">Message type.</param>
    /// <param name="errorMessagesLocalizer">Error messages resources string localizer.</param>
    /// <param name="sharedLocalizer">Shared resources string localizer.</param>
    /// <param name="enumsLocalizer">Enumerations resources string localizer.</param>
    /// <param name="tablesLocalizer">Tables resources string localizer.</param>
    /// <param name="exceptionalColumnsLocalizer">Exceptional columns resources string localizer.</param>
    /// <returns>Error message.</returns>
    string PrepareErrorMessage(ValidationError error, MessageType messageType, IStringLocalizer errorMessagesLocalizer, 
        IStringLocalizer sharedLocalizer, IStringLocalizer enumsLocalizer, IStringLocalizer tablesLocalizer, 
        IStringLocalizer exceptionalColumnsLocalizer);

    /// <summary>
    /// Translate columns display names.
    /// </summary>
    /// <param name="columnConfigs">Collection of grid column configuration.</param>
    /// <param name="tablesLocalizer">Tables resources string localizer.</param>
    /// <param name="exceptionalColumnsLocalizer">Exceptional columns resources string localizer.</param>
    void TranslateColumnsDisplayNames(IList<GridColumnConfiguration> columnConfigs,
        IStringLocalizer tablesLocalizer, IStringLocalizer exceptionalColumnsLocalizer);
}