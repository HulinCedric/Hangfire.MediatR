using MediatR;

namespace Hangfire.MediatR;

public static class MediatorExtensions
{
    public static string Enqueue(this IMediator mediator, string jobName, IRequest request)
    {
        var client = new BackgroundJobClient();
        return client.Enqueue<MediatorHangfireBridge>(bridge => bridge.Send(jobName, request));
    }

    public static string Enqueue(this IMediator mediator, IRequest request)
    {
        var client = new BackgroundJobClient();
        return client.Enqueue<MediatorHangfireBridge>(bridge => bridge.Send(request));
    }

    public static string Enqueue(this IMediator mediator, string jobName, INotification notification)
    {
        var client = new BackgroundJobClient();
        return client.Enqueue<MediatorHangfireBridge>(bridge => bridge.Publish(jobName, notification));
    }

    public static string Enqueue(this IMediator mediator, INotification notification)
    {
        var client = new BackgroundJobClient();
        return client.Enqueue<MediatorHangfireBridge>(bridge => bridge.Publish(notification));
    }
}