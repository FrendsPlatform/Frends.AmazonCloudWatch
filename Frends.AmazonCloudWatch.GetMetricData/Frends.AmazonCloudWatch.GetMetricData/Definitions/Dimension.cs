using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.AmazonCloudWatch.GetMetricData.Definitions;

/// <summary>
/// Name and value pair that is part of the identity of a metric.
/// </summary>
public class Dimension
{
    /// <summary>
    /// Name of the dimension. ASCII characters only, can't start with a colon.
    /// </summary>
    /// <example>InstanceId</example>
    [DisplayFormat(DataFormatString = "Text")]
    [Required]
    [StringLength(255)]
    [RegularExpression(@"^(?!:)[\x20-\x7E]+$", ErrorMessage = "Dimension name must contain only printable ASCII characters and can't start with a colon.")]
    public string Name { get; set; }

    /// <summary>
    /// Value of the dimension. ASCII characters only.
    /// </summary>
    /// <example>i-1234567890abcdef0</example>
    [DisplayFormat(DataFormatString = "Text")]
    [Required]
    [StringLength(1024)]
    [RegularExpression(@"^[\x20-\x7E]+$", ErrorMessage = "Dimension value must contain only printable ASCII characters.")]
    public string Value { get; set; }
}
