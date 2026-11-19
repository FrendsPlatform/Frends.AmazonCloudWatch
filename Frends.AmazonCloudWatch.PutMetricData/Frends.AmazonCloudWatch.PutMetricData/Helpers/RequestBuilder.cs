using Amazon.CloudWatch;
using Amazon.CloudWatch.Model;
using Frends.AmazonCloudWatch.PutMetricData.Enums;
using Input = Frends.AmazonCloudWatch.PutMetricData.Definitions.Input;
using MetricDatum = Frends.AmazonCloudWatch.PutMetricData.Definitions.MetricDatum;

namespace Frends.AmazonCloudWatch.PutMetricData.Helpers;

internal static class RequestBuilder
{
    internal static PutMetricDataRequest Build(Input input) => new()
    {
        Namespace = input.Namespace,
        MetricData = input.Metrics.Select(ToDatum).ToList(),
    };

    private static Amazon.CloudWatch.Model.MetricDatum ToDatum(MetricDatum metric) => new()
    {
        MetricName = metric.MetricName,
        Value = metric.Value,
        Unit = StandardUnit.FindValue(metric.Unit.ToString().Replace("PerSecond", "/Second")),
        Timestamp = ToUtc(metric.Timestamp),
        StorageResolution = metric.StorageResolution == StorageResolution.High ? 1 : 60,
        Dimensions = (metric.Dimensions ?? [])
            .Select(d => new Dimension { Name = d.Name, Value = d.Value })
            .ToList(),
    };

    private static DateTime? ToUtc(DateTime? timestamp)
    {
        if (timestamp == null) return null;
        return timestamp.Value.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(timestamp.Value, DateTimeKind.Utc)
            : timestamp.Value.ToUniversalTime();
    }
}
