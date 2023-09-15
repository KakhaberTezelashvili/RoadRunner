using ProductionService.Shared.Dtos.Scanner;
using ProductionService.Shared.Enumerations.Barcode;

namespace ScannerClient.WebApp.Core.Scanner.Models
{
    public record BarcodeData : BarcodeDto
    {
        public BarcodeInputDeviceType DeviceType { get; set; }

        public BarcodeData(BarcodeInputDeviceType deviceType, BarcodeType codeType, string codeValue) : base(codeType, codeValue)
        {
            DeviceType = deviceType;
        }
    }
}
