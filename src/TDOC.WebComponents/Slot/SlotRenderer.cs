using Microsoft.AspNetCore.Components.Rendering;

namespace TDOC.WebComponents.Slot;

public sealed class SlotRenderer : IComponent
{
    [Parameter]
    public string Name { get; set; }

    [Parameter]
    public string CssClass { get; set; }

    [Parameter]
    public string CssStyle { get; set; }

    [Parameter]
    public Func<RenderFragment> SlotContent { get; set; }

    private bool initialized;
    private Action renderSlotContent;

    public Task SetParametersAsync(ParameterView parameters)
    {
        if (!initialized)
        {
            initialized = true;
            foreach (ParameterValue p in parameters)
            {
                if (p.Name.Equals(nameof(Name), StringComparison.OrdinalIgnoreCase))
                    Name = (string)p.Value;
                else if (p.Name.Equals(nameof(CssClass), StringComparison.OrdinalIgnoreCase))
                    CssClass = (string)p.Value;
                else if (p.Name.Equals(nameof(CssStyle), StringComparison.OrdinalIgnoreCase))
                    CssStyle = (string)p.Value;
                else if (p.Name.Equals(nameof(SlotContent), StringComparison.OrdinalIgnoreCase))
                    SlotContent = (Func<RenderFragment>)p.Value;
            }
            renderSlotContent();
        }
        return Task.CompletedTask;
    }

    public void Attach(RenderHandle handle) => 
        renderSlotContent = () => handle.Render(b => BuildSlotContent(b));

    public void Refresh() => renderSlotContent();

    private void BuildSlotContent(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "div");
        builder.AddAttribute(0, "slot", Name);
        builder.AddAttribute(0, "class", CssClass);
        builder.AddAttribute(0, "style", CssStyle);
        SlotContent().Invoke(builder);
        builder.CloseElement();
    }
}