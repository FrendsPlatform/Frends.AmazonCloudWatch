namespace Frends.AmazonCloudWatch.PutMetricData.Enums;

public enum AuthenticationType
{
    /// <summary>
    /// AWS access key credentials consisting of an access key and secret key.
    /// An optional session token can be provided when using temporary credentials.
    /// </summary>
    AccessKey,

    /// <summary>
    /// Uses the default AWS SDK for .NET credential provider chain.
    /// Credentials are discovered automatically from the runtime environment, for example
    /// environment variables, shared credentials/config files (including SSO profiles),
    /// EKS Web Identity (IRSA), EKS Pod Identity, ECS task roles and EC2 instance profiles.
    /// The order of the sources is defined by the AWS SDK.
    /// </summary>
    DefaultCredentialChain,

    /// <summary>
    /// Assumes an IAM role through AWS STS. Source credentials are obtained from
    /// the provided AccessKey and SecretKey (optionally with SessionToken).
    /// If no access keys are provided, the AWS default credential chain is used.
    /// </summary>
    AssumeRole,

    /// <summary>
    /// Loads credentials from a named AWS profile.
    /// Profiles using role_arn/source_profile, IAM Identity Center (SSO) or credential_process are also supported.
    /// Role assumption configured in a profile does not require the AssumeRole mode.
    /// </summary>
    AwsProfile,
}
