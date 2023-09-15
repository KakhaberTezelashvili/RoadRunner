namespace ProductionService.Core.Services.Texts;

/// <inheritdoc cref="ITextValidator" />
public class TextValidator : ValidatorBase<TextModel>, ITextValidator
{
    private readonly ITextRepository _textRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextValidator" /> class.
    /// </summary>
    /// <param name="textRepository">Repository provides methods to retrieve/handle error codes, control codes etc.</param>
    public TextValidator(ITextRepository textRepository) : base(textRepository)
    {
        _textRepository = textRepository;
    }

    /// <inheritdoc />
    public async Task<TextModel> GetErrorValidateAsync(int errorNo, int minErrorNo = 0)
    {
        if (errorNo < minErrorNo)
            throw ArgumentNotValidException();

        var entity = await _textRepository.GetTextAsync(TextType.Error, errorNo);
        if (entity == null)
            throw ArgumentNotFoundException();

        return entity;
    }
}