using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Frends.AmazonCloudWatch.PutMetricData.Enums;

namespace Frends.AmazonCloudWatch.PutMetricData.Definitions;

/// <summary>
/// A single metric data point.
/// </summary>
public class MetricDatum
{
    /// <summary>
    /// Name of the metric.
    /// </summary>
    /// <example>OrdersProcessed</example>
    [DisplayFormat(DataFormatString = "Text")]
    [Required]
    [StringLength(255)]
    public string MetricName { get; set; }

    /// <summary>
    /// Value of the metric. NaN and Infinity are not supported, the value must be between -2^360 and 2^360.
    /// </summary>
    /// <example>42</example>
    [Range(-2.348542582773833E+108, 2.348542582773833E+108, ErrorMessage = "Metric value must be a finite number between -2^360 and 2^360.")]
    public double Value { get; set; }

    /// <summary>
    /// Unit of the value.
    /// </summary>
    /// <example>MetricUnit.Count</example>
    [DefaultValue(MetricUnit.None)]
    public MetricUnit Unit { get; set; }

    /// <summary>
    /// Time of the data point. DateTime of unspecified kind is treated as UTC. When empty, CloudWatch uses the time it received the request.
    /// </summary>
    /// <example>2025-01-31T12:00:00Z</example>
    public DateTime? Timestamp { get; set; }

    /// <summary>
    /// Dimensions of the metric, up to 30.
    /// </summary>
    /// <example>[{ Name = "Environment", Value = "Production" }]</example>
    [MaxLength(30, ErrorMessage = "A metric can have at most 30 dimensions.")]
    public Dimension[] Dimensions { get; set; }

    /// <summary>
    /// Resolution at which CloudWatch stores the metric. High resolution is available only for custom metrics.
    /// </summary>
    /// <example>StorageResolution.Standard</example>
    [DefaultValue(StorageResolution.Standard)]
    public StorageResolution StorageResolution { get; set; }
}
