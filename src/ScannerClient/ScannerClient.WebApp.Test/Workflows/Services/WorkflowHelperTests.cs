using ScannerClient.WebApp.Core.Constants;
using ScannerClient.WebApp.Workflows.Services;
using TDOC.Common.Data.Constants;
using Xunit;

namespace ScannerClient.WebApp.Test.Workflows.Services;

public class WorkflowHelperTests
{
    private const int _locationKeyId = 1;
    private const int _unitKeyId = 2;
    private const int _noKeyId = 0;
    private const string _locationSubpath = "/1";
    private const string _locationAndUnitSubpath = _locationSubpath + "/2";
    private const string _queryParams = "?id=1001";
    private const string _baseNavigationUri = "https://localhost:5001/";

    #region GetWorkflowBoardIcon

    [Theory]
    [InlineData(ProcessType.PackWF, $"{ContentUrls.ResourceImg}units/unitPack.svg")]
    [InlineData(ProcessType.ReturnWF, $"{ContentUrls.ResourceImg}units/unitReturn.svg")]
    [InlineData(ProcessType.SteriPreBatchWF, $"{ContentUrls.ResourceImg}processes/sterilizerPreBatch.svg")]
    [InlineData(ProcessType.SteriPostBatchWF, $"{ContentUrls.ResourceImg}processes/sterilizerPostBatch.svg")]
    [InlineData(ProcessType.WashPreBatchWF, $"{ContentUrls.ResourceImg}processes/washerPreBatch.svg")]
    [InlineData(ProcessType.WashPostBatchWF, $"{ContentUrls.ResourceImg}processes/washerPostBatch.svg")]
    [InlineData(ProcessType.OutWF, $"{ContentUrls.ResourceImg}units/unitDispatch.svg")]
    [InlineData(ProcessType.Pack, $"{ContentUrls.ResourceImg}others/dummy.svg")]
    public void GetWorkflowBoardIcon_ReturnsPathToWorkflowIcon(ProcessType processType, string expectedPath)
    {
        // Arrange -
        // Act
        string actualPath = WorkflowHelper.GetWorkflowBoardIcon(processType);
        // Assert
        Assert.Equal(expectedPath, actualPath);
    }

    #endregion GetWorkflowBoardIcon

    #region GetWorkflowStartPanelIcon

    [Theory]
    [InlineData(ProcessType.PackWF, $"{ContentUrls.ResourceImg}units/unitPack.svg")]
    [InlineData(ProcessType.ReturnWF, $"{ContentUrls.ResourceImg}units/unitReturn.svg")]
    [InlineData(ProcessType.SteriPreBatchWF, $"{ContentUrls.ResourceImg}units/unit.svg")]
    [InlineData(ProcessType.SteriPostBatchWF, $"{ContentUrls.ResourceImg}units/unit.svg")]
    [InlineData(ProcessType.WashPreBatchWF, $"{ContentUrls.ResourceImg}units/unit.svg")]
    [InlineData(ProcessType.WashPostBatchWF, $"{ContentUrls.ResourceImg}units/unit.svg")]
    [InlineData(ProcessType.OutWF, $"{ContentUrls.ResourceImg}units/unitDispatchStart.svg")]
    [InlineData(ProcessType.Pack, $"{ContentUrls.ResourceImg}others/dummy.svg")]
    public void GetWorkflowStartPanelIcon_ReturnsPathToWorkflowIcon(ProcessType processType, string expectedPath)
    {
        // Arrange -

        // Act
        string actualPath = WorkflowHelper.GetWorkflowStartPanelIcon(processType);

        // Assert
        Assert.Equal(expectedPath, actualPath);
    }

    #endregion GetWorkflowStartPanelIcon

    #region GetWorkflowNavLink

