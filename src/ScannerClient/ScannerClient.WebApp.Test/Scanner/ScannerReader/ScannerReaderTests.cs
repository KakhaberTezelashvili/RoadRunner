using Microsoft.JSInterop;
using Moq;
using ProductionService.Client.Services.Barcodes;
using ScannerClient.WebApp.Core.Scanner.Models;
using ScannerClient.WebApp.Core.Scanner.ScannerSimulator;
using Xunit;

namespace ScannerClient.WebApp.Test.Scanner.ScannerReader
{
    public class ScannerReaderTests
    {
        private readonly Mock<BrowserService> _browserServiceMock;
        private readonly Mock<ScannerSimulatorService> _scannerSimulatorServiceMock;

        // Service to test.
        private readonly Core.Scanner.ScannerReader.ScannerReader _scannerReader;

        public ScannerReaderTests()
        {
            Mock<IJSInProcessRuntime> jsInProcessRuntimeMock = new();
            Mock<IBarcodeApiService> scannerApiServiceMock = new();
            _browserServiceMock = new Mock<BrowserService>(jsInProcessRuntimeMock.Object);
            _scannerSimulatorServiceMock = new Mock<ScannerSimulatorService>(jsInProcessRuntimeMock.Object);
            CustomTimer customTimer = new();

            _scannerReader = new Core.Scanner.ScannerReader.ScannerReader(scannerApiServiceMock.Object, _browserServiceMock.Object,
                _scannerSimulatorServiceMock.Object, customTimer);
        }

        private Task AfterBarcodeProcessed(BarcodeData data) => Task.CompletedTask;

        #region StartListenScannerAsync

        [Fact]
        [Trait("Category", "ScannerReader.StartListenScannerAsync")]
        public async Task StartListenScannerAsync_ScannerIsNotInitialized_ExecutedSubscribeToKeyPress()
        {
            // Arrange

            // Act
            await _scannerReader.StartListenScannerAsync(true, null, null, AfterBarcodeProcessed);

            // Assert
            _browserServiceMock.Verify(m =>
                m.SubscribeToKeyPress(nameof(ScannerReader), It.IsAny<KeyPressInvokeHelper>()), Times.Once);
        }

        [Fact]
        [Trait("Category", "ScannerReader.StartListenScannerAsync")]
        public async Task StartListenScannerAsync_ScannerIsNotInitialized_NotExecutedInitScannerSimulator()
        {
            // Arrange

            // Act
            await _scannerReader.StartListenScannerAsync(true, null, null, AfterBarcodeProcessed);

            // Assert
            _scannerSimulatorServiceMock.Verify(m =>
                m.InitScannerSimulator(It.IsAny<ScannerSimulatorInvokeHelper>()), Times.Once);
        }

        [Fact]
        [Trait("Category", "ScannerReader.StartListenScannerAsync")]
        public async Task StartListenScannerAsync_ScannerIsInitialized_ExecutedOnceSubscribeToKeyPress()
        {
            // Arrange

            // Act
            await _scannerReader.StartListenScannerAsync(true, null, null, AfterBarcodeProcessed);
            await _scannerReader.StartListenScannerAsync(true, null, null, AfterBarcodeProcessed);

            // Assert
            _scannerSimulatorServiceMock.Verify(m =>
                m.InitScannerSimulator(It.IsAny<ScannerSimulatorInvokeHelper>()), Times.Once);
        }

        // Todo: ProcessScannedSymbolsAsync method functionality could not be tested at the moment because it process the private _scannedSymbols list
        // and also execution is triggered by a timer
        //[Fact]
        //[Trait("Category", "ScannerReader.StartListenScannerAsync")]
        //public async Task StartListenScannerAsync_X()
        //{
        //    // Arrange
        //    using var ctx = new TestContext();
        //    ctx.Services.AddHttpClient(TypedHttpClientNames.ProductionClient.ToString());
        //    ctx.Services.AddScoped<ITypedHttpClientFactory, TypedHttpClientFactory>();
        //    ctx.Services.AddTransient<IScannerApiService, ScannerApiService>();
        //    ctx.Services.AddTransient<IScannerReader, ScannerReader>();
        //    ctx.Services.AddTransient<ScannerSimulatorService>();
        //    ctx.Services.AddTransient<IWorkflowHandler, WorkflowHandler>();
        //    ctx.Services.AddTransient<IUnitApiService, UnitApiService>();
        //    ctx.Services.AddTransient<CustomTimer>();
        //    ctx.Services.AddTransient<BrowserService>();
        //    var cut = ctx.RenderComponent<MainLayout>();

        // var buttonElement = cut.Find("div");

        //    // Act
        //    buttonElement.KeyPress(Key.Enter);
        //}

        #endregion StartListenScannerAsync

        #region StopListenScanner

        [Fact]
        [Trait("Category", "ScannerReader.StopListenScanner")]
        public void StopListenScanner_ScannerIsNotInitialized_NotExecutedUnsubscribeFromKeyPress()
        {
            // Arrange

            // Act
            _scannerReader.StopListenScanner();

            // Assert
            _browserServiceMock.Verify(m => m.UnsubscribeFromKeyPress(nameof(ScannerReader)), Times.Never());
        }

        [Fact]
        [Trait("Category", "ScannerReader.StopListenScanner")]
        public async Task StopListen_ScannerIsInitialized_ExecutedUnsubscribeFromKeyPress()
        {
            // Arrange

            // Act
            await _scannerReader.StartListenScannerAsync(true, null, null, AfterBarcodeProcessed);
            _scannerReader.StopListenScanner();

            // Assert
            _browserServiceMock.Verify(m => m.UnsubscribeFromKeyPress(nameof(ScannerReader)), Times.Once);
        }

        [Fact]
        [Trait("Category", "ScannerReader.StopListenScanner")]
        public async Task StopListen_ScannerAndSimulatorInvokeHelperInitialized_ExecutedDisposeScannerSimulator()
        {
            // Arrange

            // Act
            await _scannerReader.StartListenScannerAsync(true, null, null, AfterBarcodeProcessed);
            _scannerReader.StopListenScanner();

            // Assert
            _scannerSimulatorServiceMock.Verify(m => m.DisposeScannerSimulator(), Times.Once);
        }

        #endregion StopListenScanner
    }
}