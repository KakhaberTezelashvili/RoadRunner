namespace ProductionService.Core.Services.Serials;

/// <inheritdoc cref="ISerialService" />
public class SerialService : ISerialService
{
    private readonly ISerialRepository _serialRepository;
    private readonly ISerialValidator _serialValidator;

    /// <summary>
    /// Initializes a new instance of the <see cref="SerialService" /> class.
    /// </summary>
    /// <param name="serialRepository">Repository provides methods to retrieve/handle serial numbers.</param>
    /// <param name="serialValidator">Validator provides methods to validate serial numbers.</param>
    public SerialService(ISerialRepository serialRepository, ISerialValidator serialValidator)
    {
        _serialRepository = serialRepository;
        _serialValidator = serialValidator;
    }

    /// <inheritdoc />
    public async Task<SerialModel> GetByKeyIdAsync(int keyId) =>
        await _serialValidator.FindByKeyIdValidateAsync(keyId, _serialRepository.GetByKeyIdAsync);
}