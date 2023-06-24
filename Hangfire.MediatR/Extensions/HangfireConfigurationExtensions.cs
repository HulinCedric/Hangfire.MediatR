using Newtonsoft.Json;

namespace Hangfire.MediatR;

public static class HangfireConfigurationExtensions
{
    public static IGlobalConfiguration UseMediatR(this IGlobalConfiguration configuration)
    {
        configuration.UseRecommendedSerializerSettings(
            jsonSettings =>
                jsonSettings.TypeNameHandling = TypeNameHandling.All);

        return configuration;
    }
}