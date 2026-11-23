using Frends.AmazonCloudWatch.GetMetricData.Definitions;
using Frends.AmazonCloudWatch.GetMetricData.Enums;
using NUnit.Framework;

namespace Frends.AmazonCloudWatch.GetMetricData.Tests;

[TestFixture]
internal class FunctionalTests : TestBase
{
    [Test]
    public async Task ShouldGetMetricDataWithFormQueries()
    {
        var result = await AmazonCloudWatch.GetMetricData(
            FormInput(),
            DefaultConnection(),
            DefaultOptions(),
            CancellationToken.None);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Error, Is.Null);
        Assert.That(result.NextToken, Is.Null);

        var series = result.MetricDataResults.Single();
        Assert.That(series.Id, Is.EqualTo("m1"));
        Assert.That(series.StatusCode, Is.EqualTo("Complete"));
        Assert.That(series.Timestamps, Is.EquivalentTo(SeededTimestamps));
        Assert.That(series.Timestamps.All(t => t.Kind == DateTimeKind.Utc), Is.True);
        Assert.That(series.Values, Is.EquivalentTo(SeededValues));
        Assert.That(series.Values, Has.Count.EqualTo(series.Timestamps.Count));
    }

    [Test]
    public async Task ShouldGetMetricDataWithJsonQueries()
    {
        var input = new Input
        {
            StartTime = SeedStart.AddMinutes(-5),
            EndTime = SeedStart.AddMinutes(30),
            QueryInputMode = QueryInputMode.Json,
            QueriesJson = $$"""
            [
              {
                "Id": "m1",
                "ReturnData": false,
                "MetricStat": {
                  "Metric": {
                    "Namespace": "{{TestNamespace}}",
                    "MetricName": "{{TestMetricName}}",
                    "Dimensions": [
                      {
                        "Name": "{{DimensionName}}",
                        "Value": "{{DimensionValue}}"
                      }
                    ]
                  },
                  "Period": 60,
                  "Stat": "Average",
                  "Unit": "Count"
                }
              },
              {
                "Id": "doubled",
                "Label": "Doubled",
                "Expression": "m1 * 2",
                "ReturnData": true
              }
            ]
            """,
        };

        var result = await AmazonCloudWatch.GetMetricData(
            input,
            DefaultConnection(),
            DefaultOptions(),
            CancellationToken.None);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Error, Is.Null);

        var series = result.MetricDataResults.Single();

        Assert.That(series.Id, Is.EqualTo("doubled"));
        Assert.That(series.Label, Is.EqualTo("Doubled"));
        Assert.That(series.StatusCode, Is.EqualTo("Complete"));
        Assert.That(series.Values, Is.EquivalentTo(SeededValues.Select(v => v * 2)));
        Assert.That(series.Timestamps, Is.EquivalentTo(SeededTimestamps));
    }

    [Test]
    public async Task ShouldReturnAwsErrorWhenJsonQueryHasNeitherMetricStatNorExpression()
    {
        var input = FormInput();
        input.QueryInputMode = QueryInputMode.Json;
        input.QueriesJson = """[{ "Id": "m1" }]""";
        var options = new Options { ThrowErrorOnFailure = false };

        var result = await AmazonCloudWatch.GetMetricData(input, DefaultConnection(), options, CancellationToken.None);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error.Message, Is.Not.Null);
        Assert.That(result.Error.Message.Contains("The parameter MetricDataQueries.member.1.MetricStat is required."), Is.True);
        Assert.That(result.MetricDataResults, Is.Empty);
    }

    [Test]
    public async Task ShouldEvaluateExpressionQuery()
    {
        var hidden = TestQuery();
        hidden.ReturnData = false;
        var input = FormInput(
            hidden,
            new MetricDataQuery { Id = "doubled", QueryType = QueryType.Expression, Expression = "m1 * 2", Label = "Doubled" });

        var result = await AmazonCloudWatch.GetMetricData(input, DefaultConnection(), DefaultOptions(), CancellationToken.None);

        Assert.That(result.Success, Is.True);
        var series = result.MetricDataResults.Single();
        Assert.That(series.Id, Is.EqualTo("doubled"));
        Assert.That(series.Label, Is.EqualTo("Doubled"));
        Assert.That(series.Values, Is.EquivalentTo(SeededValues.Select(v => v * 2)));
    }

    [Test]
    public async Task ShouldReturnNextTokenAndAllowFetchingFollowingPages()
    {
        var options = new Options { MaxDatapoints = 4, ScanBy = ScanBy.TimestampAscending };
        var pages = new List<Result>();
        var input = FormInput();

        do
        {
            var page = await AmazonCloudWatch.GetMetricData(input, DefaultConnection(), options, CancellationToken.None);
            Assert.That(page.Success, Is.True);
            pages.Add(page);
            input.NextToken = page.NextToken;
        }
        while (!string.IsNullOrEmpty(input.NextToken) && pages.Count < 10);

        Assert.That(pages.Count, Is.GreaterThan(1));
        Assert.That(pages[0].NextToken, Is.Not.Null.And.Not.Empty);
        Assert.That(pages.Sum(p => p.MetricDataResults.Single().Values.Count), Is.EqualTo(SeededPoints));
        Assert.That(pages[0].MetricDataResults.Single().StatusCode, Is.EqualTo("PartialData"));
        Assert.That(pages.Last().NextToken, Is.Null);

        var timestamps = pages.SelectMany(p => p.MetricDataResults.Single().Timestamps).ToList();
        var values = pages.SelectMany(p => p.MetricDataResults.Single().Values).ToList();
        Assert.That(timestamps, Is.EqualTo(SeededTimestamps));
        Assert.That(values, Is.EqualTo(SeededValues));
    }

    [Test]
    public async Task ShouldReturnNewestFirstWhenScanByDescending()
    {
        var options = new Options { ScanBy = ScanBy.TimestampDescending };

        var result = await AmazonCloudWatch.GetMetricData(FormInput(), DefaultConnection(), options, CancellationToken.None);

        var series = result.MetricDataResults.Single();
        Assert.That(series.Timestamps, Is.Ordered.Descending);
        Assert.That(series.Values, Is.EqualTo(SeededValues.Reverse()));
    }

    [Test]
    public async Task ShouldReturnNoValuesForNotMatchingDimension()
    {
        var query = TestQuery();
        query.Dimensions = [TestDimension("NoSuchEnvironment")];

        var result = await AmazonCloudWatch.GetMetricData(
            FormInput(query),
            DefaultConnection(),
            DefaultOptions(),
            CancellationToken.None);

        Assert.That(result.Success, Is.True);
        var series = result.MetricDataResults.Single();
        Assert.That(series.StatusCode, Is.EqualTo("Complete"));
        Assert.That(series.Values, Is.Empty);
        Assert.That(series.Timestamps, Is.Empty);
    }

    [Test]
    public async Task ShouldReturnSeparateSeriesForMultipleQueries()
    {
        var sum = TestQuery("sum");
        sum.Statistic = "Sum";
        var maximum = TestQuery("max");
        maximum.Statistic = "Maximum";

        var result = await AmazonCloudWatch.GetMetricData(
            FormInput(sum, maximum),
            DefaultConnection(),
            DefaultOptions(),
            CancellationToken.None);

        Assert.That(result.Success, Is.True);
        Assert.That(result.MetricDataResults.Select(r => r.Id), Is.EquivalentTo(new[] { "sum", "max" }));
        Assert.That(result.MetricDataResults.Single(r => r.Id == "max").Values, Is.EquivalentTo(SeededValues));
    }

    [Test]
    public async Task ShouldReturnAwsErrorForInvalidPeriodInJsonQuery()
    {
        var input = new Input
        {
            StartTime = SeedStart.AddMinutes(-5),
            EndTime = SeedStart.AddMinutes(30),
            QueryInputMode = QueryInputMode.Json,
            QueriesJson = $$"""
            [{
                "Id": "m1",
                "MetricStat": {
                    "Metric": {
                        "Namespace": "{{TestNamespace}}",
                        "MetricName": "{{TestMetricName}}"
                    },
                    "Period": 7,
                    "Stat": "Average"
                }
            }]
            """,
        };

        var options = new Options { ThrowErrorOnFailure = false };

        var result = await AmazonCloudWatch.GetMetricData(
            input, DefaultConnection(), options, CancellationToken.None);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error, Is.Not.Null);
        Assert.That(result.Error.Message, Is.Not.Empty);
        Assert.That(result.Error.Message.Contains("The parameter MetricDataQueries.member.1.MetricStat.Period must be a value in the set [ 1, 5, 10, 20, 30 ]"), Is.True);
    }

    [Test]
    public async Task ShouldReturnFailedResultWithInvalidCredentials()
    {
        var connection = DefaultConnection();
        connection.AccessKey = "AKIAINVALIDKEY000000";
        connection.SecretKeySecret = "invalidsecretkeyinvalidsecretkeyinvalid00";

        var options = DefaultOptions();
        options.ThrowErrorOnFailure = false;

        var result = await AmazonCloudWatch.GetMetricData(FormInput(), connection, options, CancellationToken.None);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error, Is.Not.Null);
        Assert.That(result.Error.Message.Contains("The security token included in the request is invalid."), Is.True);
    }

    [Test]
    public async Task ShouldGetMetricDataWithDefaultCredentialChain()
    {
        var connection = new Connection
        {
            AuthenticationType = AuthenticationType.DefaultCredentialChain,
            Region = Region.EuNorth1,
        };

        var result = await AmazonCloudWatch.GetMetricData(FormInput(), connection, DefaultOptions(), CancellationToken.None);

        Assert.That(result.Success, Is.True);
        var series = result.MetricDataResults.Single();
        Assert.That(series.Id, Is.EqualTo("m1"));
        Assert.That(series.StatusCode, Is.EqualTo("Complete"));
    }

    [Test]
    public async Task ShouldGetMetricDataWithAwsProfile()
    {
        using var profile = CreateTemporaryAwsProfile();

        var connection = new Connection
        {
            AuthenticationType = AuthenticationType.AwsProfile,
            Region = Region.EuNorth1,
            ProfileName = profile.ProfileName,
            ProfilesLocation = profile.ProfilesLocation,
        };

        var result = await AmazonCloudWatch.GetMetricData(
            FormInput(),
            connection,
            DefaultOptions(),
            CancellationToken.None);

        Assert.That(result.Success, Is.True);
        var series = result.MetricDataResults.Single();
        Assert.That(series.Id, Is.EqualTo("m1"));
        Assert.That(series.StatusCode, Is.EqualTo("Complete"));
    }

    [Test]
    public async Task ShouldFailWhenAwsProfileDoesNotExist()
    {
        var connection = new Connection
        {
            AuthenticationType = AuthenticationType.AwsProfile,
            Region = Region.EuNorth1,
            ProfileName = "profile-that-does-not-exist",
        };
        var options = DefaultOptions();
        options.ThrowErrorOnFailure = false;

        var result = await AmazonCloudWatch.GetMetricData(FormInput(), connection, options, CancellationToken.None);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error, Is.Not.Null);
        Assert.That(result.Error.Message, Does.Contain("profile-that-does-not-exist"));
    }
}
