using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Frends.AmazonCloudWatch.GetMetricData.Enums;

namespace Frends.AmazonCloudWatch.GetMetricData.Definitions;

/// <summary>
/// Additional parameters.
/// </summary>
public class Options
{
    /// <summary>
    /// Order of the returned data points.
    /// </summary>
    /// <example>TimestampDescending</example>
    [DefaultValue(ScanBy.TimestampDescending)]
    public ScanBy ScanBy { get; set; } = ScanBy.TimestampDescending;

    /// <summary>
    /// Maximum number of data points returned in a single call. If more data is available, the result contains NextToken.
    /// Leave empty to use the AWS default of 100800.
    /// </summary>
    /// <example>1000</example>
    [Range(1, int.MaxValue)]
    public int? MaxDatapoints { get; set; }

    /// <summary>
    /// Time zone used in labels of the returned data, in format +HHMM or -HHMM. Leave empty to use UTC.
    /// Affects only dynamic labels and labels of expressions, not the returned timestamps.
    /// </summary>
    /// <example>+0200</example>
    [DisplayFormat(DataFormatString = "Text")]
    [RegularExpression(@"^[+-]\d{4}$", ErrorMessage = "LabelTimezone must be in format +HHMM or -HHMM.")]
    public string LabelTimezone { get; set; }

    /// <summary>
    /// Whether to throw an error on failure.
    /// </summary>
    /// <example>true</example>
    [DefaultValue(true)]
    public bool ThrowErrorOnFailure { get; set; } = true;

    /// <summary>
    /// Overrides the error message on failure.
    /// </summary>
    /// <example>Custom error message</example>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("")]
    public string ErrorMessageOnFailure { get; set; } = string.Empty;
}
