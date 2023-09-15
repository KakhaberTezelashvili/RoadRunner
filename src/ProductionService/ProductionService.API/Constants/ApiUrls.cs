namespace ProductionService.API.Constants;

public class ApiUrls
{
    public const string Production = "production/api/v{version:apiVersion}";
    public const string Units = $"{Production}/units";
}