namespace AdminClient.WebApp.Customers.Store;

public class GetAllCustomersAction
{
    public GetAllCustomersAction()
    {
    }
}

public class GetCustomersByUserKeyIdAction
{
    public int UserKeyId { get; set; }

    public GetCustomersByUserKeyIdAction(int userKeyId = 0)
    {
        UserKeyId = userKeyId;
    }
}

public class SetCustomersAction
{
    public IList<CustomerModel> Customers { get; }

    public SetCustomersAction(IList<CustomerModel> customers)
    {
        Customers = customers;
    }
}