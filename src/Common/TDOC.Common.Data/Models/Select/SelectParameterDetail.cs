using Newtonsoft.Json;

namespace TDOC.Common.Data.Models.Select
{
    /// <summary>
    /// Select parameter detail.
    /// </summary>
    public class SelectParameterDetail
    {
        /// <summary>
        /// Select parameter name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Select parameter value.
        /// </summary>
        [JsonConverter(typeof(JsonObjectAndEnumConverter))]
        public object Value { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectParameterDetail" /> class.
        /// </summary>
        /// <param name="name">Select parameter name.</param>
        /// <param name="value">Select parameter value.</param>
        public SelectParameterDetail(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }
}