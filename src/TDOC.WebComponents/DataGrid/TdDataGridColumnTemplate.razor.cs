namespace TDOC.WebComponents.DataGrid
{
    public partial class TdDataGridColumnTemplate<TItem> : IDisposable
    {
        [CascadingParameter]
        private TdDataGrid<TItem> DataGrid { get; set; }

        [Parameter]
        public string Field { get; set; }

        [Parameter]
        public RenderFragment<TItem> DisplayTemplate { get; set; }

        protected override void OnInitialized() => DataGrid.AddColumnTemplate(this);

        public void Dispose() => DataGrid.RemoveColumnTemplate(this);
    }
}