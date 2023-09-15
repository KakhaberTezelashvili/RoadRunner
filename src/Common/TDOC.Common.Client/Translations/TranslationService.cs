using System.Reflection;
using TDOC.Common.Data.Constants.Translations;
using TDOC.Common.Data.Enumerations.Messages;

namespace TDOC.Common.Client.Translations;

/// <inheritdoc cref="ITranslationService" />
public class TranslationService : ITranslationService
{
    /// <inheritdoc />
    public string PrepareErrorMessage(ValidationError error, MessageType messageType, IStringLocalizer errorMessagesLocalizer, 
        IStringLocalizer sharedLocalizer, IStringLocalizer enumsLocalizer, IStringLocalizer tablesLocalizer,
        IStringLocalizer exceptionalColumnsLocalizer)
    {
        if (error is DomainValidationError domainValidationError)
        {
            string errorMessageKey = $"{error.Code.GetType().Name}.{domainValidationError.Code}{messageType}";
            LocalizedString message = errorMessagesLocalizer[errorMessageKey];

            if (message == errorMessageKey)
                return null;

            if (domainValidationError.Details == null)
                return message;

            var parameters = new List<string>();
            IEnumerable<ValidationCodeDetails> details = domainValidationError.Details.Where(detail => detail.MessageType == messageType);

            const string tdocModelSuffix = "Model.";
            foreach (ValidationCodeDetails detail in details)
            {
                if (detail.Value is not Enum)
                {
                    if (detail.Value is string && detail.Value.ToString().Contains(tdocModelSuffix))
                    {
                        var columnDisplayName =
                            TranslateColumnDisplayName(detail.Value.ToString(), tablesLocalizer, exceptionalColumnsLocalizer);
                        parameters.Add(columnDisplayName);
                    }
                    else
                        parameters.Add(detail.Value.ToString());
                }
                else
                {
                    Type valueType = detail.Value.GetType();
                    MethodInfo getEnumValueNameMethod = typeof(TranslationHelper).GetMethod(nameof(TranslationHelper.GetEnumValueName));
                    MethodInfo genericGetEnumValueNameMethod = getEnumValueNameMethod.MakeGenericMethod(valueType);

                    if (genericGetEnumValueNameMethod.Invoke(null, new[] { detail.Value, sharedLocalizer, enumsLocalizer }) is string value)
                        parameters.Add(value);
                    else
                        parameters.Add(detail.Value.ToString());
                }
            }

            return string.Format(message, parameters.ToArray());
        }

        return messageType == MessageType.Title ? "Bad request!" : null;
    }

    // Example of "raw" display name to be translated:
    // "OrderTemplateModel.SuppRefSuppKeyId.SupplierModel.FacKeyId.FactoryModel.CreatedKeyId.UserModel.Initials".
    /// <inheritdoc />
    public void TranslateColumnsDisplayNames(IList<GridColumnConfiguration> columnConfigs,
        IStringLocalizer tablesLocalizer, IStringLocalizer exceptionalColumnsLocalizer)
    {
        foreach (GridColumnConfiguration columnConfig in columnConfigs)
        {
            // Process "calculated" columns.
            if (columnConfig.Calculated)
                columnConfig.DisplayName = exceptionalColumnsLocalizer[columnConfig.DisplayName];
            else
            {
                string[] displayNameParts = columnConfig.DisplayName.Split(".");
                if (displayNameParts.Length % 2 == 0)
                {
                    string translatedDisplayName = "";
                    for (var i = 0; i < displayNameParts.Length - 1; i += 2)
                    {
                        if (!string.IsNullOrEmpty(translatedDisplayName))
                            translatedDisplayName += ".";

                        string columnName = displayNameParts[i + 1];
                        string tableWithColumnName = $"{displayNameParts[i]}.{columnName}";

                        // Process "user-field" columns.
                        if (columnConfig.UserField && i == displayNameParts.Length - 2)
                        {
                            if (columnName.Contains(ExceptionalColumns.UserFieldColumnPrefix))
                                translatedDisplayName += columnName.Replace(ExceptionalColumns.UserFieldColumnPrefix, exceptionalColumnsLocalizer[ExceptionalColumns.UserFieldColumn]);
                            else
                                translatedDisplayName += columnName;
                        }
                        else
                            translatedDisplayName += TranslateColumnDisplayName(tableWithColumnName, tablesLocalizer, exceptionalColumnsLocalizer);
                    }

                    columnConfig.DisplayName = translatedDisplayName;
                }
            }
        }
    }

    /// <summary>
    /// Returns translated column name.
    /// </summary>
    /// <param name="tableWithColumnName">Column name including its table name as dot separated preffix (e.g. ItemModel.Item).</param>
    /// <param name="tablesLocalizer">Tables resources string localizer.</param>
    /// <param name="exceptionalColumnsLocalizer">Exceptional columns resources string localizer.</param>
    /// <returns></returns>
    private string TranslateColumnDisplayName(string tableWithColumnName, IStringLocalizer tablesLocalizer, IStringLocalizer exceptionalColumnsLocalizer)
    {
        string columnName = tableWithColumnName.Split(".")[1];
        // Process "special-field" columns: Created, Created by, Modified, Modified by.
        if (columnName == nameof(AGSModel.Created) || columnName == nameof(AGSModel.Modified) ||
            columnName == nameof(AGSModel.CreatedKeyId) || columnName == nameof(AGSModel.ModifiedKeyId))
            return exceptionalColumnsLocalizer[$"{ExceptionalColumns.SpecialColumnsPrefix}.{columnName}"];
        // Process "UnitModel.KeyId" column.
        else if (tableWithColumnName == $"{nameof(UnitModel)}.{nameof(UnitModel.KeyId)}")
            return tablesLocalizer[ExceptionalColumns.UnitKeyIdColumn];
        // Process "ProcessModel.KeyId" column.
        else if (tableWithColumnName == $"{nameof(ProcessModel)}.{nameof(ProcessModel.KeyId)}")
            return tablesLocalizer[ExceptionalColumns.ProcessKeyIdColumn];
        // Process "CompositeModel.KeyId" column.
        else if (tableWithColumnName == $"{nameof(CompositeModel)}.{nameof(CompositeModel.KeyId)}")
            return tablesLocalizer[ExceptionalColumns.CompositeKeyIdColumn];
        // Process all other "normal" columns.
        else
            return tablesLocalizer[tableWithColumnName];
    }
}