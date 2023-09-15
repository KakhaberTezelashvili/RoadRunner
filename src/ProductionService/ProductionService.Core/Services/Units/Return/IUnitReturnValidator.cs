using ProductionService.Shared.Dtos.Units;

namespace ProductionService.Core.Services.Units.Return;

/// <summary>
/// Validator provides methods to validate unit return data.
/// </summary>
public interface IUnitReturnValidator
{
    /// <summary>
    /// Validates returning unit.
    /// </summary>
    /// <param name="userKeyId">User key identifier.</param>
    /// <param name="args">Unit return arguments.</param>
    /// <returns>If the validation passes, unit will be returned.</returns>
    Task<UnitModel> ReturnValidateAsync(int userKeyId, UnitReturnArgs args);
}