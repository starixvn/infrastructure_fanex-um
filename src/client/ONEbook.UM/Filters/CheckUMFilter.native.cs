#if NET5_0_OR_GREATER

namespace ONEbook.UM.Filters;

using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Configurations;
using Services;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class CheckUMFilterAttribute(IMaintenanceService maintenanceService, UMClientConfiguration configuration)
    : ActionFilterAttribute
{
    private readonly UMClientConfiguration configuration = configuration ?? UMClientManager.UMClientConfigs;

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var requestPath = context.HttpContext.Request.Path.Value ?? string.Empty;
        if(requestPath.Contains("healthz", StringComparison.OrdinalIgnoreCase))
        {
            await base.OnActionExecutionAsync(context, next);
            return;
        }

        var actionDescriptor = (ControllerActionDescriptor)context.ActionDescriptor;
        var attributes = actionDescriptor.MethodInfo.CustomAttributes;
        if (attributes.Any(a => a.AttributeType == typeof(SkipUMFilterAttribute)))
        {
            await base.OnActionExecutionAsync(context, next);
            return;
        }

        var isUnderMaintenance = await maintenanceService.IsUnderMaintenanceAsync();

        if (isUnderMaintenance)
        {
            if (IsAjax(context.HttpContext.Request))
            {
                var response = context.HttpContext.Response;
                response.ContentType = "application/json";
                response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                var result = JsonConvert.SerializeObject(new { code = 999_999, message = "Under maintenance time." });
                await response.WriteAsync(result);
                return;
            }

            UnderMaintenanceAction(context);
        }

        var umTime = await maintenanceService.GetUnderMaintenanceTimeAsync();

        if (!string.IsNullOrWhiteSpace(configuration.ErrorUMPageUrl) && umTime.ConnectionResult.IsNotOk)
        {
            ErrorUnderMaintenanceAction(context);
        }

        await base.OnActionExecutionAsync(context, next);
    }

    public void UnderMaintenanceAction(ActionExecutingContext filterContext)
    {
        var underMaintenancePageAbsoluteUrl = UriHelper.BuildAbsolute(
            filterContext.HttpContext.Request.Scheme,
            filterContext.HttpContext.Request.Host,
            configuration.UMPageUrl);
        var currentPageAbsoluteUrl = filterContext.HttpContext.Request.GetDisplayUrl();

        if (underMaintenancePageAbsoluteUrl != currentPageAbsoluteUrl)
        {
            filterContext.Result = new RedirectResult(configuration.UMPageUrl, true);
        }
    }

    public void ErrorUnderMaintenanceAction(ActionExecutingContext filterContext)
    {
        var underMaintenancePageAbsoluteUrl = UriHelper.BuildAbsolute(
            configuration.ErrorUMPageUrl,
            filterContext.HttpContext.Request.Host);
        var currentPageAbsoluteUrl = filterContext.HttpContext.Request.GetDisplayUrl();

        if (underMaintenancePageAbsoluteUrl != currentPageAbsoluteUrl)
        {
            filterContext.Result = new RedirectResult(configuration.ErrorUMPageUrl, true);
        }
    }

    private static bool IsAjax(HttpRequest request)
        => request.Headers != null && request.Headers["X-Requested-With"] == "XMLHttpRequest";
}

#endif