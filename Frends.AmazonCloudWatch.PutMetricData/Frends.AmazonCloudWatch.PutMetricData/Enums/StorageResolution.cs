namespace Frends.AmazonCloudWatch.PutMetricData.Enums;

/// <summary>
/// Resolution at which CloudWatch stores a metric.
/// </summary>
public enum StorageResolution
{
    /// <summary>
    /// Regular resolution, stored at 1-minute granularity.
    /// </summary>
    Standard,

    /// <summary>
    /// High resolution, stored with sub-minute granularity down to one second.
    /// </summary>
    High,
}
