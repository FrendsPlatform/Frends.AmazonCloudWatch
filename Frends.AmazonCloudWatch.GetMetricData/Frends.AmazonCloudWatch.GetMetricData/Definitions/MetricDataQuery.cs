using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Frends.AmazonCloudWatch.GetMetricData.Attributes;
using Frends.AmazonCloudWatch.GetMetricData.Enums;

namespace Frends.AmazonCloudWatch.GetMetricData.Definitions;

/// <summary>
/// Single metric or math expression query.
/// </summary>
public class MetricDataQuery
{
    /// <summary>
    /// Unique identifier of the query. Must start with a lowercase letter and contain only letters, digits and underscores.
    /// Can be referenced in expressions of other queries.
    /// </summary>
    /// <example>cpu</example>
    [DisplayFormat(DataFormatString = "Text")]
    [Required]
    [StringLength(255)]
    [RegularExpression("^[a-z][a-zA-Z0-9_]*$", ErrorMessage = "Query Id must start with a lowercase letter and contain only letters, digits and underscores.")]
    public string Id { get; set; }

    /// <summary>
    /// Whether the query retrieves a single metric or evaluates a math or search expression.
    /// </summary>
    /// <example>Metric</example>
    [DefaultValue(QueryType.Metric)]
    public QueryType QueryType { get; set; } = QueryType.Metric;

    /// <summary>
    /// Namespace of the metric.
    /// </summary>
    /// <example>AWS/EC2</example>
    [UIHint(nameof(QueryType), "", QueryType.Metric)]
    [RequiredIf(nameof(QueryType), QueryType.Metric)]
    [DisplayFormat(DataFormatString = "Text")]
    [StringLength(255)]
    public string Namespace { get; set; }

    /// <summary>
    /// Name of the metric.
    /// </summary>
    /// <example>CPUUtilization</example>
    [UIHint(nameof(QueryType), "", QueryType.Metric)]
    [RequiredIf(nameof(QueryType), QueryType.Metric)]
    [DisplayFormat(DataFormatString = "Text")]
    [StringLength(255)]
    public string MetricName { get; set; }

    /// <summary>
    /// Dimensions of the metric, up to 30 items. All dimensions of the metric must be given to match it.
    /// </summary>
    /// <example>[{ Name = "InstanceId", Value = "i-1234567890abcdef0" }]</example>
    [UIHint(nameof(QueryType), "", QueryType.Metric)]
    [MaxLength(30, ErrorMessage = "A metric can have at most 30 dimensions.")]
    public Dimension[] Dimensions { get; set; }

    /// <summary>
    /// Granularity of the returned data points in seconds.
    /// Must be 1, 5, 10, 20, 30 (high-resolution metrics) or a multiple of 60.
    /// </summary>
    /// <example>300</example>
    [UIHint(nameof(QueryType), "", QueryType.Metric)]
    [DefaultValue(60)]
    public int Period { get; set; } = 60;

    /// <summary>
    /// Statistic to return. Standard statistics are SampleCount, Average, Sum, Minimum and Maximum.
    /// Extended statistics such as p99, tm90, wm99, tc90 or TS(10%:90%) are also supported.
    /// </summary>
    /// <example>Average</example>
    [UIHint(nameof(QueryType), "", QueryType.Metric)]
    [RequiredIf(nameof(QueryType), QueryType.Metric)]
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("Average")]
    public string Statistic { get; set; } = "Average";

    /// <summary>
    /// Unit to filter the data points by. Undefined returns data points of all units.
    /// </summary>
    /// <example>Percent</example>
    [UIHint(nameof(QueryType), "", QueryType.Metric)]
    [DefaultValue(MetricUnit.Undefined)]
    public MetricUnit Unit { get; set; } = MetricUnit.Undefined;

    /// <summary>
    /// Math or search expression, for example SUM(METRICS()) or m1/m2*100. Other queries can be referenced by their Id.
    /// </summary>
    /// <example>cpu * 2</example>
    [UIHint(nameof(QueryType), "", QueryType.Expression)]
    [RequiredIf(nameof(QueryType), QueryType.Expression)]
    [DisplayFormat(DataFormatString = "Text")]
    [StringLength(2048)]
    public string Expression { get; set; }

    /// <summary>
    /// Human-readable label of the returned time series. Leave empty to let CloudWatch generate one.
    /// </summary>
    /// <example>CPU usage</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string Label { get; set; }

    /// <summary>
    /// Whether the data of this query is returned. Set to false for queries used only as input to an expression.
    /// </summary>
    /// <example>true</example>
    [DefaultValue(true)]
    public bool ReturnData { get; set; } = true;

    /// <summary>
    /// ID of the AWS account owning the metric. Used with CloudWatch cross-account observability from a monitoring account.
    /// Leave empty to query the current account.
    /// </summary>
    /// <example>123456789012</example>
    [DisplayFormat(DataFormatString = "Text")]
    [StringLength(255)]
    public string AccountId { get; set; }
}
