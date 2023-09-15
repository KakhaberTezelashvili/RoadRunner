using MasterDataService.Shared.Dtos.Media;
using TDOC.Common.Data.Enumerations.Media;
using TDOC.Common.Data.Models.Media;

namespace MasterDataService.Client.Services.Media;

/// <inheritdoc cref="IMediaApiService" />
public class MediaApiService : IMediaApiService
{
    private const string urlPathBase = "media";
    private const string identifierThumbnail = "thumbnail";
    private readonly ITypedHttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="MediaApiService" /> class.
    /// </summary>
    /// <param name="httpClientFactory">Http client factory.</param>
    public MediaApiService(ITypedHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(HttpClientConfigurationName.MasterDataClient);
    }

    /// <inheritdoc />
    public string GetImageUrl(int keyId, bool thumbnail = false) =>
        $"{_httpClient.ClientBaseAddress()}{urlPathBase}/{keyId}/image?thumbnail={thumbnail}";

    /// <inheritdoc />
    public string GetVideoUrl(int keyId, string mode = "") =>
        $"{_httpClient.ClientBaseAddress()}{urlPathBase}/{keyId}/video?mode={mode}";

    /// <inheritdoc />
    public string GetAudioUrl(int keyId) =>
        $"{_httpClient.ClientBaseAddress()}{urlPathBase}/{keyId}/audio";

    /// <inheritdoc />
    public async Task<byte[]> GetImageAsync(int keyId, bool thumbnail = false) =>
        await _httpClient.GetBytesAsync(GetImageUrl(keyId, thumbnail));

    /// <inheritdoc />
    public async Task<byte[]> GetVideoAsync(int keyId, string mode = "") =>
        await _httpClient.GetBytesAsync(GetVideoUrl(keyId, mode));

    /// <inheritdoc />
    public async Task<byte[]> GetAudioAsync(int keyId) =>
        await _httpClient.GetBytesAsync(GetAudioUrl(keyId));

    /// <inheritdoc />
    public async Task<string> GetTextAsync(int keyId) =>
        await _httpClient.GetStringAsync($"{urlPathBase}/{keyId}/text");

    /// <inheritdoc />
    public async Task<IList<MediaEntryDto>> GetEntryListAsync(int keyId, string linkType, int seriesType = 1) =>
        await _httpClient.GetAsync<IList<MediaEntryDto>>($"{urlPathBase}/entry-list?keyId={keyId}&linkType={linkType}&seriesType={seriesType}");

    /// <inheritdoc />
    public async Task ObtainMediaDataAsync(MediaEntryData entryData)
    {
        foreach (MediaItemType mediaItem in entryData.Types)
        {
            switch (mediaItem.Type)
            {
                case nameof(MediaFormat.Image):
                    // 'data:image/*;base64, ' + Convert.ToBase64String(await MediaService.GetImageAsync(entryData.KeyId, mediaItem.Identifier))
                    if (string.IsNullOrEmpty(entryData.Thumbnail) & (mediaItem.Identifier == identifierThumbnail))
                        entryData.Thumbnail = GetImageUrl(entryData.KeyId, true);
                    else if (string.IsNullOrEmpty(entryData.Image))
                        entryData.Image = GetImageUrl(entryData.KeyId, false);
                    break;
                case nameof(MediaFormat.Video):
                    if (string.IsNullOrEmpty(entryData.Video))
                        entryData.Video = GetVideoUrl(entryData.KeyId, "");
                    break;
                case nameof(MediaFormat.Audio):
                    if (string.IsNullOrEmpty(entryData.Audio))
                        entryData.Audio = GetAudioUrl(entryData.KeyId);
                    break;
                case nameof(MediaFormat.Text):
                    if (string.IsNullOrEmpty(entryData.Text))
                        entryData.Text = await GetTextAsync(entryData.KeyId);
                    break;
            }
        }
    }
}
