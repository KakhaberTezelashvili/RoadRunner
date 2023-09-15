namespace TDOC.Common.Server.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class NoTransactionAttribute : Attribute
    {
    }
}