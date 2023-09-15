using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using ProductionService.Client.Services.Barcodes;
using ProductionService.Shared.Dtos.Scanner;
using ScannerClient.WebApp.Core.Scanner.Models;
using ScannerClient.WebApp.Core.Scanner.ScannerSimulator;
using System.Text.Json;

namespace ScannerClient.WebApp.Core.Scanner.ScannerReader;

public class ScannerReader : IScannerReader
{
    private readonly IBarcodeApiService _barcodeApiService;
    private readonly BrowserService _browserService;
    private readonly ScannerSimulatorService _scannerSimulatorService;
    private readonly CustomTimer _timer;

    private const double intervalOfProcessingScannedSymbols = 15;
    private KeyPressInvokeHelper keyPressInvokeHelper;
    private ScannerSimulatorInvokeHelper scannerSimulatorInvokeHelper;
    private readonly List<ScannedSymbolData> _scannedSymbols = new();

    private Func<string> _getKeyboardBarcode;
    private Action _beforeBarcodeProcessed;
    private Func<BarcodeData, Task> _afterBarcodeProcessed;

    public ScannerReader(
        IBarcodeApiService barcodeApiService,
        BrowserService browserService,
        ScannerSimulatorService scannerSimulatorService,
        CustomTimer timer)
    {
        _barcodeApiService = barcodeApiService;
        _browserService = browserService;
        _scannerSimulatorService = scannerSimulatorService;
        _timer = timer;
    }

    /// <inheritdoc />
    public async Task StartListenScannerAsync(
        bool listenSimulator,
        Func<string> getKeyboardBarcode,
        Action beforeBarcodeProcessed,
        Func<BarcodeData, Task> afterBarcodeProcessed)
    {
        if (!IsInitialized())
        {
            _getKeyboardBarcode = getKeyboardBarcode;
            _beforeBarcodeProcessed = beforeBarcodeProcessed;
            _afterBarcodeProcessed = afterBarcodeProcessed;

            // Subscribe to KeyPress event.
            keyPressInvokeHelper = new KeyPressInvokeHelper(KeyPressed);
            await _browserService.SubscribeToKeyPress(nameof(ScannerReader), keyPressInvokeHelper);

            // Subscribe to Simulator event.
            if (listenSimulator)
            {
                await InitializeScannerSimulatorAsync();
            }

            // Start listening.
            StartListenTimer();
        }
    }

    /// <inheritdoc />
    public void StopListenScanner()
    {
        if (IsInitialized())
        {
            _browserService.UnsubscribeFromKeyPress(nameof(ScannerReader));
            DisposeScannerSimulator();
        }
    }

    /// <summary>
    /// Check either scanner reader initialized.
    /// </summary>
    /// <returns>Scanner reader initialized or not.</returns>
    private bool IsInitialized() => keyPressInvokeHelper != null;

    private void KeyPressed(string key) => _scannedSymbols.Add(new ScannedSymbolData(DateTime.Now, key));

    private void StartListenTimer()
    {
        _timer.ExecActionAfterSomeDelay(
            async () => await ProcessScannedSymbolsAsync(),
            intervalOfProcessingScannedSymbols, true);
    }

    private async Task ProcessScannedSymbolsAsync()
    {
        if (_scannedSymbols.Count <= 1)
            return;

        const int scanCharacterDelay = 20;
        ScannedSymbolData firstScannedSymbol = _scannedSymbols[0];
        ScannedSymbolData secondScannedSymbol = _scannedSymbols[1];
        ScannedSymbolData lastScannedSymbol = _scannedSymbols[^1];
        ScannedSymbolData beforeLastScannedSymbol = _scannedSymbols[^2];
        double diffMillisecondsAtBegin = Math.Abs((firstScannedSymbol.TimeStamp - secondScannedSymbol.TimeStamp).TotalMilliseconds);
        double diffMillisecondsAtEnd = Math.Abs((lastScannedSymbol.TimeStamp - beforeLastScannedSymbol.TimeStamp).TotalMilliseconds);

        // Case for user inputs. Check whether first element typed from keyboard. If yes -
        // remove it from scanned items array
        if (diffMillisecondsAtBegin > scanCharacterDelay)
        {
            if (!await HandleKeyboardEnterAsync(lastScannedSymbol))
                _scannedSymbols.Remove(firstScannedSymbol);
        }
        // Check whether received symbols are taken from scanner and scanning is finished
        else if (diffMillisecondsAtEnd < scanCharacterDelay && lastScannedSymbol.Key == "Enter")
        {
            // Remove Enter element(s)
            string scannedBarcode = string.Join("",
                _scannedSymbols.Where(symbol => symbol.Key != "Enter").Select(symbol => symbol.Key));
            if (!string.IsNullOrEmpty(scannedBarcode))
                await ProcessScannedBarcodeAsync(scannedBarcode, BarcodeInputDeviceType.Scanner);
        }
        // Check whether received symbols are taken from scanner and scanning is NOT finished
        else if (diffMillisecondsAtEnd < scanCharacterDelay && lastScannedSymbol.Key != "Enter")
        {
        }
        // Other cases - symbols are entered from keyboard
        else if (!await HandleKeyboardEnterAsync(lastScannedSymbol))
            _scannedSymbols.Clear();
    }

    private async Task<bool> HandleKeyboardEnterAsync(ScannedSymbolData lastScannedSymbol)
    {
        string keyboardBarcode = _getKeyboardBarcode?.Invoke();
        if (lastScannedSymbol.Key != "Enter" || string.IsNullOrEmpty(keyboardBarcode))
            return false;
        await ProcessScannedBarcodeAsync(keyboardBarcode, BarcodeInputDeviceType.Keyboard);
        return true;
    }

    private async Task ProcessScannedBarcodeAsync(string barcode, BarcodeInputDeviceType deviceType)
    {
        _beforeBarcodeProcessed?.Invoke();
        _scannedSymbols.Clear();
        BarcodeDto data = null;
        try
        {
            data = await _barcodeApiService.GetBarcodeDataAsync(barcode);
        }
        catch (AccessTokenNotAvailableException exception)
        {
            exception.Redirect();
        }
        BarcodeData barcodeData = JsonSerializer.Deserialize<BarcodeData>(JsonSerializer.Serialize(data));
        barcodeData.DeviceType = deviceType;
        await _afterBarcodeProcessed?.Invoke(barcodeData);
    }

    #region Scanner Simulator

    private async Task InitializeScannerSimulatorAsync()
    {
        scannerSimulatorInvokeHelper = new ScannerSimulatorInvokeHelper(ScanBarcodeBySimulatorAsync);
        await _scannerSimulatorService.InitScannerSimulator(scannerSimulatorInvokeHelper);
    }

    private void DisposeScannerSimulator()
    {
        if (scannerSimulatorInvokeHelper != null)
            _scannerSimulatorService.DisposeScannerSimulator();
    }

    private async Task ScanBarcodeBySimulatorAsync(string barcode) => await ProcessScannedBarcodeAsync(barcode, BarcodeInputDeviceType.Simulator);

    #endregion Scanner Simulator
}