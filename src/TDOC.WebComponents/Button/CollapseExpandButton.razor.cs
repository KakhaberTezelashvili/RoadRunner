namespace TDOC.WebComponents.Button
{
    public partial class CollapseExpandButton
    {
        [Parameter]
        public string CssClass { get; set; }

        [Parameter]
        public bool Expanded { get; set; }

        [Parameter]
        public string IdentifierToCollapse { get; set; }

        [Parameter]
        public EventCallback OnClick { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        private string href;

        protected override void OnParametersSet()
        {
            href = "#" + IdentifierToCollapse;
        }
    }
}
