using Newtonsoft.Json.Linq;
using TDOC.Common.Data.Models.Media;

namespace TDOC.WebComponents.Media.Models;

public class MediaIconPopoverDetails
{
    public string LinkType { get; init; }

    public string PopupTitle { get; init; }

    public string PopupButtonCancelText { get; init; }

    public Func<int, bool, string> GetImageUrl { get; init; }

    public EventCallback<MediaEntryData> ObtainMediaData { get; init; }

    public Func<int, string, int, Task<IList<MediaEntryData>>> GetEntryList { get; init; }

    public Func<JObject, int> GetMainEntityKeyId { get; init; }

    public int GetMediaKeyId(JObject item, string mediaFieldName) => item[mediaFieldName].Value<int?>() ?? 0;
}