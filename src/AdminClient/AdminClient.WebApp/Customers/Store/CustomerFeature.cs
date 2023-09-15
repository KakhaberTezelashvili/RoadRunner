namespace AdminClient.WebApp.Customers.Store;

public class CustomerFeature : Feature<CustomerState>
{
    public override string GetName() => nameof(CustomerState);
    protected override CustomerState GetInitialState() => new(false, null);
}