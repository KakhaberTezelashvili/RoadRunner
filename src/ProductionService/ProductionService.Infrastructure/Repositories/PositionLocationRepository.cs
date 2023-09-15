namespace ProductionService.Infrastructure.Repositories;

/// <inheritdoc cref="IPositionLocationRepository" />
public class PositionLocationRepository : RepositoryBase<PosLocationModel>, IPositionLocationRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PositionLocationRepository" /> class.
    /// </summary>
    /// <param name="context">EF database context.</param>
    public PositionLocationRepository(TDocEFDbContext context) : base(context)
    {
    }
}