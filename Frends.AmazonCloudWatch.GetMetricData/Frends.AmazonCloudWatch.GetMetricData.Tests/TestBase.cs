using Amazon;
using Amazon.CloudWatch;
using Amazon.Runtime;
using dotenv.net;
using Frends.AmazonCloudWatch.GetMetricData.Definitions;
using Frends.AmazonCloudWatch.GetMetricData.Enums;
using Frends.AmazonCloudWatch.GetMetricData.Tests.Helpers;
using NUnit.Framework;
using Sdk = Amazon.CloudWatch.Model;

namespace Frends.AmazonCloudWatch.GetMetricData.Tests;

internal abstract class TestBase
{
    protected const string TestNamespace = "Frends/GetMetricDataTests";
    protected const string TestMetricName = "TestMetric";
    protected const string DimensionName = "Environment";
    protected const string DimensionValue = "Test";
    protected const int SeededPoints = 10;

    // Seeded data starts at a full hour two hours ago, so its timestamps are stable within an hour
    // and repeated publishing of the same values doesn't change the Average statistic.
    protected static readonly DateTime SeedStart = CalculateSeedStart();

    internal TestBase()
    {
        DotEnv.Load();
        AccessKey = GetEnvVar("AWS_ACCESS_KEY_ID");
        SecretKey = GetEnvVar("AWS_SECRET_ACCESS_KEY");
    }

    protected static DateTime[] SeededTimestamps =>
        Enumerable.Range(0, SeededPoints).Select(i => SeedStart.AddMinutes(i)).ToArray();

    protected static double[] SeededValues =>
        Enumerable.Range(1, SeededPoints).Select(i => (double)i).ToArray();

    protected string AccessKey { get; }

    protected string SecretKey { get; }

    [OneTimeSetUp]
    public async Task SeedMetricData()
    {
        using var client = new AmazonCloudWatchClient(new BasicAWSCredentials(AccessKey, SecretKey), RegionEndpoint.EUNorth1);

        await client.PutMetricDataAsync(new Sdk.PutMetricDataRequest
        {
            Namespace = TestNamespace,
            MetricData = SeededTimestamps
                .Select((timestamp, i) => new Sdk.MetricDatum
                {
                    MetricName = TestMetricName,
                    Value = i + 1,
                    Unit = StandardUnit.Count,
                    Timestamp = timestamp,
                    Dimensions = [new Sdk.Dimension { Name = DimensionName, Value = DimensionValue }],
                })
                .ToList(),
        });

        // New data points become queryable after a short delay.
        for (var attempt = 0; attempt < 24; attempt++)
        {
            var response = await client.GetMetricDataAsync(new Sdk.GetMetricDataRequest
            {
                StartTime = SeedStart.AddMinutes(-5),
                EndTime = SeedStart.AddMinutes(30),
                MetricDataQueries =
                [
                    new Sdk.MetricDataQuery
                    {
                        Id = "check",
                        MetricStat = new Sdk.MetricStat
                        {
                            Period = 60,
                            Stat = "Average",
                            Metric = new Sdk.Metric
                            {
                                Namespace = TestNamespace,
                                MetricName = TestMetricName,
                                Dimensions = [new Sdk.Dimension { Name = DimensionName, Value = DimensionValue }],
                            },
                        },
                    },
                ],
            });

            if (response.MetricDataResults.Single().Values.Count >= SeededPoints) return;
            await Task.Delay(TimeSpan.FromSeconds(10));
        }

        Assert.Fail("Seeded metric data didn't become available in CloudWatch in time.");
    }

    protected static Definitions.Dimension TestDimension(string value = DimensionValue) =>
        new() { Name = DimensionName, Value = value };

    protected static MetricDataQuery TestQuery(string id = "m1") => new()
    {
        Id = id,
        Namespace = TestNamespace,
        MetricName = TestMetricName,
        Dimensions = [TestDimension()],
        Period = 60,
        Statistic = "Average",
    };

    protected static Input FormInput(params MetricDataQuery[] queries) => new()
    {
        StartTime = SeedStart.AddMinutes(-5),
        EndTime = SeedStart.AddMinutes(30),
        QueryInputMode = QueryInputMode.Form,
        Queries = queries.Length > 0 ? queries : [TestQuery()],
    };

    protected static Options DefaultOptions() => new();

    protected Connection DefaultConnection() => new()
    {
        AuthenticationType = AuthenticationType.AccessKey,
        Region = Region.EuNorth1,
        AccessKey = AccessKey,
        SecretKeySecret = SecretKey,
    };

    protected TemporaryAwsProfile CreateTemporaryAwsProfile() =>
    new(AccessKey, SecretKey);

    private static DateTime CalculateSeedStart()
    {
        var now = DateTime.UtcNow;
        return new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0, DateTimeKind.Utc).AddHours(-2);
    }

    private static string GetEnvVar(string name) => Environment.GetEnvironmentVariable(name) ??
                                                    throw new InvalidOperationException(
                                                        $"Missing required env var: {name}");
}
