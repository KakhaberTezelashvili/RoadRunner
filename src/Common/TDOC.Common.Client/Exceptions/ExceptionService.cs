using System.Text;
using TDOC.Common.Client.Translations;
using TDOC.Common.Data.Enumerations.Messages;
using TDOC.Common.Data.Models.Exceptions;

namespace TDOC.Common.Client.Exceptions;

/// <inheritdoc cref="IExceptionService" />
public class ExceptionService : IExceptionService
{
    private readonly IMediator _mediator;
    private readonly ITranslationService _translationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionService" /> class.
    /// </summary>
    /// <param name="mediator">MediatR.</param>
    /// <param name="translationService">Translation service.</param>
    public ExceptionService(IMediator mediator, ITranslationService translationService)
    {
        _mediator = mediator;
        _translationService = translationService;
    }

    /// <inheritdoc />
    public async Task ShowException(Exception exception)
    {
        try
        {
            switch (exception)
            {
                case null:
                    break;

                case DomainException ex:
                    await _mediator.Publish(new ErrorMessagesNotification(new[]
                    {
                        new DomainValidationError
                        {
                            Code = ex.Code,
                            Details = ex.Details
                        }
                    }));
                    break;

                case HttpBadRequestException ex:
                    await _mediator.Publish(new ErrorMessagesNotification(ex.Errors));
                    break;

                default:
                    Serilog.Log.Fatal(exception, exception.Message);
                    await _mediator.Publish(new ErrorMessagesNotification(new[] { new ValidationError { Message = "An error has occurred!" } }));
                    break;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    /// <inheritdoc />
    public void PrepareErrorDetails(
        ErrorMessagesNotification notification, 
        IStringLocalizer errorMessagesLocalizer, 
        IStringLocalizer sharedLocalizer, 
        IStringLocalizer enumsLocalizer,
        IStringLocalizer tablesLocalizer,
        IStringLocalizer exceptionalColumnsLocalizer,
        out string title, out string description)
    {
        StringBuilder errorTitle = new();
        StringBuilder errorDescription = new();
        foreach (ValidationError error in notification.Errors)
        {
            if (error.Code != null)
            {
                AppendError(errorTitle, 
                    _translationService.PrepareErrorMessage(error, MessageType.Title, errorMessagesLocalizer, sharedLocalizer, enumsLocalizer, tablesLocalizer, exceptionalColumnsLocalizer));
                AppendError(errorDescription, 
                    _translationService.PrepareErrorMessage(error, MessageType.Description, errorMessagesLocalizer, sharedLocalizer, enumsLocalizer, tablesLocalizer, exceptionalColumnsLocalizer));
            }
            else
            {
                AppendError(errorTitle, error.Message);
                AppendError(errorDescription, error.Description);
            }
        }
        title = errorTitle.ToString();
        description = errorDescription.ToString();
    }

    private void AppendError(StringBuilder error, string message)
    {
        if (!string.IsNullOrEmpty(message))
        {
            if (error.Length > 0)
                error.Append("<br>");
            error.Append(message);
        }
    }
}