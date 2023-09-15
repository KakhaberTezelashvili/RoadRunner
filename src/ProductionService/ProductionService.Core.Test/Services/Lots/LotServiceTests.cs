using Moq;
using ProductionService.Core.Models.Lots;
using ProductionService.Core.Repositories.Interfaces;
using ProductionService.Core.Services.Lots;
using ProductionService.Shared.Dtos.Lots;
using Xunit;

namespace ProductionService.Core.Test.Services.Lots
{
    public class LotServiceTests
    {
        private const int _keyId = 1;
        private const int _secondaryId = 2;
        private const int _userKeyId = 1;
        private const int _locationKeyId = 1;
        private const int _unitKeyId = 1;

        private const TDOCTable _entity = TDOCTable.Unit;
        private readonly List<LotInformationEntryDto> _lotEntries;
        private readonly List<UnitLotInformationArgs> _lotInfoList;
        private readonly ILotValidator _lotValidator;

        // Service to test.
        private readonly LotService _lotService;

        // Injected services.
        private readonly Mock<ILotRepository> _lotRepository;

        public LotServiceTests()
        {
            _lotEntries = new List<LotInformationEntryDto>();
            _lotInfoList = new()
            {
                new UnitLotInformationArgs { KeyId = 1, Position = 1 }
            };

            _lotRepository = new Mock<ILotRepository>();
            _lotValidator = new LotValidator(_lotRepository.Object);
            _lotService = new LotService(_lotRepository.Object, _lotValidator);
        }

        #region GetLOTInformationAsync

        [Fact]
        public async Task GetLOTInformationAsync_ReturnsFailedValidateBeforeLOTInformation()
        {
            // Arrange
            var lotParams = new LotInformationParams();

            // Act
            Exception exception = await Record.ExceptionAsync(() => _lotService.GetLotInformationAsync(lotParams));

            // Assert
            Assert.NotNull(exception);
        }

        // TODO Add TDOCTable.Product, TDOCTable.Composite, TDOCTable.IndicatorType
        [Theory]
        [InlineData(TDOCTable.Unit, true, true)]
        [InlineData(TDOCTable.Unit, true, false)]
        [InlineData(TDOCTable.Unit, false, true)]
        [InlineData(TDOCTable.Unit, false, false)]
        public async Task GetLOTInformationAsync_ReturnsLOTInformation(TDOCTable entity, bool isCompItem, bool isOperationAssigned)
        {
            // Arrange
            var expectedHeaderInfo = new HeaderInfo
            {
                Info1 = _unitKeyId.ToString(),
                Info2 = "Info 2",
                Info3 = "Info 3",
                Info4 = "Info 4"
            };

            var lotParams = new LotInformationParams()
            {
                Entity = entity,
                KeyId = _keyId,
                SecondaryId = _secondaryId,
                ProcessType = ProcessType.Pack
            };

            var lotInformationEntries = new List<LotInformationEntryDto>()
            {
                new LotInformationEntryDto()
                {
                    KeyId = 1,
                    ItemKeyId = 1
                },
                new LotInformationEntryDto()
                {
                    KeyId = 2,
                    ItemKeyId = 2
                }
            };

            var linkedItems = new List<int>() { 1, 2 };
            var itemInfoList = new List<ItemLotInformationExtDto>()
            {
                new ItemLotInformationExtDto()
                {
                    KeyId = 1,
                    Composite = isCompItem
                },
                new ItemLotInformationExtDto()
                {
                    KeyId = 2,
                    Composite = isCompItem
                }
            };
            _lotRepository.Setup(r => r.GetLotInfoEntriesByUnitKeyIdAsync(lotParams.KeyId, true, true)).ReturnsAsync(
                await Task.FromResult(lotInformationEntries));
            _lotRepository.Setup(r => r.IsOperationAssignedAsync(lotParams.KeyId)).ReturnsAsync(
                await Task.FromResult(isOperationAssigned));
            _lotRepository.Setup(r => r.GetItemLotInfoListAsync(lotParams, linkedItems)).ReturnsAsync(
                await Task.FromResult(new Tuple<IList<ItemLotInformationExtDto>, bool>(itemInfoList, isCompItem)));
            _lotRepository.Setup(r => r.GetHeaderInfoByLotParamsAsync(lotParams, isCompItem)).ReturnsAsync(
                await Task.FromResult(expectedHeaderInfo));

            // Act
            LotInformationDto lotInformation = await _lotService.GetLotInformationAsync(lotParams);

            // Assert
            Assert.NotNull(lotInformation);
            Assert.Equal(isOperationAssigned, lotInformation.OperationAssigned);
            Assert.Equal(isCompItem, lotInformation.CompositeItem);
            Assert.Equal(expectedHeaderInfo.Info1, lotInformation.Info1);
            Assert.Equal(expectedHeaderInfo.Info2, lotInformation.Info2);
            Assert.Equal(expectedHeaderInfo.Info3, lotInformation.Info3);
            Assert.Equal(expectedHeaderInfo.Info4, lotInformation.Info4);
            Assert.Equal(itemInfoList.Count, lotInformation.LotEntries.Count);
            Assert.Equal(itemInfoList[0].KeyId, lotInformation.LotEntries[0].KeyId);
            Assert.Equal(itemInfoList.Count, lotInformation.Items.Count);
            Assert.Equal(itemInfoList[0].KeyId, lotInformation.Items[0].KeyId);
            _lotRepository.Verify(r => r.ItemSupportedLotNumbersAsync(It.IsAny<IList<int>>(), It.IsAny<IList<int>>(), It.IsAny<LotInformationDto>()), Times.Once);
        }

        #endregion GetLOTInformationAsync

        #region UpdateLOTInformationAsync

        [Fact]
        public async Task UpdateLOTInformationAsync_ReturnsFailedValidateBeforeUpdateLOTInformation()
        {
            // Arrange
            var lotInformation = new LotInformationDto();

            // Act
            Exception exception = await Record.ExceptionAsync(() => _lotService.UpdateLotInformationAsync(lotInformation));

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public async Task UpdateLOTInformationAsync_ReturnsUpdateLOTInformation()
        {
            // Arrange
            var lotInformation = new LotInformationDto()
            {
                KeyId = _keyId,
                UserKeyId = _userKeyId,
                LocationKeyId = _locationKeyId,
                Entity = _entity,
                LotEntries = _lotEntries
            };
            _lotRepository.Setup(r => r.GetMatchedLotIdsAsync(lotInformation)).ReturnsAsync(
                await Task.FromResult(new List<int>()));

            _lotRepository.Setup(r => r.GetUnitContentListAsync(lotInformation)).ReturnsAsync(
                await Task.FromResult(new List<int?>()));

            // Act
            Exception exception = await Record.ExceptionAsync(() => _lotService.UpdateLotInformationAsync(lotInformation));

            // Assert
            Assert.Null(exception);
        }

        #endregion UpdateLOTInformationAsync
    }
}