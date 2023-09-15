namespace TDOC.WebComponents.Tab;

public partial class TdTabs
{
    [Parameter]
    public bool Enabled { get; set; } = true;

    [Parameter]
    public RenderFragment ChildContent { get; set; }

    [Parameter]
    public int ActiveTabIndex
    {
        get => activeTabIndex;
        set
        {
            activeTabIndex = value;
            if (tabTemplates.Count > activeTabIndex)
                activeTabId = tabTemplates[activeTabIndex].TabId;
        }
    }

    [Parameter]
    public EventCallback<int> ActiveTabIndexChanged { get; set; }

    private int activeTabIndex;
    private int activeTabId;
    private List<TdTabTemplate> tabTemplates { get; set; } = new();

    public void AddTabTemplate(TdTabTemplate tabTemplate)
    {
        tabTemplate.TabId = tabTemplates.Count;
        tabTemplates.Add(tabTemplate);
        StateHasChanged();
    }

    public void RemoveTabTemplate(TdTabTemplate tabTemplate) => tabTemplates.Remove(tabTemplate);

    private async void OnActiveTabIndexChanged(int index)
    {
        ActiveTabIndex = index;
        await ActiveTabIndexChanged.InvokeAsync(activeTabIndex);
    }

    private void CloseTab(TdTabTemplate tabTemplate)
    {
        RemoveTabTemplate(tabTemplate);
        OnActiveTabIndexChanged(0);
    }
}