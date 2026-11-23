using System.ComponentModel;
using Amazon.CloudWatch;
using Frends.AmazonCloudWatch.GetMetricData.Definitions;
using Frends.AmazonCloudWatch.GetMetricData.Helpers;

namespace Frends.AmazonCloudWatch.GetMetricData;

/// <summary>
/// Task Class for AmazonCloudWatch operations.
/// </summary>
public static class AmazonCloudWatch
{
    /// <summary>
    /// Retrieves metric data from Amazon CloudWatch.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends-AmazonCloudWatch-GetMetricData)
    /// </summary>
    /// <param name="input">Essential parameters.</param>
    /// <param name="connection">Connection parameters.</param>
    /// <param name="options">Additional parameters.</param>
    /// <param name="cancellationToken">A cancellation token provided by Frends Platform.</param>
    /// <returns>object { bool Success, List&lt;object&gt; MetricDataResults { string Id, string Label, string StatusCode, List&lt;DateTime&gt; Timestamps, List&lt;double&gt; Values, List&lt;object&gt; Messages { string Code, string Value } }, string NextToken, List&lt;object&gt; Messages { string Code, string Value }, object Error { string Message, Exception AdditionalInfo } }</returns>
    public static async Task<Result> GetMetricData(
        [PropertyTab] Input input,
        [PropertyTab] Connection connection,
        [PropertyTab] Options options,
        CancellationToken cancellationToken)
    {
        return await Execute(input, connection, options, ClientFactory.Create, cancellationToken);
    }

    internal static async Task<Result> Execute(
        Input input,
        Connection connection,
        Options options,
        Func<Connection, IAmazonCloudWatch> clientFactory,
        CancellationToken cancellationToken)
    {
        try
        {
            ValidationHandler.Run(input, connection, options);

            var queries = QueryResolver.Resolve(input);

            cancellationToken.ThrowIfCancellationRequested();

            var request = RequestBuilder.Build(input, options, queries);
            using var client = clientFactory(connection);
            var response = await client.GetMetricDataAsync(request, cancellationToken);

            return ResultMapper.Map(response);
        }
        catch (Exception ex)
        {
            return ex.Handle(options);
        }
    }
}
