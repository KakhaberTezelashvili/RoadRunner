using ProductionService.Core.Services.Lots;
using ProductionService.Shared.Dtos.Lots;
using Xunit;

namespace ProductionService.Core.Test.Services.Lots
{
    public class LotInformationHelperTests
    {
        #region TableName

        [Theory]
        [ClassData(typeof(LotInformationHelperTestsData))]
        public void TableName_ReturnsTableName(CustomLotInfoHelper lotInfoHelper, Dictionary<string, object> expectedData)
        {
            // Arrange
            object expected = expectedData[LotInformationHelperTestsData.TableName];

            // Act
            string actual = lotInfoHelper.TableName();

            // Assert
            Assert.Equal(expected, actual);
        }

        #endregion

        #region CreatedKeyIdField

        [Theory]
        [ClassData(typeof(LotInformationHelperTestsData))]
        public void CreatedKeyIdField_ReturnsCreatedKeyIdField(CustomLotInfoHelper lotInfoHelper, Dictionary<string, object> expectedData)
        {
            // Arrange
            object expected = expectedData[LotInformationHelperTestsData.CreatedKeyIdField];

            // Act
            string actual = lotInfoHelper.CreatedKeyIdField();

            // Assert
            Assert.Equal(expected, actual);
        }

        #endregion

        #region CreatedField

        [Theory]
        [ClassData(typeof(LotInformationHelperTestsData))]
        public void CreatedField_ReturnsCreatedField(CustomLotInfoHelper lotInfoHelper, Dictionary<string, object> expectedData)
        {
            // Arrange
            object expected = expectedData[LotInformationHelperTestsData.CreatedField];

            // Act
            string actual = lotInfoHelper.CreatedField();

            // Assert
            Assert.Equal(expected, actual);
        }

        #endregion

        #region LocationKeyIdField

        [Theory]
        [ClassData(typeof(LotInformationHelperTestsData))]
        public void LocationKeyIdField_ReturnsLocationKeyIdField(CustomLotInfoHelper lotInfoHelper, Dictionary<string, object> expectedData)
        {
            // Arrange
            object expected = expectedData[LotInformationHelperTestsData.LocationKeyIdField];

            // Act
            string actual = lotInfoHelper.LocationKeyIdField();

            // Assert
            Assert.Equal(expected, actual);
        }

        #endregion

        #region LinkToLotField

        [Theory]
        [ClassData(typeof(LotInformationHelperTestsData))]
        public void LinkToLotField_ReturnsLinkToLotField(CustomLotInfoHelper lotInfoHelper, Dictionary<string, object> expectedData)
        {
            // Arrange
            object expected = expectedData[LotInformationHelperTestsData.LinkToLotField];

            // Act
            string actual = lotInfoHelper.LinkToLotField();

            // Assert
            Assert.Equal(expected, actual);
        }

        #endregion

        #region LinkToTableField

        [Theory]
        [ClassData(typeof(LotInformationHelperTestsData))]
        public void LinkToTableField_ReturnsLinkToTableField(CustomLotInfoHelper lotInfoHelper, Dictionary<string, object> expectedData)
        {
            // Arrange
            object expected = expectedData[LotInformationHelperTestsData.LinkToTableField];

            // Act
            string actual = lotInfoHelper.LinkToTableField();

            // Assert
            Assert.Equal(expected, actual);
        }

        #endregion

        #region LinkToPositionField

        [Theory]
        [ClassData(typeof(LotInformationHelperTestsData))]
        public void LinkToPositionField_ReturnsLinkToPositionField(CustomLotInfoHelper lotInfoHelper, Dictionary<string, object> expectedData)
        {
            // Arrange
            object expected = expectedData[LotInformationHelperTestsData.LinkToPositionField];

            // Act
            string actual = lotInfoHelper.LinkToPositionField();

            // Assert
            Assert.Equal(expected, actual);
        }

        #endregion

        #region HasLinkToPositionField

        [Theory]
        [ClassData(typeof(LotInformationHelperTestsData))]
        public void HasLinkToPositionField_ReturnsHasLinkToPositionField(CustomLotInfoHelper lotInfoHelper, Dictionary<string, object> expectedData)
        {
            // Arrange
            object expected = expectedData[LotInformationHelperTestsData.HasLinkToPositionField];

            // Act
            bool actual = lotInfoHelper.HasLinkToPositionField();

            // Assert
            Assert.Equal(expected, actual);
        }

