using TDOC.Common.Data.Constants.DesktopData;
using TDOC.Data.Enumerations;

namespace MasterDataService.Infrastructure.Repositories;

/// <inheritdoc cref="IDesktopDataRepository" />
public class DesktopDataRepository : RepositoryBase<DesktopModel>, IDesktopDataRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DesktopDataRepository" /> class.
    /// </summary>
    /// <param name="context">EF database context.</param>
    public DesktopDataRepository(TDocEFDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<DesktopModel> GetByUserKeyIdAndDataIdentifierAsync(int userKeyId, string dataIdentifier)
    {
        return await _context.DesktopData.AsNoTracking()
            .Where(d => d.AppType == TDOCAppType.WebServer && d.UserKeyId == userKeyId &&
                        d.Version == DesktopDataFormatVersion.CurrentDesktopDataFormatVersion &&
                        d.DataIdentifier == dataIdentifier)
            .FirstOrDefaultAsync();
    }

    /// <inheritdoc />
    public async Task<DesktopModel> GetWebServerDesktopOptionsAsync()
    {
        return await _context.DesktopData.AsNoTracking()
            .Where(d => d.AppType == TDOCAppType.WebServer && d.UserKeyId == null &&
                        d.Version == DesktopDataFormatVersion.CurrentDesktopDataFormatVersion &&
                        d.DataIdentifier == DesktopDataIdentifiers.ServerDesktopOptions)
            .FirstOrDefaultAsync();
    }

    /// <inheritdoc />
    public async Task<DesktopModel> GetWebUserDesktopOptionsAsync(int userKeyId)
    {
        return await _context.DesktopData.AsNoTracking()
            .Where(d => d.AppType == TDOCAppType.WebServer && d.UserKeyId == userKeyId &&
                        d.Version == DesktopDataFormatVersion.CurrentDesktopDataFormatVersion &&
                        d.DataIdentifier == DesktopDataIdentifiers.UserDesktopOptions)
            .FirstOrDefaultAsync();
    }

    /// <inheritdoc />
    public async Task<DesktopModel> GetWebDefaultUserDesktopOptionsAsync()
    {
        return await _context.DesktopData.AsNoTracking()
            .Where(d => d.AppType == TDOCAppType.WebServer && d.UserKeyId == null &&
                        d.Version == DesktopDataFormatVersion.CurrentDesktopDataFormatVersion &&
                        d.DataIdentifier == DesktopDataIdentifiers.DefaultUserDesktopOptions)
            .FirstOrDefaultAsync();
    }
}