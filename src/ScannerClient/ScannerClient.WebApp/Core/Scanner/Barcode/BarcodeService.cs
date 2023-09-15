using Newtonsoft.Json.Linq;
using ProductionService.Shared.Enumerations.Barcode;
using ScannerClient.WebApp.Core.Scanner.Models;

namespace ScannerClient.WebApp.Core.Scanner.Barcode
{
    /// <inheritdoc cref="IBarcodeService" />
    public class BarcodeService : IBarcodeService
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="BarcodeService" /> class.
        /// </summary>
        /// <param name="mediator"MediatR.></param>
        public BarcodeService(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <inheritdoc />
        public async Task ExecuteBarcodeActionAsync(JObject data, string mainEntity, string mainEntityKey, bool rowClicked)
        {
            if (data == null)
                return;

            BarcodeType mainEntityBarcodeType = GetBarcodeTypeByMainEntity(mainEntity, mainEntityKey);
            if (mainEntityBarcodeType == BarcodeType.Unknown)
                return;

            // Get keyId by MainEntityKey.
            int keyId = (int)data[mainEntityKey];
            // Publishing the Barcode notification.
            await _mediator.Publish(new BarcodeDataNotification(new(mainEntityBarcodeType, keyId.ToString())));
        }

        private BarcodeType GetBarcodeTypeByMainEntity(string mainEntity, string mainEntityKey)
        {
            switch (mainEntityKey)
            {
                case nameof(UnitModel.SeriKeyId):
                    return BarcodeType.SerialKey;
                default:
                    break;
            }

            return mainEntity.Split(".").Last() switch
            {
                nameof(ProcessModel) => BarcodeType.Batch,
                nameof(ProductModel) => BarcodeType.Product,
                nameof(UnitModel) => BarcodeType.Unit,
                _ => BarcodeType.Unknown,
            };
        }
    }
}
