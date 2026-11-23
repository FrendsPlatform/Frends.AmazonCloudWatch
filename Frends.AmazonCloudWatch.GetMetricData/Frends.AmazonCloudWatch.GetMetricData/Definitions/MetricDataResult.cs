namespace Frends.AmazonCloudWatch.GetMetricData.Definitions;

/// <summary>
/// Time series returned for a single query.
/// </summary>
public class MetricDataResult
{
    /// <summary>
    /// Id of the query that produced this time series.
    /// </summary>
    /// <example>cpu</example>
    public string Id { get; set; }

    /// <summary>
    /// Label of the time series.
    /// </summary>
    /// <example>CPUUtilization</example>
    public string Label { get; set; }

    /// <summary>
    /// Status of the returned data: Complete, PartialData, InternalError or Forbidden.
    /// </summary>
    /// <example>Complete</example>
    public string StatusCode { get; set; }

    /// <summary>
    /// Timestamps of the data points in UTC. Each timestamp corresponds to the value at the same index in Values.
    /// </summary>
    /// <example>[2026-11-22T10:00:00Z, 2026-11-22T10:05:00Z]</example>
    public List<DateTime> Timestamps { get; set; } = [];

    /// <summary>
    /// Values of the data points.
    /// </summary>
    /// <example>[12.5, 13.1]</example>
    public List<double> Values { get; set; } = [];

    /// <summary>
    /// Messages about this time series returned by CloudWatch.
    /// </summary>
    /// <example>[{ Code = "ArithmeticError", Value = "..." }]</example>
    public List<Message> Messages { get; set; } = [];
}
