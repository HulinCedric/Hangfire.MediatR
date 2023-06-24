using Hangfire.MemoryStorage;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Hangfire.MediatR.Tests.Common;

public class HostFixture : IDisposable
{
    public HostFixture()
    {
        var args = Array.Empty<string>();
        HostInstance = Host.CreateDefaultBuilder(args)
            .ConfigureServices(
                (_, services)
                    =>
                {
                    services.AddHangfire(
                        configuration => configuration
                            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                            .UseMemoryStorage()

                            // Configure MediatR serialization settings.
                            .UseMediatR());

                    services.AddHangfireServer();

                    services.AddMediatR(
                        configuration => configuration.RegisterServicesFromAssembly(typeof(HostFixture).Assembly));

                    // Override service lifetime to singleton for testing purposes.
                    services.AddSingleton<INotificationHandler<SpyNotification>, SpyNotificationHandler>();
                    services.AddSingleton<IRequestHandler<SpyRequest>, SpyRequestHandler>();
                })
            .Build();
    }

    public IHost HostInstance { get; }

    public void Dispose()
        => HostInstance.Dispose();
}