namespace MasterDataService.Core.Services.DesktopData;

/// <inheritdoc cref="IDesktopDataValidator" />
public class DesktopDataValidator : ValidatorBase<DesktopModel>, IDesktopDataValidator
{
    private readonly IDesktopDataRepository _desktopDataRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="DesktopDataValidator" /> class.
    /// </summary>
    /// <param name="desktopDataRepository">Repository provides methods to retrieve/handle desktop data.</param>
    public DesktopDataValidator(IDesktopDataRepository desktopDataRepository) : base(desktopDataRepository)
    {
        _desktopDataRepository = desktopDataRepository;
    }

    /// <inheritdoc />
    public void SetComponentStateValidate(string identifier, string data)
    {
        if (string.IsNullOrEmpty(identifier) || string.IsNullOrEmpty(data))
            throw ArgumentEmptyException();
    }

    /// <inheritdoc />
    public void GetComponentStateValidate(string identifier)
    {
        if (string.IsNullOrEmpty(identifier))
            throw ArgumentEmptyException();
    }
}