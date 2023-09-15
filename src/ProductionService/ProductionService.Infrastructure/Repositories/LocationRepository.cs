namespace ProductionService.Infrastructure.Repositories;

/// <inheritdoc cref="ILocationRepository" />
public class LocationRepository : RepositoryBase<LocationModel>, ILocationRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LocationRepository" /> class.
    /// </summary>
    /// <param name="context">EF database context.</param>
    public LocationRepository(TDocEFDbContext context) : base(context)
    {
    }
}