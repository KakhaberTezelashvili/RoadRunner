using ProductionService.Shared.Constants.Barcode;
using ProductionService.Shared.Dtos.Scanner;
using ProductionService.Shared.Enumerations.Barcode;

namespace ProductionService.Core.Services.Scanner;

/// <inheritdoc cref="IScannerService" />
public class ScannerService : IScannerService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ScannerService" /> class.
    /// </summary>
    public ScannerService()
    {
    }

    /// <inheritdoc />
    public BarcodeDto GetBarcodeData(string barcode) => SplitNativeBarcode(StripPadZero(barcode));

    private string StripPadZero(string barcode)
    {
        if (string.IsNullOrEmpty(barcode))
        {
            return barcode;
        }
        else
        {
            return barcode.TrimStart('0');
        }
    }

    private BarcodeDto SplitNativeBarcode(string barcode)
    {
        int codeTypeLength = 0;
        BarcodeType codeType = BarcodeType.Unknown;
        string codeValue = "";
        if (!string.IsNullOrEmpty(barcode))
        {
            if ((barcode[0] == '#') || (barcode.IndexOf(SpecialBarcodes.PREFFIX) == 0))
            {
                if (barcode[0] == '#')
                {
                    codeTypeLength = 1;
                    codeType = BarcodeType.Keyboard;
                }
                else
                {
                    codeTypeLength = 2;
                    codeType = BarcodeType.Special;
                }
            }
            else
            {
                if (int.TryParse(barcode[0].ToString(), out int firstSymbol))
                {
                    if (firstSymbol <= 7)
                    {
                        codeTypeLength = 1;
                    }
                    else if (firstSymbol == 8)
                    {
                        codeTypeLength = 2;
                    }
                    else // == 9
                    {
                        codeTypeLength = 3;
                    }
                    if (int.TryParse(barcode.Substring(0, codeTypeLength), out int codeTypeNumber))
                    {
                        codeType = (BarcodeType)codeTypeNumber;
                    }
                }
            }
            codeValue = barcode.Substring(codeTypeLength);
        }
        return new BarcodeDto(codeType, codeValue);
    }
}