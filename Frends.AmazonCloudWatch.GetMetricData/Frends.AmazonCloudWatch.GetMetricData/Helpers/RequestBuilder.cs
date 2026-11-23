using Amazon.CloudWatch;
using Amazon.CloudWatch.Model;
using Frends.AmazonCloudWatch.GetMetricData.Definitions;
using Frends.AmazonCloudWatch.GetMetricData.Enums;
using MetricDataQuery = Frends.AmazonCloudWatch.GetMetricData.Definitions.MetricDataQuery;

namespace Frends.AmazonCloudWatch.GetMetricData.Helpers;

internal static class RequestBuilder
{
    internal static GetMetricDataRequest Build(Input input, Options options, List<Amazon.CloudWatch.Model.MetricDataQuery> queries) => new()
    {
        StartTime = ToUtc(input.StartTime),
        EndTime = ToUtc(input.EndTime),
        MetricDataQueries = queries,
        NextToken = string.IsNullOrWhiteSpace(input.NextToken) ? null : input.NextToken,
        MaxDatapoints = options.MaxDatapoints,
        ScanBy = options.ScanBy == Enums.ScanBy.TimestampAscending
            ? Amazon.CloudWatch.ScanBy.TimestampAscending
            : Amazon.CloudWatch.ScanBy.TimestampDescending,
        LabelOptions = string.IsNullOrWhiteSpace(options.LabelTimezone)
            ? null
            : new LabelOptions { Timezone = options.LabelTimezone },
    };

    internal static DateTime ToUtc(DateTime timestamp) =>
        timestamp.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(timestamp, DateTimeKind.Utc)
            : timestamp.ToUniversalTime();

    internal static Amazon.CloudWatch.Model.MetricDataQuery ToQuery(MetricDataQuery query) => new()
    {
        Id = query.Id,
        Label = NullIfEmpty(query.Label),
        ReturnData = query.ReturnData,
        AccountId = NullIfEmpty(query.AccountId),
        Expression = query.QueryType == Enums.QueryType.Expression ? query.Expression : null,
        MetricStat = query.QueryType == Enums.QueryType.Metric ? ToMetricStat(query) : null,
    };

    private static MetricStat ToMetricStat(MetricDataQuery query) => new()
    {
        Metric = new Metric
        {
            Namespace = query.Namespace,
            MetricName = query.MetricName,
            Dimensions = (query.Dimensions ?? [])
                .Select(d => new Amazon.CloudWatch.Model.Dimension { Name = d.Name, Value = d.Value })
                .ToList(),
        },
        Period = query.Period,
        Stat = query.Statistic,
        Unit = query.Unit == MetricUnit.Undefined
            ? null
            : StandardUnit.FindValue(query.Unit.ToString().Replace("PerSecond", "/Second")),
    };

    private static string NullIfEmpty(string value) => string.IsNullOrWhiteSpace(value) ? null : value;
}
