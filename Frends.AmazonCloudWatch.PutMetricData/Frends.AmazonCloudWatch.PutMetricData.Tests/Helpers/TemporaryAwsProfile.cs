namespace Frends.AmazonCloudWatch.PutMetricData.Tests.Helpers;

internal sealed class TemporaryAwsProfile : IDisposable
{
    public TemporaryAwsProfile(
        string accessKey,
        string secretKey)
    {
        ProfilesLocation = Path.Combine(
            Path.GetTempPath(),
            $"aws-credentials-{Guid.NewGuid():N}");

        var lines = new List<string>
        {
            $"[{ProfileName}]",
            $"aws_access_key_id={accessKey}",
            $"aws_secret_access_key={secretKey}",
        };

        try
        {
            File.WriteAllLines(ProfilesLocation, lines);
        }
        catch
        {
            File.Delete(ProfilesLocation);
            throw;
        }
    }

    public string ProfileName { get; } =
    $"frends-test-{Guid.NewGuid():N}";

    public string ProfilesLocation { get; }

    public void Dispose()
    {
        File.Delete(ProfilesLocation);
    }
}
