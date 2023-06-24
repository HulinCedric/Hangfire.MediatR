using MediatR;

namespace Hangfire.MediatR.Tests;

public record SpyNotification : INotification;

public class SpyNotificationHandler : INotificationHandler<SpyNotification>
{
    public SpyNotificationHandler()
        => HaveHandled = false;

    public bool HaveHandled { get; private set; }

    public Task Handle(SpyNotification notification, CancellationToken cancellationToken)
    {
        HaveHandled = true;

        return Task.CompletedTask;
    }
}