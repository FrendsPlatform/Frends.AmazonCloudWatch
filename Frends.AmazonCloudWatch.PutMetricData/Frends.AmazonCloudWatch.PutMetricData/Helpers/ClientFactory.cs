using System.Text.RegularExpressions;
using Amazon;
using Amazon.CloudWatch;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.Runtime.Credentials;
using Frends.AmazonCloudWatch.PutMetricData.Definitions;
using Frends.AmazonCloudWatch.PutMetricData.Enums;

namespace Frends.AmazonCloudWatch.PutMetricData.Helpers;

internal static partial class ClientFactory
{
    internal static IAmazonCloudWatch Create(Connection connection)
    {
        var region = GetRegion(connection.Region);
        var credentials = GetCredentials(connection);

        if (credentials == null)
            return region == null ? new AmazonCloudWatchClient() : new AmazonCloudWatchClient(region);

        return region == null ? new AmazonCloudWatchClient(credentials) : new AmazonCloudWatchClient(credentials, region);
    }

    internal static RegionEndpoint GetRegion(Regions region)
    {
        if (region == Regions.Undefined) return null;

        var systemName = RegionNameRegex()
            .Replace(region.ToString(), "-")
            .ToLowerInvariant();
        return RegionEndpoint.GetBySystemName(systemName);
    }

    [GeneratedRegex("(?<=[a-z])(?=[A-Z0-9])")]
    private static partial Regex RegionNameRegex();

    private static AWSCredentials GetCredentials(Connection connection) => connection.AuthenticationType switch
    {
        AuthenticationType.DefaultCredentialChain => null,
        AuthenticationType.AccessKey => GetKeyCredentials(connection),
        AuthenticationType.AssumeRole => GetAssumeRoleCredentials(connection),
        AuthenticationType.AwsProfile => GetProfileCredentials(connection),
        _ => throw new ArgumentOutOfRangeException(nameof(connection), connection.AuthenticationType, "Unsupported authentication type."),
    };

    private static AWSCredentials GetKeyCredentials(Connection connection) =>
        string.IsNullOrWhiteSpace(connection.SessionTokenSecret)
            ? new BasicAWSCredentials(connection.AccessKey, connection.SecretKeySecret)
            : new SessionAWSCredentials(connection.AccessKey, connection.SecretKeySecret, connection.SessionTokenSecret);

    private static AWSCredentials GetAssumeRoleCredentials(Connection connection)
    {
        var hasKey = !string.IsNullOrWhiteSpace(connection.AccessKey);
        var hasSecret = !string.IsNullOrWhiteSpace(connection.SecretKeySecret);
        if (hasKey != hasSecret)
            throw new ArgumentException("AccessKey and SecretKeySecret must be provided together.");

        var source = hasKey ? GetKeyCredentials(connection) : DefaultAWSCredentialsIdentityResolver.GetCredentials();

        return new AssumeRoleAWSCredentials(source, connection.RoleArn, connection.RoleSessionName, new AssumeRoleAWSCredentialsOptions
        {
            ExternalId = string.IsNullOrWhiteSpace(connection.ExternalId) ? null : connection.ExternalId,
            DurationSeconds = connection.DurationSeconds,
        });
    }

    private static AWSCredentials GetProfileCredentials(Connection connection)
    {
        var chain = string.IsNullOrWhiteSpace(connection.ProfilesLocation)
            ? new CredentialProfileStoreChain()
            : new CredentialProfileStoreChain(connection.ProfilesLocation);

        if (!chain.TryGetAWSCredentials(connection.ProfileName, out var credentials))
            throw new ArgumentException($"Could not load credentials for profile '{connection.ProfileName}'.");

        return credentials;
    }
}
