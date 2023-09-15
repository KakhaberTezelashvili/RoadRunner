namespace TDOC.Common.Client.Exceptions;

/// <summary>
/// Service provides methods to handle exceptions.
/// </summary>
public interface IExceptionService
{
    /// <summary>
    /// Publishes error message to mediator.
    /// </summary>
    /// <param name="exception">Exception.</param>
    /// <returns>Task.</returns>
    Task ShowException(Exception exception);

    /// <summary>
    /// Prepare error details.
    /// </summary>
    /// <param name="notification">Error messages notification.</param>
    /// <param name="errorMessagesLocalizer">Error messages resources string localizer.</param>
    /// <param name="sharedLocalizer">Shared resources string localizer.</param>
    /// <param name="enumsLocalizer">Enumerations resources string localizer.</param>
    /// <param name="tablesLocalizer">Tables resources string localizer.</param>
    /// <param name="exceptionalColumnsLocalizer">Exceptional columns resources string localizer.</param>
    /// <param name="title">Title of error message.</param>
    /// <param name="description">Description of error message.</param>
    void PrepareErrorDetails(
        ErrorMessagesNotification notification,
        IStringLocalizer errorMessagesLocalizer, 
        IStringLocalizer sharedLocalizer, 
        IStringLocalizer enumsLocalizer,
        IStringLocalizer tablesLocalizer,
        IStringLocalizer exceptionalColumnsLocalizer,
        out string title, out string description);
}