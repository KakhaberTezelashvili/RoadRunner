namespace TDOC.Common.Utilities;

/// <summary>
/// Provides utilities for attributes.
/// </summary>
public static class AttributeUtilities
{
    /// <summary>
    /// Gets attribute of specified type.
    /// </summary>
    /// <param name="instance">Object.</param>
    /// <param name="propertyName">Property name.</param>
    /// <returns>Returns the first available attribute of a given type based on object and property.</returns>
    public static T GetAttributeFrom<T>(object instance, string propertyName) where T : Attribute
    {
        var attributeType = typeof(T);
        var property = instance.GetType().GetProperty(propertyName);
        return (T)property.GetCustomAttributes(attributeType, false).FirstOrDefault();
    }
}