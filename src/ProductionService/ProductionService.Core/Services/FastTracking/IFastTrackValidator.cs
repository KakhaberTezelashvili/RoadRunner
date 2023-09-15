namespace ProductionService.Core.Services.FastTracking;

/// <summary>
/// Validator provides methods to validate fast track data.
/// </summary>
public interface IFastTrackValidator : IValidatorBase<UnitFastTrackModel>
{
    /// <summary>
    /// Validates unit fast track display info.
    /// </summary>
    /// <param name="unitKeyId">Unit key identifier.</param>
    void UnitFastTrackDisplayInfoValidate(int unitKeyId);
}