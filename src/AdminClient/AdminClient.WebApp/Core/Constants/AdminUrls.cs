namespace AdminClient.WebApp.Core.Constants;

public class AdminUrls
{
    // General admin URls.
    public const string Default = "";
    public const string Login = $"{prefixAdmin}login";
    // Customer URLs.
    public const string CustomerList = $"{prefixCustomer}list";
    public const string CustomerDetails = $"{prefixCustomer}details";
    // Item URLs.
    public const string ItemDetails = $"{prefixItem}details";
    // Product URLs.
    public const string ProductDetails = $"{prefixProduct}details";

    private const string prefixAdmin = "admin-";
    private const string prefixCustomer = "customer-";
    private const string prefixItem = "item-";
    private const string prefixProduct = "product-";
}