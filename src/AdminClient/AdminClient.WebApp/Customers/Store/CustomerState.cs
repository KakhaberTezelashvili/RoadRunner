namespace AdminClient.WebApp.Customers.Store;

public class CustomerState
{
    public bool IsLoading { get; }
    public IList<CustomerModel> Customers { get; }

    public CustomerState(bool isLoading, IList<CustomerModel> customers)
    {
        IsLoading = isLoading;
        Customers = customers ?? new List<CustomerModel>();
    }
}