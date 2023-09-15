namespace ProductionService.Core.Services.Serials;

/// <inheritdoc cref="ISerialValidator" />
public class SerialValidator : ValidatorBase<SerialModel>, ISerialValidator
{
    private readonly ISerialRepository _serialRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="SerialValidator" /> class.
    /// </summary>
    /// <param name="programRepository">Repository provides methods to retrieve/handle serial numbers.</param>
    public SerialValidator(ISerialRepository serialRepository) : base(serialRepository)
    {
        _serialRepository = serialRepository;
    }
}