    [Theory]
    [InlineData(ProcessType.PackWF, ScannerUrls.WorkflowUnitPack + _locationSubpath)]
    [InlineData(ProcessType.ReturnWF, ScannerUrls.WorkflowUnitReturn + _locationSubpath)]
    [InlineData(ProcessType.SteriPreBatchWF, ScannerUrls.WorkflowBatchCreate + _locationSubpath)]
    [InlineData(ProcessType.SteriPostBatchWF, ScannerUrls.WorkflowBatchHandleList + _locationSubpath)]
    [InlineData(ProcessType.WashPreBatchWF, ScannerUrls.WorkflowBatchCreate + _locationSubpath)]
    [InlineData(ProcessType.WashPostBatchWF, ScannerUrls.WorkflowBatchHandleList + _locationSubpath)]
    [InlineData(ProcessType.OutWF, ScannerUrls.WorkflowUnitDispatch + _locationSubpath)]
    [InlineData(ProcessType.Pack, ScannerUrls.WorkflowList + _locationSubpath)]
    public void GetWorkflowNavLink_DefinedLocationKeyId_ReturnsWorkflowNavLink(ProcessType processType, string expectedNavLink)
    {
        // Arrange -

        // Act
        string actualNavLink = WorkflowHelper.GetWorkflowNavLink(processType, _locationKeyId, _noKeyId, "");

        // Assert
        Assert.Equal(expectedNavLink, actualNavLink);
    }

    [Theory]
    [InlineData(ProcessType.PackWF, ScannerUrls.WorkflowUnitPack + _locationAndUnitSubpath)]
    [InlineData(ProcessType.ReturnWF, ScannerUrls.WorkflowUnitReturn + _locationAndUnitSubpath)]
    [InlineData(ProcessType.SteriPreBatchWF, ScannerUrls.WorkflowBatchCreate + _locationAndUnitSubpath)]
    [InlineData(ProcessType.SteriPostBatchWF, ScannerUrls.WorkflowBatchHandleList + _locationAndUnitSubpath)]
    [InlineData(ProcessType.WashPreBatchWF, ScannerUrls.WorkflowBatchCreate + _locationAndUnitSubpath)]
    [InlineData(ProcessType.WashPostBatchWF, ScannerUrls.WorkflowBatchHandleList + _locationAndUnitSubpath)]
    [InlineData(ProcessType.OutWF, ScannerUrls.WorkflowUnitDispatch + _locationAndUnitSubpath)]
    [InlineData(ProcessType.Pack, ScannerUrls.WorkflowList + _locationAndUnitSubpath)]
    public void GetWorkflowNavLink_DefinedLocationAndUnitKeyIds_ReturnsWorkflowNavLink(ProcessType processType, string expectedNavLink)
    {
        // Arrange -

        // Act
        string actualNavLink = WorkflowHelper.GetWorkflowNavLink(processType, _locationKeyId, _unitKeyId, "");

        // Assert
        Assert.Equal(expectedNavLink, actualNavLink);
    }

    [Theory]
    [InlineData(ProcessType.PackWF, ScannerUrls.WorkflowUnitPack + _locationSubpath + _queryParams)]
    [InlineData(ProcessType.ReturnWF, ScannerUrls.WorkflowUnitReturn + _locationSubpath + _queryParams)]
    [InlineData(ProcessType.SteriPreBatchWF, ScannerUrls.WorkflowBatchCreate + _locationSubpath + _queryParams)]
    [InlineData(ProcessType.SteriPostBatchWF, ScannerUrls.WorkflowBatchHandleList + _locationSubpath + _queryParams)]
    [InlineData(ProcessType.WashPreBatchWF, ScannerUrls.WorkflowBatchCreate + _locationSubpath + _queryParams)]
    [InlineData(ProcessType.WashPostBatchWF, ScannerUrls.WorkflowBatchHandleList + _locationSubpath + _queryParams)]
    [InlineData(ProcessType.OutWF, ScannerUrls.WorkflowUnitDispatch + _locationSubpath + _queryParams)]
    [InlineData(ProcessType.Pack, ScannerUrls.WorkflowList + _locationSubpath + _queryParams)]
    public void GetWorkflowNavLink_DefinedLocationKeyIdAndNavigationUri_ReturnsWorkflowNavLink(ProcessType processType, string expectedNavLink)
    {
        // Arrange -

        // Act
        string actualNavLink = WorkflowHelper.GetWorkflowNavLink(processType, _locationKeyId, _noKeyId, _baseNavigationUri + ScannerUrls.WorkflowList + _queryParams);

        // Assert
        Assert.Equal(expectedNavLink, actualNavLink);
    }

    #endregion GetWorkflowNavLink

    #region GetWorkflowMainBlockHeight

    // TODO:

    #endregion GetWorkflowMainBlockHeight

    #region GetWorkflowMediaBlock

    // TODO:

    #endregion GetWorkflowMediaBlock
}