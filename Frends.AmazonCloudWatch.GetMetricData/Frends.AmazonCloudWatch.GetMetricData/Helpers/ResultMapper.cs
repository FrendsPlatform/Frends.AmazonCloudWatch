using Amazon.CloudWatch.Model;
using Frends.AmazonCloudWatch.GetMetricData.Definitions;

namespace Frends.AmazonCloudWatch.GetMetricData.Helpers;

internal static class ResultMapper
{
    internal static Result Map(GetMetricDataResponse response) => new()
    {
        Success = true,
        MetricDataResults = (response.MetricDataResults ?? [])
            .Select(r => new Definitions.MetricDataResult
            {
                Id = r.Id,
                Label = r.Label,
                StatusCode = r.StatusCode?.Value,
                Timestamps = (r.Timestamps ?? []).Select(RequestBuilder.ToUtc).ToList(),
                Values = r.Values ?? [],
                Messages = ToMessages(r.Messages),
            })
            .ToList(),
        NextToken = string.IsNullOrEmpty(response.NextToken) ? null : response.NextToken,
        Messages = ToMessages(response.Messages),
        Error = null,
    };

    private static List<Message> ToMessages(List<MessageData> messages) =>
        (messages ?? []).Select(m => new Message { Code = m.Code, Value = m.Value }).ToList();
}
