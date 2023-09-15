using ProductionService.Shared.Dtos.Scanner;

namespace ScannerClient.WebApp.Core.Scanner.Models
{
    /// <summary>
    /// General <see cref="MediatR"/> data notification containing the Barcode.
    /// </summary>
    public class BarcodeDataNotification : INotification
    {
        /// <summary>
        /// Data of the barcode.
        /// </summary>
        public BarcodeDto Data { get; }

        /// <summary>
        /// The constructor for the general <see cref="MediatR"/> data notification
        /// </summary>
        /// <param name="data">Data of the barcode</param>
        public BarcodeDataNotification(BarcodeDto data)
        {
            Data = data;
        }
    }
}