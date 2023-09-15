using ProductionService.Shared.Dtos.Texts;

namespace ProductionService.Infrastructure.Repositories;

/// <inheritdoc cref="ITextRepository" />
public class TextRepository : RepositoryBase<TextModel>, ITextRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TextRepository" /> class.
    /// </summary>
    /// <param name="context">EF database context.</param>
    public TextRepository(TDocEFDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<TextModel> GetTextAsync(TextType textType, int textNumber)
    {
        return await _context.Text.AsNoTracking()
            .Where(t => t.Type == (int)textType && t.Number == textNumber)
            .FirstOrDefaultAsync();
    }

    /// <inheritdoc />
    public async Task<IList<ErrorCodeDetailsDto>> GetErrorCodesAsync()
    {
        return await _context.Text.AsNoTracking()
            .Where(t => t.Type == (int)TextType.Error)
            .Select(t => new ErrorCodeDetailsDto()
            {
                ErrorNumber = t.Number,
                ErrorText = t.Text
            })
            .OrderBy(e => e.ErrorNumber)
            .ToListAsync();
    }
}