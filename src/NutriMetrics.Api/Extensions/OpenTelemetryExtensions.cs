using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace NutriMetrics.Api.Extensions;
using NutriMetrics.Shared.Infrastructure.Observability;

public static class OpenTelemetryExtensions
{
    public static IServiceCollection AddNutriMetricsObservability(
        this IServiceCollection services, IConfiguration config)
    {
        var endpoint = new Uri(config["Otel:Endpoint"] ?? "http://localhost:4317");
        const string serviceName = "nutrimetrics-api";

        services.AddOpenTelemetry()
            .ConfigureResource(r => r.AddService(serviceName))
            .WithTracing(t => t
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddEntityFrameworkCoreInstrumentation()
                .AddSource(NutriMetricsActivitySource.Name)
                .AddOtlpExporter(o => o.Endpoint = endpoint))
            .WithMetrics(m => m
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                .AddOtlpExporter(o => o.Endpoint = endpoint));

        return services;
    }

    public static ILoggingBuilder AddNutriMetricsLogging(
        this ILoggingBuilder logging, IConfiguration config)
    {
        var endpoint = new Uri(config["Otel:Endpoint"] ?? "http://localhost:4317");

        logging.AddOpenTelemetry(o =>
        {
            o.IncludeFormattedMessage = true;
            o.IncludeScopes = true;
            o.AddOtlpExporter(e => e.Endpoint = endpoint);
        });

        return logging;
    }
}