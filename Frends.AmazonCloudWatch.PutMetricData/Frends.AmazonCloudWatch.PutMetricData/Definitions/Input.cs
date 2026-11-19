using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.AmazonCloudWatch.PutMetricData.Definitions;

/// <summary>
/// Essential parameters.
/// </summary>
public class Input
{
    /// <summary>
    /// Namespace of the metrics. ASCII characters only, can't start with a colon. Avoid the AWS/ prefix reserved for AWS services.
    /// </summary>
    /// <example>MyApp/Orders</example>
    [DisplayFormat(DataFormatString = "Text")]
    [Required]
    [StringLength(255)]
    [RegularExpression(@"^(?!:)[\x20-\x7E]+$", ErrorMessage = "Namespace must contain only printable ASCII characters and can't start with a colon.")]
    public string Namespace { get; set; }

    /// <summary>
    /// Metric data points published in a single request, 1 to 1000 items. Split larger sets in the Process.
    /// </summary>
    /// <example>[{ MetricName = "OrdersProcessed", Value = 42, Unit = MetricUnit.Count }]</example>
    [Required]
    [MinLength(1, ErrorMessage = "At least one metric is required.")]
    [MaxLength(1000, ErrorMessage = "A single request can contain at most 1000 metrics.")]
    public MetricDatum[] Metrics { get; set; }
}
