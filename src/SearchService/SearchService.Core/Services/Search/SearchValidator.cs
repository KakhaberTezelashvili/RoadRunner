namespace SearchService.Core.Services.Search;

/// <inheritdoc cref="ISearchValidator" />
public class SearchValidator : ISearchValidator
{
    /// <inheritdoc />
    public void SelectArgsValidate(SelectArgs selectArgs)
    {
        if (selectArgs == null)
            throw new InputArgumentException(GenericErrorCodes.ArgumentsNull);

        if (selectArgs.PaginationArgs.PageRowCount <= 0 ||
            selectArgs.PaginationArgs.StartingRow < 0 ||
            string.IsNullOrEmpty(selectArgs.MainEntity))
            throw new InputArgumentException(GenericErrorCodes.ArgumentsNotValid);

        if (selectArgs.SelectedFields == null || !selectArgs.SelectedFields.Any())
            throw new InputArgumentException(InputArgumentSearchErrorCodes.NoSelectedField);
    }
}