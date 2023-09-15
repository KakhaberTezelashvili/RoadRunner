using ProductionService.Core.Services.Scanner;
using ProductionService.Shared.Dtos.Scanner;
using ProductionService.Shared.Enumerations.Barcode;
using Xunit;

namespace ProductionService.Core.Test.Services.Scanner
{
    public class ScannerServiceTests
    {
        private const string _keyId = "1001";

        #region GetBarcodeData

        [Theory]
        [InlineData("", BarcodeType.Unknown, "")]
        [InlineData(null, BarcodeType.Unknown, "")]
        [InlineData("#" + _keyId, BarcodeType.Keyboard)]
        [InlineData("@@" + _keyId, BarcodeType.Special)]
        [InlineData("6" + _keyId, BarcodeType.Unit)]
        [InlineData("901" + _keyId, BarcodeType.Item)]
        [InlineData("902" + _keyId, BarcodeType.Product)]
        [InlineData("83" + _keyId, BarcodeType.SerialKey)]
        public void GetBarcodeData_ReturnsBarcodeTypeAndValue(string barcode, BarcodeType expectedType, string expectedValue = _keyId)
        {
            // Arrange
            var scannerService = new ScannerService();
            // Act
            BarcodeDto barcodeData = scannerService.GetBarcodeData(barcode);
            // Assert
            Assert.Equal(expectedType, barcodeData.CodeType);
            Assert.Equal(expectedValue, barcodeData.CodeValue);
        }

        #endregion
    }
}
