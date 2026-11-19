using dotenv.net;
using Frends.AmazonCloudWatch.PutMetricData.Definitions;
using Frends.AmazonCloudWatch.PutMetricData.Enums;
using Frends.AmazonCloudWatch.PutMetricData.Tests.Helpers;

namespace Frends.AmazonCloudWatch.PutMetricData.Tests;

internal abstract class TestBase
{
    protected const string TestNamespace = "Frends/PutMetricDataTests";

    internal TestBase()
    {
        DotEnv.Load();
        AccessKey = GetEnvVar("AWS_ACCESS_KEY_ID");
        SecretKey = GetEnvVar("AWS_SECRET_ACCESS_KEY");
    }

    protected string AccessKey { get; }

    protected string SecretKey { get; }

    protected static Input DefaultInput() => new()
    {
        Namespace = TestNamespace,
        Metrics =
        [
            new MetricDatum { MetricName = "TestMetric", Value = 1, Unit = MetricUnit.Count },
        ],
    };

    protected static string GetEnvVar(string name) => Environment.GetEnvironmentVariable(name) ??
                                                    throw new InvalidOperationException(
                                                        $"Missing required env var: {name}");

    protected static Options DefaultOptions() => new();

    protected Connection DefaultConnection() => new()
    {
        AuthenticationType = AuthenticationType.AccessKey,
        Region = Regions.EuNorth1,
        AccessKey = AccessKey,
        SecretKeySecret = SecretKey,
    };

    protected TemporaryAwsProfile CreateTemporaryAwsProfile() =>
    new(AccessKey, SecretKey);
}
