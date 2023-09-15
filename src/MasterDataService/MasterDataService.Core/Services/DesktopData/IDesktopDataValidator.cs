namespace MasterDataService.Core.Services.DesktopData;

/// <summary>
/// Validator provides methods to validate desktop data.
/// </summary>
public interface IDesktopDataValidator
{
    /// <summary>
    /// Validates get component state arguments.
    /// </summary>
    /// <param name="identifier">Component identifier.</param>
    void GetComponentStateValidate(string identifier);

    /// <summary>
    /// Validates set component state arguments.
    /// </summary>
    /// <param name="identifier">Component identifier.</param>
    /// <param name="data">Component data.</param>
    void SetComponentStateValidate(string identifier, string data);
}