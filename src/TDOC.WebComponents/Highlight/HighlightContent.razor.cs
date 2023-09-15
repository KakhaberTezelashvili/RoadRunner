namespace TDOC.WebComponents.Highlight
{
    public partial class HighlightContent
    {
        [Inject]
        private CustomTimer _timer { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public string CssClass { get; set; } = "";

        private bool highlight;
        public bool Highlight
        {
            get => highlight;
            set
            {
                if (highlight != value)
                {
                    highlight = value;
                    StartTimerToHighlightContent();
                }
            }
        }

        private const double delayBeforeClearHighlighting = 4000;

        private void StartTimerToHighlightContent()
        {
            if (highlight)
                _timer.ExecActionAfterSomeDelay(ClearHighlightingOfContent, delayBeforeClearHighlighting);
        }

        private void ClearHighlightingOfContent()
        {
            highlight = false;
            StateHasChanged();
        }
    }
}