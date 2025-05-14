using System.Reflection;
using Calgon.Host.Controllers;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Calgon.Host.Mvc.Features;

internal sealed class InternalControllerFeatureProvider : ControllerFeatureProvider
{
    protected override bool IsController(TypeInfo typeInfo)
    {
        return typeInfo is
               {
                   IsClass: true,
                   IsGenericType: false,
                   ContainsGenericParameters: false,
                   IsAbstract: false,
                   IsInterface: false,
               } &&
               typeInfo.IsAssignableTo(typeof(ApplicationController));
    }
}