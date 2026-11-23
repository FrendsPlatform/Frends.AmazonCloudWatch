namespace Frends.AmazonCloudWatch.GetMetricData.Enums;

/// <summary>
/// How the metric queries are provided.
/// </summary>
public enum QueryInputMode
{
    /// <summary>
    /// Queries are defined with the form fields of the Queries parameter.
    /// </summary>
    Form,

    /// <summary>
    /// Queries are provided as a JSON array in the format used by the AWS CLI and the CloudWatch console.
    /// </summary>
    Json,
}
