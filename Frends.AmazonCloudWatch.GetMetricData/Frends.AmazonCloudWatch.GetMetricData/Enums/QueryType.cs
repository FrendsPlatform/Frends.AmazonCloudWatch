namespace Frends.AmazonCloudWatch.GetMetricData.Enums;

/// <summary>
/// Type of a metric data query.
/// </summary>
public enum QueryType
{
    /// <summary>
    /// Retrieves a single metric identified by namespace, name and dimensions.
    /// </summary>
    Metric,

    /// <summary>
    /// Evaluates a metric math or search expression.
    /// </summary>
    Expression,
}
