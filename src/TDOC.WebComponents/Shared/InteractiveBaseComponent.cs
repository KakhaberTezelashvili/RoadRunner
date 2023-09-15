using TDOC.WebComponents.Shared.Models;
using TDOC.WebComponents.Shared.Enumerations;
using TDOC.Common.Data.Models.Validations;

namespace TDOC.WebComponents.Shared;

/// <summary>
/// Contains validation and data changed notification functionality.
/// </summary>
public class InteractiveBaseComponent : ComponentBase
{
    [Inject]
    protected CustomTimer Timer { get; set; }

    [Inject]
    private IMediator _mediator { get; set; }

    /// <summary>
    /// Caption. Can be used in the formation of validation text.
    /// </summary>
    [Parameter]
    public string CaptionText { get; set; }

    /// <summary>
    /// Notification mode <see cref="DataChangedNotificationMode" />.
    /// </summary>
    [Parameter]
    public DataChangedNotificationMode DataChangedNotificationMode
    { 
        get => dataChangedNotificationMode; 
        set
        {
            if (dataChangedNotificationMode != value)
            {
                dataChangedNotificationMode = value;
                DataChanged = false;
            }
        } 
    }

    /// <summary>
    /// Function for additional validation.
    /// </summary>
    [Parameter]
    public Func<string, InputValidationResult> ValidateData { get; set; }

    /// <summary>
    /// Function for obtaining custom text for required validation.
    /// </summary>
    [Parameter]
    public Func<string, string> ObtainRequiredValidationText { get; set; }

    /// <summary>
    /// Required flag.
    /// </summary>
    [Parameter]
    public bool Required { get; set; }

    /// <summary>
    /// Default timer delay in milisecond.
    /// </summary>
    protected const int DefaultTimerDelayOnChange = 100;

    /// <summary>
    /// Data changed.
    /// </summary>
    public bool DataChanged { get; protected set; }

    /// <summary>
    /// Get Validation text.
    /// </summary>
    public string ValidationText => validationText;

    /// <summary>
    /// Valid state.
    /// </summary>
    public bool IsValid { get; set; } = true;

    private string validationText;
    private DataChangedNotificationMode dataChangedNotificationMode = DataChangedNotificationMode.Never;

    /// <summary>
    /// Validates the component and updates IsValid state.
    /// </summary>
    /// <returns><c>true</c> if the validation is valid, <c>false</c> otherwise.</returns>
    public virtual bool Validate()
    {
        if (!Required && ValidateData == null)
            return true;

        IsValid = IsRequiredValid();
        if (IsValid && ValidateData != null)
        {
            var validationResult = ValidateData(CaptionText);
            IsValid = validationResult.Success;
            validationText = validationResult.ErrorMessage;
        }
        else if (!IsValid)
            validationText = ObtainRequiredValidationText(CaptionText);

        return IsValid;
    }

    /// <summary>
    /// Resets all validation related properties and apply them.
    /// </summary>
    public virtual void ResetValidation()
    {
        IsValid = true;
        DataChanged = false;
        ApplyInputCssStyles();
    }

    /// <summary>
    /// Applies input CSS styles.
    /// </summary>
    protected virtual void ApplyInputCssStyles() => StateHasChanged();

    /// <summary>
    /// Publishes <see cref="DataChangedNotification"/> based on <see cref="DataChangedNotificationMode" />.
    /// </summary>
    protected async Task NotifyDataChangedAsync()
    {
        switch (DataChangedNotificationMode)
        {
            case DataChangedNotificationMode.Once:
                if (!DataChanged)
                {
                    DataChanged = true;
                    await _mediator.Publish(new ComponentDataChangedNotification());
                }
                break;
            case DataChangedNotificationMode.Always:
                DataChanged = true;
                await _mediator.Publish(new ComponentDataChangedNotification());
                break;
        }
    }

    /// <summary>
    /// Is required validation valid.
    /// <para><strong>Caution:</strong> The method always returns true. Override for custom implementation.</para>
    /// </summary>
    /// <returns><c>true</c> if the required validation is valid, <c>false</c> otherwise.</returns>
    protected virtual bool IsRequiredValid() => true;
}