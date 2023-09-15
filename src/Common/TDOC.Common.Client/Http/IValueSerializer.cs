using System.Text;

namespace TDOC.Common.Client.Http
{
    /// <summary>
    /// Defines methods and properties related to serializing data and how
    /// it is represented in http content.
    /// </summary>
    public interface IValueSerializer<in TValue>
    {
        /// <summary>
        /// The desired encoding for http content.
        /// </summary>
        /// <returns>The desired encoding.</returns>
        public Encoding Encoding { get; }

        /// <summary>
        /// The desired media type in http content, expressed as a string
        /// such as <c>application/json</c> and <c>text/xml</c>.
        /// </summary>
        public string MediaType { get; }

        /// <summary>
        /// Serializes the specified value into a string.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>The value serialized into a string.</returns>
        public string Serialize(TValue value);
    }
}