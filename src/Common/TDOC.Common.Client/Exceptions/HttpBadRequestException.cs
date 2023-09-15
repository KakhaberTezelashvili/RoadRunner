namespace TDOC.Common.Client.Exceptions
{
    /// <summary>
    /// HTTP BadRequest exception class. Implements the <see cref="Exception"/>
    /// </summary>
    /// <seealso cref="Exception"/>
    public class HttpBadRequestException : Exception
    {
        /// <summary>
        /// Gets the errors.
        /// </summary>
        /// <value>The errors.</value>
        public IEnumerable<ValidationError> Errors { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpBadRequestException" /> class.
        /// </summary>
        /// <param name="error">The error.</param>
        public HttpBadRequestException(ValidationError error) : this(new[] { error })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpBadRequestException" /> class.
        /// </summary>
        /// <param name="errors">The errors.</param>
        public HttpBadRequestException(IEnumerable<ValidationError> errors)
        {
            Errors = errors;
        }
    }
}