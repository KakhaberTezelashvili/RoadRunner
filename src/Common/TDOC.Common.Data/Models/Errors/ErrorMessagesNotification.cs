using MediatR;

namespace TDOC.Common.Data.Models.Errors
{
    /// <summary>
    /// General <see cref="MediatR"/> data notification containing the error details.
    /// </summary>
    public class ErrorMessagesNotification : INotification
    {
        public IEnumerable<ValidationError> Errors { get; }

        /// <summary>
        /// The constructor for the general <see cref="MediatR"/> data notification
        /// </summary>
        /// <param name="errors">List of error messages.</param>
        public ErrorMessagesNotification(IEnumerable<ValidationError> errors)
        {
            Errors = errors;
        }
    }
}