        #endregion

        #region BoundArticleField

        [Theory]
        [ClassData(typeof(LotInformationHelperTestsData))]
        public void BoundArticleField_ReturnsBoundArticleField(CustomLotInfoHelper lotInfoHelper, Dictionary<string, object> expectedData)
        {
            // Arrange
            object expected = expectedData[LotInformationHelperTestsData.BoundArticleField];

            // Act
            string actual = lotInfoHelper.BoundArticleField();

            // Assert
            Assert.Equal(expected, actual);
        }

        #endregion

        #region HasBoundArticleField

        [Theory]
        [ClassData(typeof(LotInformationHelperTestsData))]
        public void HasBoundArticleField_ReturnsHasBoundArticleField(CustomLotInfoHelper lotInfoHelper, Dictionary<string, object> expectedData)
        {
            // Arrange
            object expected = expectedData[LotInformationHelperTestsData.HasBoundArticleField];

            // Act
            bool actual = lotInfoHelper.HasBoundArticleField();

            // Assert
            Assert.Equal(expected, actual);
        }

        #endregion

        #region LinkToKeyField

        [Theory]
        [ClassData(typeof(LotInformationHelperTestsData))]
        public void LinkToKeyField_ReturnsLinkToKeyField(CustomLotInfoHelper lotInfoHelper, Dictionary<string, object> expectedData)
        {
            // Arrange
            object expected = expectedData[LotInformationHelperTestsData.LinkToKeyField];

            // Act
            string actual = lotInfoHelper.LinkToKeyField();

            // Assert
            Assert.Equal(expected, actual);
        }

        #endregion

        #region HasLinkToKeyField

        [Theory]
        [ClassData(typeof(LotInformationHelperTestsData))]
        public void HasLinkToKeyField_ReturnsHasLinkToKeyField(CustomLotInfoHelper lotInfoHelper, Dictionary<string, object> expectedData)
        {
            // Arrange
            object expected = expectedData[LotInformationHelperTestsData.HasLinkToKeyField];

            // Act
            bool actual = lotInfoHelper.HasLinkToKeyField();

            // Assert
            Assert.Equal(expected, actual);
        }

        #endregion

        #region OperationDataInfo

        [Theory]
        [ClassData(typeof(LotInformationHelperOperationDataInfoTestData))]
        public void OperationDataInfo_ReturnsOperationDataInfo(CustomLotInfoHelper lotInfoHelper, int OperationKeyId, string expected)
        {
            // Arrange

            // Act
            string actual = lotInfoHelper.OperationDataInfo(OperationKeyId);

            // Assert
            Assert.Equal(expected, actual);
        }

        #endregion

        #region OperationAssigned

        [Theory]
        [ClassData(typeof(LotInformationHelperOperationAssignedTestData))]
        public void OperationAssigned_ReturnsOperationAssigned(CustomLotInfoHelper lotInfoHelper, int keyId, bool expected)
        {
            // Arrange

            // Act
            Action actual = () => lotInfoHelper.OperationAssigned(keyId);

            // Assert
            Assert.Throws<NotImplementedException>(actual);
        }

        #endregion

        #region UpdateExcludedLotNumbers

        // TODO:

        #endregion

        #region AdjustLOTEntriesLinks

        [Theory]
        [ClassData(typeof(LotInformationHelperAdjustLOTEntriesLinksTestData))]
        public void AdjustLOTEntriesLinks_ReturnsAdjustLOTEntriesLinks(CustomLotInfoHelper lotInfoHelper, LotInformationDto lotInformation, int expected)
        {
            // Arrange

            // Act
            lotInfoHelper.AdjustLOTEntriesLinks(lotInformation);

            // Assert
            Assert.Equal(expected, lotInformation.LotEntries[0].LinkPosition);
        }

        #endregion

        #region LotExists

        [Theory]
        [ClassData(typeof(LotInformationHelperLotExistsTestData))]
        public void LotExists_ReturnsLotExists(CustomLotInfoHelper lotInfoHelper, LotInformationDto lotInformation, int keyId, bool expected)
        {
            // Arrange

            // Act
            bool actual = lotInfoHelper.LotExists(lotInformation, keyId);

            // Assert
            Assert.Equal(expected, actual);
        }

        #endregion
    }
}
