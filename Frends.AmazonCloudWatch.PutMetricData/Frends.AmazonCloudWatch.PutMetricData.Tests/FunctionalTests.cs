using Frends.AmazonCloudWatch.PutMetricData.Definitions;
using Frends.AmazonCloudWatch.PutMetricData.Enums;
using NUnit.Framework;

namespace Frends.AmazonCloudWatch.PutMetricData.Tests;

[TestFixture]
internal class FunctionalTests : TestBase
{
    [Test]
    public async Task ShouldPutSingleMetric()
    {
        var result = await AmazonCloudWatch.PutMetricData(
            DefaultInput(),
            DefaultConnection(),
            DefaultOptions(),
            CancellationToken.None);

        Assert.That(result.Success, Is.True);
        Assert.That(result.MetricsSent, Is.EqualTo(1));
        Assert.That(result.Error, Is.Null);
    }

    [Test]
    public async Task ShouldPutMultipleMetricsWithDimensionsAndTimestamp()
    {
        var input = new Input
        {
            Namespace = TestNamespace,
            Metrics =
            [
                new MetricDatum
                {
                    MetricName = "Latency",
                    Value = 12.5,
                    Unit = MetricUnit.Milliseconds,
                    Timestamp = DateTime.UtcNow.AddMinutes(-5),
                    Dimensions = [new Dimension { Name = "Environment", Value = "Test" }],
                },
                new MetricDatum
                {
                    MetricName = "Throughput",
                    Value = 100,
                    Unit = MetricUnit.CountPerSecond,
                    StorageResolution = StorageResolution.High,
                    Dimensions =
                    [
                        new Dimension { Name = "Environment", Value = "Test" },
                        new Dimension { Name = "Service", Value = "Orders" },
                    ],
                },
                new MetricDatum { MetricName = "NoUnit", Value = -3 },
            ],
        };

        var result = await AmazonCloudWatch.PutMetricData(
            input,
            DefaultConnection(),
            DefaultOptions(),
            CancellationToken.None);

        Assert.That(result.Success, Is.True);
        Assert.That(result.MetricsSent, Is.EqualTo(3));
        Assert.That(result.Error, Is.Null);
    }

    [Test]
    public async Task ShouldPutMetricWithLocalTimestamp()
    {
        var input = DefaultInput();
        input.Metrics[0].Timestamp = DateTime.Now.AddMinutes(-1);

        var result = await AmazonCloudWatch.PutMetricData(
            input,
            DefaultConnection(),
            DefaultOptions(),
            CancellationToken.None);

        Assert.That(result.Success, Is.True);
        Assert.That(result.MetricsSent, Is.EqualTo(1));
    }

    [Test]
    public async Task ShouldReturnFailedResultWhenAwsRejectsReservedNamespace()
    {
        var input = DefaultInput();
        input.Namespace = "AWS/FrendsTest";

        var options = DefaultOptions();
        options.ThrowErrorOnFailure = false;

        var result = await AmazonCloudWatch.PutMetricData(input, DefaultConnection(), options, CancellationToken.None);

        Assert.That(result.Success, Is.False);
        Assert.That(result.MetricsSent, Is.EqualTo(0));
        Assert.That(result.Error, Is.Not.Null);
        Assert.That(result.Error.Message, Is.Not.Empty);
        Assert.That(result.Error.AdditionalInfo, Is.Not.Null);
    }

    [Test]
    public async Task ShouldReturnFailedResultWithInvalidCredentials()
    {
        var connection = DefaultConnection();
        connection.AccessKey = "AKIAINVALIDKEY000000";
        connection.SecretKeySecret = "invalidsecretkeyinvalidsecretkeyinvalid00";

        var options = DefaultOptions();
        options.ThrowErrorOnFailure = false;

        var result = await AmazonCloudWatch.PutMetricData(DefaultInput(), connection, options, CancellationToken.None);

        Assert.That(result.Success, Is.False);
        Assert.That(result.MetricsSent, Is.EqualTo(0));
        Assert.That(result.Error, Is.Not.Null);
        Assert.That(result.Error.Message, Is.Not.Empty);
    }

    [Test]
    public async Task ShouldPutMetricWithDefaultCredentialChain()
    {
        var connection = new Connection
        {
            AuthenticationType = AuthenticationType.DefaultCredentialChain,
            Region = Regions.EuNorth1,
        };

        var result = await AmazonCloudWatch.PutMetricData(DefaultInput(), connection, DefaultOptions(), CancellationToken.None);

        Assert.That(result.Success, Is.True);
        Assert.That(result.MetricsSent, Is.EqualTo(1));
    }

    [Test]
    public async Task ShouldPutMetricWithAwsProfile()
    {
        using var profile = CreateTemporaryAwsProfile();

        var connection = new Connection
        {
            AuthenticationType = AuthenticationType.AwsProfile,
            Region = Regions.EuNorth1,
            ProfileName = profile.ProfileName,
            ProfilesLocation = profile.ProfilesLocation,
        };

        var result = await AmazonCloudWatch.PutMetricData(
            DefaultInput(),
            connection,
            DefaultOptions(),
            CancellationToken.None);

        Assert.That(result.Success, Is.True);
        Assert.That(result.MetricsSent, Is.EqualTo(1));
        Assert.That(result.Error, Is.Null);
    }

    [Test]
    public async Task ShouldFailWhenAwsProfileDoesNotExist()
    {
        var connection = new Connection
        {
            AuthenticationType = AuthenticationType.AwsProfile,
            Region = Regions.EuNorth1,
            ProfileName = "profile-that-does-not-exist",
        };
        var options = DefaultOptions();
        options.ThrowErrorOnFailure = false;

        var result = await AmazonCloudWatch.PutMetricData(DefaultInput(), connection, options, CancellationToken.None);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error.Message, Does.Contain("profile-that-does-not-exist"));
    }
}
