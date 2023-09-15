namespace ProductionService.Core.Services.Units.Locations;

/// <summary>
/// Service provides methods to retrieve/handle unit locations.
/// </summary>
public interface IUnitLocationService
{
    /// <summary>
    /// Adds unit location.
    /// </summary>
    /// <param name="unitKeyId">Unit key identifier.</param>
    /// <param name="userKeyId">User key identifier.</param>
    /// <param name="locationKeyId">Location key identifier.</param>
    /// <param name="positionLocationKeyId">Position location key identifier.</param>
    /// <param name="locationTime">Location time.</param>
    /// <param name="what">Signifies what happened.</param>
    /// <param name="error">Error no.</param>
    Task AddAsync(int unitKeyId, int userKeyId, int locationKeyId, int positionLocationKeyId, DateTime locationTime, WhatType what, int error = 0);

    /// <summary>
    /// Updates unit location.
    /// </summary>
    /// <param name="batchKeyId">Batch key identifier.</param>
    /// <param name="batchType">Batch type.</param>
    /// <param name="unitKeyId">Unit key identifier.</param>
    /// <param name="locationKeyId">Location key identifier.</param>
    /// <param name="locationTime">Location time.</param>
    Task UpdateAsync(int batchKeyId, BatchType batchType, int unitKeyId, int locationKeyId, DateTime locationTime);
}