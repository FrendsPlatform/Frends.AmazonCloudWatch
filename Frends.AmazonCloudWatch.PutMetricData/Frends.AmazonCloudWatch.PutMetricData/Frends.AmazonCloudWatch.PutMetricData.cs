using System.ComponentModel;
using Amazon.CloudWatch;
using Frends.AmazonCloudWatch.PutMetricData.Definitions;
using Frends.AmazonCloudWatch.PutMetricData.Helpers;

namespace Frends.AmazonCloudWatch.PutMetricData;

/// <summary>
/// Task Class for AmazonCloudWatch operations.
/// </summary>
public static class AmazonCloudWatch
{
    /// <summary>
    /// Frends task to publish one or more custom metric data points to CloudWatch.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends-AmazonCloudWatch-PutMetricData)
    /// </summary>
    /// <param name="input">Essential parameters.</param>
    /// <param name="connection">Connection parameters.</param>
    /// <param name="options">Additional parameters.</param>
    /// <param name="cancellationToken">A cancellation token provided by Frends Platform.</param>
    /// <returns>object { bool Success, int MetricsSent, object Error { string Message, Exception AdditionalInfo } }</returns>
    public static async Task<Result> PutMetricData(
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

            cancellationToken.ThrowIfCancellationRequested();

            var request = RequestBuilder.Build(input);
            using var client = clientFactory(connection);
            await client.PutMetricDataAsync(request, cancellationToken);

            return new Result
            {
                Success = true,
                MetricsSent = request.MetricData.Count,
                Error = null,
            };
        }
        catch (Exception ex)
        {
            return ex.Handle(options);
        }
    }
}
