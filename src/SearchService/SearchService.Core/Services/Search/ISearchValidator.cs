namespace SearchService.Core.Services.Search;

/// <summary>
/// Validator provides methods to validate search data.
/// </summary>
public interface ISearchValidator
{
    /// <summary>
    /// Validates select arguments.
    /// </summary>
    /// <param name="selectArgs">The select arguments.</param>
    void SelectArgsValidate(SelectArgs selectArgs);
}