namespace ScannerClient.WebApp.Workflows.Shared
{
    public partial class WorkflowLastActionPanel
    {
        [Parameter]
        public string Text { get; set; }

        [Parameter]
        public string FastTrackText { get; set; }

        [Parameter]
        public bool Highlight
        {
            get => refHighlightContent != null ? refHighlightContent.Highlight : false;
            set
            {
                if (refHighlightContent != null)
                    refHighlightContent.Highlight = value;
            }
        }

        private HighlightContent refHighlightContent;
    }
}
