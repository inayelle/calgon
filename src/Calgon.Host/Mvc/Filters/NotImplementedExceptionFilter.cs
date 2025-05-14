using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Calgon.Host.Mvc.Filters;

internal sealed class NotImplementedExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        context.Result = new StatusCodeResult((int)HttpStatusCode.NotImplemented);

        context.ExceptionHandled = true;
    }
}