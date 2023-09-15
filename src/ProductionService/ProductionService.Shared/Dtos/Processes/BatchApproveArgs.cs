namespace ProductionService.Shared.Dtos.Processes;

/// <summary>
/// Input arguments needed to approve a batch.
/// </summary>
/// <param name="LocationKeyId">Location key id.</param>
/// <param name="PositionLocationKeyId">Position/Location key id.</param>
public record BatchApproveArgs(int LocationKeyId, int PositionLocationKeyId);