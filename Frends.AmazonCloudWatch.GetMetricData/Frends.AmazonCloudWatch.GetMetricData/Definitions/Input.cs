using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Frends.AmazonCloudWatch.GetMetricData.Attributes;
using Frends.AmazonCloudWatch.GetMetricData.Enums;

namespace Frends.AmazonCloudWatch.GetMetricData.Definitions;

/// <summary>
/// Essential parameters.
/// </summary>
public class Input
{
    /// <summary>
    /// Start of the time range (inclusive). Values with Kind Unspecified are treated as UTC, Local values are converted to UTC.
    /// CloudWatch rounds the value down based on the Period and the age of the data.
    /// </summary>
    /// <example>2026-11-22T00:00:00Z</example>
    [Required]
    public DateTime StartTime { get; set; }

    /// <summary>
    /// End of the time range (exclusive). Values with Kind Unspecified are treated as UTC, Local values are converted to UTC.
    /// Must be later than StartTime.
    /// </summary>
    /// <example>2026-11-23T00:00:00Z</example>
    [Required]
    public DateTime EndTime { get; set; }

    /// <summary>
    /// How the metric queries are provided. Form uses the Queries parameter, Json uses the QueriesJson parameter.
    /// </summary>
    /// <example>Form</example>
    [DefaultValue(QueryInputMode.Form)]
    public QueryInputMode QueryInputMode { get; set; } = QueryInputMode.Form;

    /// <summary>
    /// Metric queries to run, 1 to 500 items. Each query returns its own entry in the result, identified by its Id.
    /// </summary>
    /// <example>[{ Id = "cpu", QueryType = QueryType.Metric, Namespace = "AWS/EC2", MetricName = "CPUUtilization", Period = 300, Statistic = "Average" }]</example>
    [UIHint(nameof(QueryInputMode), "", QueryInputMode.Form)]
    [RequiredIf(nameof(QueryInputMode), QueryInputMode.Form)]
    [MaxLength(500, ErrorMessage = "A single request can contain at most 500 queries.")]
    public MetricDataQuery[] Queries { get; set; }

    /// <summary>
    /// Metric queries provided as a JSON array matching the AWS CloudWatch MetricDataQuery structure.
    /// Supports all query properties available in the AWS SDK, including MetricStat, metric math expressions and Metrics Insights queries.
    /// </summary>
    /// <example>[{ "Id": "cpu", "MetricStat": { "Metric": { "Namespace": "AWS/EC2", "MetricName": "CPUUtilization", "Dimensions": [{ "Name": "InstanceId", "Value": "i-1234567890abcdef0" }] }, "Period": 300, "Stat": "Average", "Unit": "Percent" } }, { "Id": "cpu2x", "Expression": "cpu * 2" }]</example>
    [UIHint(nameof(QueryInputMode), "", QueryInputMode.Json)]
    [RequiredIf(nameof(QueryInputMode), QueryInputMode.Json)]
    [DisplayFormat(DataFormatString = "Json")]
    public string QueriesJson { get; set; }

    /// <summary>
    /// Token returned by a previous call to fetch the next page of data.
    /// Leave empty to fetch the first page. Use the same StartTime, EndTime and Queries as in the call that returned the token.
    /// </summary>
    /// <example>eyJ0b2tlbiI6IjEyMyJ9</example>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("")]
    public string NextToken { get; set; } = string.Empty;
}
