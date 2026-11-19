using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Frends.AmazonCloudWatch.PutMetricData.Attributes;
using Frends.AmazonCloudWatch.PutMetricData.Enums;

namespace Frends.AmazonCloudWatch.PutMetricData.Definitions;

/// <summary>
/// Connection parameters.
/// </summary>
public class Connection
{
    /// <summary>
    /// AWS authentication method.
    /// </summary>
    /// <example>DefaultCredentialChain</example>
    [DefaultValue(AuthenticationType.DefaultCredentialChain)]
    public AuthenticationType AuthenticationType { get; set; }
        = AuthenticationType.DefaultCredentialChain;

    /// <summary>
    /// AWS Region to connect to.
    /// </summary>
    /// <example>EuNorth1</example>
    public Regions Region { get; set; }

    /// <summary>
    /// AWS Access Key used for authentication.
    /// In AssumeRole mode it is optional and used as source credentials to call STS.
    /// </summary>
    /// <example>AKIAIOSFODNN7EXAMPLE</example>
    [UIHint(nameof(AuthenticationType), "", AuthenticationType.AccessKey, AuthenticationType.AssumeRole)]
    [RequiredIf(nameof(AuthenticationType), AuthenticationType.AccessKey)]
    public string AccessKey { get; set; }

    /// <summary>
    /// AWS Secret Access Key used for authentication.
    /// In AssumeRole mode it is optional (but required if AccessKey is given) and used as source credentials to call STS.
    /// </summary>
    /// <example>your-secret-access-key</example>
    [UIHint(nameof(AuthenticationType), "", AuthenticationType.AccessKey, AuthenticationType.AssumeRole)]
    [PasswordPropertyText(true)]
    public string SecretKeySecret { get; set; }

    /// <summary>
    /// AWS Session Token used with temporary credentials.
    /// In AssumeRole mode it is optional and used as source credentials to call STS.
    /// </summary>
    /// <example>your-session-token</example>
    [UIHint(nameof(AuthenticationType), "", AuthenticationType.AccessKey, AuthenticationType.AssumeRole)]
    [PasswordPropertyText(true)]
    public string SessionTokenSecret { get; set; }

    /// <summary>
    /// ARN of the IAM role to assume.
    /// </summary>
    /// <example>arn:aws:iam::123456789012:role/IntegrationRole</example>
    [UIHint(nameof(AuthenticationType), "", AuthenticationType.AssumeRole)]
    [RequiredIf(nameof(AuthenticationType), AuthenticationType.AssumeRole)]
    public string RoleArn { get; set; }

    /// <summary>
    /// Name identifying the assumed role session. Visible in CloudTrail logs.
    /// Must be 2-64 characters long and may contain letters, digits and the characters _=,.@-
    /// </summary>
    /// <example>FrendsSession</example>
    [UIHint(nameof(AuthenticationType), "", AuthenticationType.AssumeRole)]
    [DefaultValue("FrendsSession")]
    [RequiredIf(nameof(AuthenticationType), AuthenticationType.AssumeRole)]
    public string RoleSessionName { get; set; } = "FrendsSession";

    /// <summary>
    /// External ID used when assuming a role in another AWS account.
    /// Provide this value if required by the target role's trust policy.
    /// </summary>
    /// <example>customer-external-id</example>
    [UIHint(nameof(AuthenticationType), "", AuthenticationType.AssumeRole)]
    public string ExternalId { get; set; }

    /// <summary>
    /// Requested duration of the assumed role session in seconds (900-43200).
    /// Leave empty to use the SDK default (3600 seconds).
    /// The value cannot exceed the maximum session duration configured on the role,
    /// otherwise STS returns an error. With role chaining the maximum is 3600 seconds.
    /// </summary>
    /// <example>3600</example>
    [UIHint(nameof(AuthenticationType), "", AuthenticationType.AssumeRole)]
    [Range(900, 43200)]
    public int? DurationSeconds { get; set; }

    /// <summary>
    /// Name of the AWS credentials profile to use.
    /// </summary>
    /// <example>production</example>
    [UIHint(nameof(AuthenticationType), "", AuthenticationType.AwsProfile)]
    [RequiredIf(nameof(AuthenticationType), AuthenticationType.AwsProfile)]
    public string ProfileName { get; set; }

    /// <summary>
    /// Optional path to the shared AWS credentials file.
    /// If empty, the AWS SDK uses its default profile locations,
    /// including the shared credentials and config files.
    /// If specified, the SDK searches the specified credentials file.
    /// </summary>
    /// <example>C:\Users\User\.aws\credentials</example>
    [UIHint(nameof(AuthenticationType), "", AuthenticationType.AwsProfile)]
    public string ProfilesLocation { get; set; }
}
