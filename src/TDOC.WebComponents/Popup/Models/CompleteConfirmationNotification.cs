using TDOC.WebComponents.Popup.Enumerations;

namespace TDOC.WebComponents.Popup.Models;

/// <summary>
/// General <see cref="MediatR"/> data notification used for confirmation popup result.
/// </summary>
public class CompleteConfirmationNotification : INotification
{
    /// <summary>
    /// Confirmation result.
    /// </summary>
    public ConfirmationResult Result { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CompleteConfirmationNotification" /> class.
    /// </summary>
    /// <param name="result"> The <see cref="ConfirmationResult" /> of the user's choice.</param>
    public CompleteConfirmationNotification(ConfirmationResult result)
    {
        Result = result;
    }
}