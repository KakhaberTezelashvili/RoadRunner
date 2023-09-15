namespace AdminClient.WebApp.Customers.Store;

public static class CustomerReducers
{
    [ReducerMethod]
    public static CustomerState ReduceGetAllCustomersAction(CustomerState state, GetAllCustomersAction action) => 
        new(true, null);

    [ReducerMethod]
    public static CustomerState ReduceGetCustomersByUserKeyIdAction(CustomerState state, GetCustomersByUserKeyIdAction action) => 
        new(true, null);

    [ReducerMethod]
    public static CustomerState ReduceSetCustomersAction(CustomerState state, SetCustomersAction action) => 
        new(false, action.Customers);
}