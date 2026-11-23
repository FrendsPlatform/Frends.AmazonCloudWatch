using Frends.AmazonCloudWatch.GetMetricData.Definitions;
using Frends.AmazonCloudWatch.GetMetricData.Enums;
using Newtonsoft.Json;
using SdkQuery = Amazon.CloudWatch.Model.MetricDataQuery;

namespace Frends.AmazonCloudWatch.GetMetricData.Helpers;

/// <summary>
/// Resolves metric queries from the form fields or from the AWS-style JSON.
/// </summary>
internal static class QueryResolver
{
    internal static List<SdkQuery> Resolve(Input input) =>
        input.QueryInputMode == QueryInputMode.Json
            ? ParseJson(input.QueriesJson)
            : (input.Queries ?? []).Select(RequestBuilder.ToQuery).ToList();

    // The JSON uses the AWS MetricDataQueries format, so it maps directly to the SDK type. AWS validates the content.
    internal static List<SdkQuery> ParseJson(string json)
    {
        try
        {
            return JsonConvert.DeserializeObject<List<SdkQuery>>(json)
                ?? throw new ArgumentException("QueriesJson must be a JSON array of queries.");
        }
        catch (JsonException ex)
        {
            throw new ArgumentException($"QueriesJson is not valid JSON: {ex.Message}", ex);
        }
    }
}
