namespace Frends.AmazonCloudWatch.GetMetricData.Tests.Helpers;

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
            var options = new FileStreamOptions
            {
                Mode = FileMode.CreateNew,
                Access = FileAccess.Write,
                Share = FileShare.None,
            };

            if (!OperatingSystem.IsWindows())
            {
                options.UnixCreateMode =
                    UnixFileMode.UserRead | UnixFileMode.UserWrite;
            }

            using var stream = new FileStream(ProfilesLocation, options);
            using var writer = new StreamWriter(stream);

            foreach (var line in lines)
            {
                writer.WriteLine(line);
            }
        }
        catch
        {
            if (File.Exists(ProfilesLocation))
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
