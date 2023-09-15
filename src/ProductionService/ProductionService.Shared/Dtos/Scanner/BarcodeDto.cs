using ProductionService.Shared.Enumerations.Barcode;

namespace ProductionService.Shared.Dtos.Scanner
{
    /// <summary>
    /// Barcode details: type and value.
    /// </summary>
    public record BarcodeDto
    {
        /// <summary>
        /// Barcode type.
        /// </summary>
        public BarcodeType CodeType { get; set; }
        /// <summary>
        /// Barcode value.
        /// </summary>
        public string CodeValue { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BarcodeDto" /> class.
        /// </summary>
        /// <param name="codeType">Barcode type.</param>
        /// <param name="codeValue">Barcode value.</param>
        public BarcodeDto(BarcodeType codeType, string codeValue)
        {
            CodeType = codeType;
            CodeValue = codeValue;
        }
    }
}
