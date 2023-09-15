using FluentAssertions;
using Moq;
using ProductionService.Core.Models.Batches;
using ProductionService.Core.Repositories.Interfaces;
using ProductionService.Core.Services.Batches;
using ProductionService.Core.Services.Units;
using ProductionService.Core.Services.Units.Locations;
using ProductionService.Shared.Dtos.Processes;
using Xunit;

namespace ProductionService.Core.Test.Services.Batches
{
    public class BatchServiceTests
    {
        private const int _batchKeyId = 1;
        private const int _locationKeyId = 1;
        private const int _unitKeyId = 1;
        private const int _userKeyId = 1;
        private const BatchType _batchType = BatchType.ExtraWash;

        private readonly MachineType _machineType = MachineType.Washer;
        private readonly UnitStatus _unitStatus = UnitStatus.Init;

        // Service to test.
        private readonly BatchService _batchService;

        // Injected services.
        private readonly Mock<IBatchRepository> _batchRepository;
        private readonly Mock<IBatchValidator> _batchValidator;
        private readonly Mock<IUnitService> _unitService;
        private readonly Mock<IUnitLocationService> _unitLocationService;

        public BatchServiceTests()
        {
            _batchRepository = new();
            _batchValidator = new();
            _unitService = new();
            _unitLocationService = new();
            _batchService = new BatchService(_batchRepository.Object, _batchValidator.Object, _unitService.Object, _unitLocationService.Object);
        }

        #region LinkUnitsToBatchAsync

        [Fact]
        public async Task LinkUnitsToBatchAsync_ReturnsFailedValidateBeforeLinkUnitsToBatch()
        {
            // Arrange

            // Act
            Exception exception = await Record.ExceptionAsync(() => _batchService.LinkUnitsToBatchAsync(_batchKeyId, _machineType, _userKeyId, null));

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public async Task LinkUnitsToBatchAsync_ReturnsLinkUnitsToBatchSuccessfully()
        {
            // Arrange
            BatchCreateArgs args = new()
            {
                LocationKeyId = _locationKeyId,
                UnitKeyIds = new List<int> { 1, 2 }
            };
            int unitKeyIdsCount = args.UnitKeyIds.Count;

            // Act
            Exception exception = await Record.ExceptionAsync(async () => await _batchService.LinkUnitsToBatchAsync(_batchKeyId, _machineType, _userKeyId, args));

            // Assert
            Assert.Null(exception);
            _batchRepository.Verify(r => r.AddAsync(It.IsAny<BatchModel>()), Times.Exactly(unitKeyIdsCount));
            _unitLocationService.Verify(r => r.UpdateAsync(It.IsAny<int>(), It.IsAny<BatchType>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>()), Times.Exactly(unitKeyIdsCount));
            _unitLocationService.Verify(r => r.AddAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<WhatType>(), It.IsAny<int>()), Times.Exactly(unitKeyIdsCount));
        }

        #endregion LinkUnitsToBatchAsync

        #region UpdateBatchesStatusToOkAsync

        [Fact]
        public async Task UpdateBatchesStatusToOkAsync_UpdatesBatchesSuccessfully()
        {
            // Arrange
            int batchKeyId = 1;
            List<BatchModel> batches = new()
            {
                new()
                {
                    Batch = batchKeyId,
                    Unit = 1002,
                    Type = (int)BatchType.PrimarySteri,
                    ChPlKeyId = 1002,
                    Status = BatchUnitStatus.BatchFailed
                },
                new()
                {
                    Batch = batchKeyId,
                    Unit = 1003,
                    Type = (int)BatchType.PrimarySteri,
                    ChPlKeyId = 1002,
                    Status = BatchUnitStatus.BatchFailed
                }
            };
            _batchRepository.Setup(b => b.FindUnFailedBatchesByBatchKeyIdAsync(batchKeyId)).ReturnsAsync(batches);

            // Act
            await _batchService.UpdateBatchesStatusToOkAsync(batchKeyId);

            // Assert
            batches.Select(c => c.Status).Should().AllBeEquivalentTo(BatchUnitStatus.OK);
            _batchRepository.Verify(r => r.UpdateRangeAsync(It.IsAny<IEnumerable<BatchModel>>()), Times.Exactly(1));
        }

        #endregion UpdateBatchesStatusToOkAsync

        #region UpdateBatchesStatusToFailedAsync

        [Fact]
        public async Task UpdateBatchesStatusToFailedAsync_UpdatesBatchesSuccessfully()
        {
            // Arrange
            int batchKeyId = 1;
            List<BatchModel> batches = new()
            {
                new()
                {
                    Batch = batchKeyId,
                    Unit = 1002,
                    Type = (int)BatchType.PrimarySteri,
                    ChPlKeyId = 1002,
                    Status = BatchUnitStatus.OK
                },
                new()
                {
                    Batch = batchKeyId,
                    Unit = 1003,
                    Type = (int)BatchType.PrimarySteri,
                    ChPlKeyId = 1002,
                    Status = BatchUnitStatus.UnitFailed
                }
            };
            _batchRepository.Setup(b => b.FindByBatchKeyIdAsync(batchKeyId)).ReturnsAsync(batches);

            // Act
            await _batchService.UpdateBatchesStatusToFailedAsync(batchKeyId);

            // Assert
            batches.Select(c => c.Status).Should().AllBeEquivalentTo(BatchUnitStatus.BatchFailed);
            _batchRepository.Verify(r => r.UpdateRangeAsync(It.IsAny<IEnumerable<BatchModel>>()), Times.Exactly(1));
        }

        #endregion UpdateBatchesStatusToFailedAsync

        #region RemoveBatchesByUnitKeyIdAndUnitStatusAsync

        [Fact]
        public async Task RemoveBatchesByUnitKeyIdAndUnitStatusAsync_ReturnsFailedValidateBeforeRemoveBatches()
        {
            // Arrange
            int unitKeyId = 0;
            _batchValidator.Setup(b => b.RemoveBatchesValidate(unitKeyId)).Throws(new Exception());

            // Act
            Exception exception = await Record.ExceptionAsync(() => _batchService.RemoveBatchesByUnitKeyIdAndUnitStatusAsync(unitKeyId, _unitStatus));

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public async Task RemoveBatchesByUnitKeyIdAndUnitStatusAsync_RemovesBatchesSuccessfully()
        {
            // Arrange

            // Act
            Exception exception = await Record.ExceptionAsync(() => _batchService.RemoveBatchesByUnitKeyIdAndUnitStatusAsync(_unitKeyId, _unitStatus));

            // Assert
            Assert.Null(exception);
        }

        #endregion RemoveBatchesByUnitKeyIdAndUnitStatusAsync

        #region GetLastBatchProcessesByUnitKeyIdsAndBatchTypeAsync

        [Fact]
        public async Task GetLastBatchProcessesByUnitKeyIdsAndBatchTypeAsync_ReturnsNothing()
        {
            // Arrange
            var unitKeyIds = new List<int>(_unitKeyId);

            _batchRepository.Setup(r => r.GetLastBatchProcessesByUnitKeyIdsAndBatchTypeAsync(unitKeyIds, _batchType))
                .ReturnsAsync(await Task.FromResult<List<BatchProcessData>>(null));

            // Act
            IList<BatchProcessData> batchProcessDataList = await _batchService.GetLastBatchProcessesByUnitKeyIdsAndBatchTypeAsync(unitKeyIds, _batchType);

            // Assert
            Assert.Null(batchProcessDataList);
        }

        #endregion
    }
}