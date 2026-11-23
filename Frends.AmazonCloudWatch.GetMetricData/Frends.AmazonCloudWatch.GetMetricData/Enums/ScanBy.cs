namespace Frends.AmazonCloudWatch.GetMetricData.Enums;

/// <summary>
/// Order of the returned data points.
/// </summary>
public enum ScanBy
{
    /// <summary>
    /// Newest data points first.
    /// </summary>
    TimestampDescending,

    /// <summary>
    /// Oldest data points first.
    /// </summary>
    TimestampAscending,
}
