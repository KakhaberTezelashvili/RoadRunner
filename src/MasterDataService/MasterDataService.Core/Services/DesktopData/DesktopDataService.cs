using TDOC.Common.Data.Constants.DesktopData;
using TDOC.Data.Enumerations;

namespace MasterDataService.Core.Services.DesktopData;

/// <inheritdoc cref="IDesktopDataService" />
public class DesktopDataService : IDesktopDataService
{
    private readonly IDesktopDataRepository _desktopDataRepository;
    private readonly IDesktopDataValidator _desktopDataValidator;

    /// <summary>
    /// Initializes a new instance of the <see cref="DesktopDataService" /> class.
    /// </summary>
    /// <param name="desktopDataRepository">Repository provides methods to retrieve/handle desktop data.</param>
    /// <param name="desktopDataValidator">Validator provides methods to validate desktop data.</param>
    public DesktopDataService(IDesktopDataRepository desktopDataRepository, IDesktopDataValidator desktopDataValidator)
    {
        _desktopDataRepository = desktopDataRepository;
        _desktopDataValidator = desktopDataValidator;
    }

    /// <inheritdoc />
    public async Task SetComponentStateAsync(int userKeyId, string identifier, string data)
    {
        _desktopDataValidator.SetComponentStateValidate(identifier, data);

        DesktopModel desktopData = await _desktopDataRepository.GetByUserKeyIdAndDataIdentifierAsync(userKeyId, identifier);
        if (desktopData == null)
        { 
            await _desktopDataRepository.AddAsync(new DesktopModel()
            {
                AppType = TDOCAppType.WebServer,
                Version = DesktopDataFormatVersion.CurrentDesktopDataFormatVersion,
                Time = DateTime.Now,
                UserKeyId = userKeyId,
                DataIdentifier = identifier,
                Data = data
            });
        }
        else
        {
            desktopData.Data = data;
            await _desktopDataRepository.UpdateAsync(desktopData);
        }
    }

    /// <inheritdoc />
    public async Task<DesktopModel> GetComponentStateAsync(int userKeyId, string identifier)
    {
        _desktopDataValidator.GetComponentStateValidate(identifier);
        return await _desktopDataRepository.GetByUserKeyIdAndDataIdentifierAsync(userKeyId, identifier);
    }
}