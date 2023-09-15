using MasterDataService.Shared.Constants.Media;
using MasterDataService.Shared.Dtos.Media;

namespace MasterDataService.Core.Repositories.Interfaces;

/// <summary>
/// Repository provides methods to retrieve/handle media series.
/// </summary>
public interface IMediaSeriesRepository
{
    /// <summary>
    /// Retrieves information about a media series associated with an object. If the object is a
    /// product, and there is no media series associated with it, the media series for the
    /// underlying item is retrieved instead.
    /// </summary>
    /// <param name="keyId">Key identifier of the linked object (see <paramref name="linkType"/>).</param>
    /// <param name="linkType">
    /// Identifies the type of object that the series is linked to (see <see
    /// cref="MediaSeriesLinks"/> for valid values).
    /// </param>
    /// <param name="seriesType">The type of series.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result is a collection of <see
    /// cref="MediaEntryDto"/> instances representing the media series.
    /// </returns>
    Task<IList<MediaEntryDto>> GetSeriesByKeyIdAndLinkTypeAndSeriesTypeAsync(int keyId, string linkType, int seriesType);
}