using Microsoft.AspNetCore.Mvc.ApplicationModels;
using RentalPeAPI.Shared.Infrastructure.Interfaces.ASP.Configuration.Extensions;

namespace RentalPeAPI.Shared.Infrastructure.Interfaces.ASP.Configuration;

/// <summary>
/// A convention that transforms controller and action route templates to the kebab-case.
/// </summary>
public class KebabCaseRouteNamingConvention : IControllerModelConvention
{
    /// <summary>
    /// Replaces the [controller] token in the route template with the kebab-case version of the name.
    /// </summary>
    /// <param name="selector">The selector model containing the route.</param>
    /// <param name="name">The name to transform.</param>
    /// <returns>The updated attribute route model, or null if no route model exists.</returns>
    private static AttributeRouteModel? ReplaceControllerTemplate(SelectorModel selector, string name)
    {
        return selector.AttributeRouteModel != null
            ? new AttributeRouteModel
            {
                Template = selector.AttributeRouteModel.Template?.Replace("[controller]", name.ToKebabCase())
            }
            : null;
    }
    
    /// <summary>
    /// Applies the kebab-case naming convention to the controller's routes.
    /// </summary>
    /// <param name="controller">The controller model to modify.</param>
    public void Apply(ControllerModel controller)
    {
        foreach (var selector in controller.Selectors) 
            selector.AttributeRouteModel = ReplaceControllerTemplate(selector, controller.ControllerName);
        foreach (var selector in controller.Actions.SelectMany(a => a.Selectors))
            selector.AttributeRouteModel = ReplaceControllerTemplate(selector, controller.ControllerName);
    }
}