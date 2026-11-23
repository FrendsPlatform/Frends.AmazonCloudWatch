namespace Frends.AmazonCloudWatch.GetMetricData.Definitions;

/// <summary>
/// Result of the task.
/// </summary>
public class Result
{
    /// <summary>
    /// Indicates if the task completed successfully.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; set; }

    /// <summary>
    /// Returned time series, one per query with ReturnData enabled.
    /// </summary>
    /// <example>[{ Id = "cpu", Label = "CPUUtilization", StatusCode = "Complete", Timestamps = [2026-11-22T10:00:00Z], Values = [12.5] }]</example>
    public List<MetricDataResult> MetricDataResults { get; set; } = [];

    /// <summary>
    /// Token to fetch the next page of data. Null when there is no more data.
    /// </summary>
    /// <example>eyJ0b2tlbiI6IjEyMyJ9</example>
    public string NextToken { get; set; }

    /// <summary>
    /// Messages about the whole request returned by CloudWatch.
    /// </summary>
    /// <example>[{ Code = "MaxQueryTimeRangeExceed", Value = "..." }]</example>
    public List<Message> Messages { get; set; } = [];

    /// <summary>
    /// Error that occurred during task execution.
    /// </summary>
    /// <example>object { string Message, Exception AdditionalInfo }</example>
    public Error Error { get; set; }
}
