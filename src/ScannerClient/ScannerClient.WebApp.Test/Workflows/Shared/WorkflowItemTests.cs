using AngleSharp.Dom;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Moq;
using ProductionService.Shared.Dtos.Positions;
using ScannerClient.WebApp.Core.Constants;
using ScannerClient.WebApp.Workflows.WorkflowList;
using TDOC.Resources.Locales.Enumerations;
using TDOC.Resources.Locales.Shared;
using Xunit;

namespace ScannerClient.WebApp.Test.Workflows.Shared
{
    public class WorkflowItemTests : TestContext
    {
        // Injected services.
        private readonly Mock<IStringLocalizer<TdEnumerationsResource>> _tdEnumsLocalizer;
        private readonly Mock<IStringLocalizer<TdSharedResource>> _tdSharedLocalizer;

        public WorkflowItemTests()
        {
            _tdEnumsLocalizer = new();
            _tdSharedLocalizer = new();
            Services.AddSingleton(_tdEnumsLocalizer.Object);
            Services.AddSingleton(_tdSharedLocalizer.Object);
        }

        [Fact]
        public void SetParameterWorkflow_RendersIconAndName()
        {
            // Arrange
            var positionLocationsDetails = new PositionLocationsDetailsDto()
            {
                LocationKeyId = 1,
                LocationName = "Return",
                Process = ProcessType.ReturnWF
            };
            string valueName = Enum.GetName(typeof(ProcessType), positionLocationsDetails.Process);
            string key = $"{typeof(ProcessType).Name}.{valueName}";
            _tdEnumsLocalizer.Setup(e => e[key]).Returns(new LocalizedString(key, positionLocationsDetails.LocationName));
            
            // Act
            // In bUnit "cut" this is short for "component under test".
            IRenderedComponent<WorkflowItem> cut = RenderComponent<WorkflowItem>(parameters => parameters
              .Add(p => p.Workflow, positionLocationsDetails)
            );
            IElement workflowIconElement = cut.Find($"#workflowIcon{positionLocationsDetails.LocationKeyId}");
            IElement workflowNameElement = cut.Find($"#workflowName{positionLocationsDetails.LocationKeyId}");

            // Assert
            workflowIconElement.MarkupMatches("<img id:ignore class:ignore src=\"_content/TDOC.Resources/img/units/unitReturn.svg\"/>");
            workflowNameElement.TextContent.MarkupMatches(positionLocationsDetails.LocationName);
        }

        [Fact]
        public void NavigateToWorkflow_RendersOk()
        {
            // Arrange
            var workflowDetails = new PositionLocationsDetailsDto()
            {
                LocationKeyId = 1,
                Process = ProcessType.ReturnWF
            };
            NavigationManager navMan = Services.GetRequiredService<NavigationManager>();

            // Act
            IRenderedComponent<WorkflowItem> cut = RenderComponent<WorkflowItem>(parameters => parameters
              .Add(p => p.Workflow, workflowDetails)
            );
            IElement workflowBlockElement = cut.Find($"#workflowBlock{workflowDetails.LocationKeyId}");
            workflowBlockElement.Click();

            // Assert
            Assert.Equal($"{navMan.BaseUri}{ScannerUrls.WorkflowUnitReturn}/{workflowDetails.LocationKeyId}", navMan.Uri);
        }
    }
}