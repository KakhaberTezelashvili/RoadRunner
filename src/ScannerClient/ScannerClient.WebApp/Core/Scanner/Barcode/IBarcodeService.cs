using Newtonsoft.Json.Linq;

namespace ScannerClient.WebApp.Core.Scanner.Barcode;

/// <summary>
/// Barcode actions service.
/// </summary>
public interface IBarcodeService
{
    /// <summary>
    /// Execute barcode action.
    /// </summary>
    /// <param name="data">Data.</param>
    /// <param name="mainEntity">Main entity full name.</param>
    /// <param name="mainEntityKey">Primary key field name of main entity.</param>
    /// <param name="rowClicked">Row clicked.</param>
    /// <returns></returns>
    Task ExecuteBarcodeActionAsync(JObject data, string mainEntity, string mainEntityKey, bool rowClicked);
}
