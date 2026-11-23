namespace Frends.AmazonCloudWatch.GetMetricData.Definitions;

/// <summary>
/// Informational or error message returned by CloudWatch.
/// </summary>
public class Message
{
    /// <summary>
    /// Code of the message.
    /// </summary>
    /// <example>ArithmeticError</example>
    public string Code { get; set; }

    /// <summary>
    /// Text of the message.
    /// </summary>
    /// <example>One or more data points have been dropped due to non-numeric values (NaN, -Infinite, +Infinite)</example>
    public string Value { get; set; }
}
