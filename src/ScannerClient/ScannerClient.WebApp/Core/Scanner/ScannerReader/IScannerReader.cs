using ScannerClient.WebApp.Core.Scanner.Models;

namespace ScannerClient.WebApp.Core.Scanner.ScannerReader
{
    /// <summary>
    /// Scanner reader - listen for all barcodes that physical scanner scan.
    /// </summary>
    public interface IScannerReader
    {
        /// <summary>
        /// Start listen scanner.
        /// </summary>
        /// <param name="listenSimulator">Flag that means: should we listen simulator as well.</param>
        /// <param name="getKeyboardBarcode">Func to get manually entered barcode by keyboard.</param>
        /// <param name="beforeBarcodeProcessed">Action to be executed before barcode processed.</param>
        /// <param name="afterBarcodeProcessed">Action to be executed after barcode processed.</param>
        /// <returns>Task</returns>
        Task StartListenScannerAsync(
            bool listenSimulator,
            Func<string> getKeyboardBarcode,
            Action beforeBarcodeProcessed,
            Func<BarcodeData, Task> afterBarcodeProcessed);

        /// <summary>
        /// Stop listen scanner.
        /// </summary>
        void StopListenScanner();
    }
}
