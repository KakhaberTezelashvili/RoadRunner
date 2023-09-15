using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace TDOC.Common.Server.Conventions;

internal class ControllerRoutingConvention : Attribute, IControllerModelConvention
{
    private readonly string _routingPrefix;

    public ControllerRoutingConvention(string routingPrefix)
    {
        _routingPrefix = routingPrefix;
    }

    public void Apply(ControllerModel controller)
    {
        controller.Selectors.ToList().ForEach(selector =>
        {
            if (selector.AttributeRouteModel != null)
                selector.AttributeRouteModel.Template = $"{_routingPrefix}/{selector.AttributeRouteModel.Template}";
        });
    }
}