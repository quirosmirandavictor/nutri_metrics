using System.Diagnostics;
using MediatR;

namespace NutriMetrics.Shared.Infrastructure.Observability;

public sealed class TracingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var moduleName = ExtractModuleName(typeof(TRequest).Namespace);

        using var activity = NutriMetricsActivitySource.Instance.StartActivity(
            requestName,
            ActivityKind.Internal);

        activity?.SetTag("mediatr.request", requestName);
        activity?.SetTag("mediatr.module", moduleName);
        activity?.SetTag("mediatr.request_type", GetRequestKind(requestName));

        try
        {
            var response = await next();
            activity?.SetStatus(ActivityStatusCode.Ok);
            return response;
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.AddException(ex);
            throw;
        }
    }

    private static string ExtractModuleName(string? namespaceValue)
    {
        // e.g. "NutriMetrics.Modules.Identity.Application" -> "Identity"
        if (string.IsNullOrEmpty(namespaceValue))
        {
            return "Unknown";
        }

        var parts = namespaceValue.Split('.');
        var moduleIndex = Array.IndexOf(parts, "Modules");

        return moduleIndex >= 0 && moduleIndex + 1 < parts.Length
            ? parts[moduleIndex + 1]
            : "Unknown";
    }

    private static string GetRequestKind(string requestName) =>
        requestName.EndsWith("Command", StringComparison.Ordinal) ? "Command" :
        requestName.EndsWith("Query", StringComparison.Ordinal) ? "Query" :
        "Unknown";
}