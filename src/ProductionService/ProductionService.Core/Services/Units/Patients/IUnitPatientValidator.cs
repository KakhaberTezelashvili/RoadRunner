using ProductionService.Shared.Dtos.Units;

namespace ProductionService.Core.Services.Units.Patients;

public interface IUnitPatientValidator : IValidatorBase<UnitModel>
{
    /// <summary>
    /// Validates updating the unit with the specified patient.
    /// </summary>
    /// <param name="args">Unit patient arguments.</param>
    /// <param name="unitKeyId">Unit key identifier.</param>
    /// <param name="userKeyId">User key identifier.</param>
    /// <returns>If the validation passes, unit will be returned.</returns>
    Task<UnitModel> UpdatePatientValidateAsync(int unitKeyId, int userKeyId, UnitPatientArgs args);
}