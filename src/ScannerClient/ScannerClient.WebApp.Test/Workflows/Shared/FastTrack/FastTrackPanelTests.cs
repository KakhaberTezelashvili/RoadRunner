using AngleSharp.Dom;
using Bunit;
using ScannerClient.WebApp.Workflows.Shared.FastTrack;
using Xunit;

namespace ScannerClient.WebApp.Test.Workflows.Shared.FastTrack
{
    public class FastTrackPanelTests : TestContext
    {
        [Fact]
        public void SetParameterFastTrackName_NotEmpty_RendersOk()
        {
            const string fastTrackName = "High importance!";

            // Arrange -

            // Act
            // In bUnit "cut" this is short for "component under test".
            IRenderedComponent<FastTrackPanel> cut = RenderComponent<FastTrackPanel>(parameters => parameters
              .Add(p => p.FastTrackName, fastTrackName)
            );
            IElement fastTrackNameElement = cut.Find("#fastTrackName");

            // Assert
            fastTrackNameElement.TextContent.MarkupMatches(fastTrackName);
        }

        [Fact]
        public void SetParameterFastTrackName_Empty_RendersNothing()
        {
            const string fastTrackName = "";

            // Arrange -

            // Act
            IRenderedComponent<FastTrackPanel> cut = RenderComponent<FastTrackPanel>(parameters => parameters
              .Add(p => p.FastTrackName, fastTrackName)
            );

            // Assert
            cut.MarkupMatches(fastTrackName);
        }
    }
}
