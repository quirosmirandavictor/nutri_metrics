using System.Diagnostics;

namespace NutriMetrics.Shared.Infrastructure.Observability;

public static class NutriMetricsActivitySource
{
    public const string Name = "NutriMetrics.Application";

    public static readonly ActivitySource Instance = new(Name);
}