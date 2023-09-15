namespace ProductionService.Client.Services.Units.Errors;

/// <summary>
/// API service provides methods to retrieve/handle unit error data.
/// </summary>
public interface IUnitErrorApiService
{
    /// <summary>
    /// Updates the unit with the specified error.
    /// </summary>
    /// <param name="unitKeyId">Unit key identifier</param>
    /// <param name="errorNumber">Error number.</param>
    Task UpdateErrorAsync(int unitKeyId, int errorNumber);
}