namespace ProductionService.Core.Services.Texts;

/// <summary>
/// Validator provides methods to validate error codes, control codes etc.
/// </summary>
public interface ITextValidator : IValidatorBase<TextModel>
{
    /// <summary>
    /// Validates error data.
    /// </summary>
    /// <param name="errorNo">Error number.</param>
    /// <param name="minErrorNo">The lowest error number that is allowed.</param>
    /// <returns>Task&lt;TextModel&gt;.</returns>
    Task<TextModel> GetErrorValidateAsync(int errorNo, int minErrorNo = 0);
}