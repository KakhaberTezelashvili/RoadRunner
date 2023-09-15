using ProductionService.Shared.Dtos.Texts;

namespace ProductionService.Core.Services.Texts;

/// <inheritdoc cref="ITextService" />
public class TextService : ITextService
{
    private readonly ITextRepository _textRepository;
    private readonly ITextValidator _textValidator;

    /// <summary>
    /// Initilizes a new instance of the <see cref="TextService" /> class.
    /// </summary>
    /// <param name="textRepository">Repository provides methods to retrieve/handle error codes, control codes etc.</param>
    /// <param name="textValidator">Validator provides methods to validate error codes, control codes etc.</param>
    public TextService(ITextRepository textRepository, ITextValidator textValidator)
    {
        _textRepository = textRepository;
        _textValidator = textValidator;
    }

    /// <inheritdoc />
    public async Task<TextModel> GetErrorAsync(int errorNo) => await _textValidator.GetErrorValidateAsync(errorNo);

    /// <inheritdoc />
    public async Task<IList<ErrorCodeDetailsDto>> GetErrorCodesAsync() => await _textRepository.GetErrorCodesAsync();
}