namespace ProductionService.Shared.Dtos.Processes;

/// <summary>
/// Input arguments to disapprove a batch.
/// </summary>
/// <param name="LocationKeyId">Location key identifier.</param>
/// <param name="PositionLocationKeyId">Position/Location key identifier.</param>
/// <param name="Error">Error number.</param>
public record BatchDisapproveArgs(int LocationKeyId, int PositionLocationKeyId, int Error);