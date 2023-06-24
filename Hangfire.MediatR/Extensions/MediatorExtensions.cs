using MediatR;
using Hangfire;

namespace Hangfire.MediatR;

public static class MediatorExtensions
{
    public static void Enqueue(this IMediator mediator, string jobName, IRequest request)
    {
        var client = new BackgroundJobClient();
        client.Enqueue<MediatorHangfireBridge>(bridge => bridge.Send(jobName, request));
    }

    public static void Enqueue(this IMediator mediator, IRequest request)
    {
        var client = new BackgroundJobClient();
        client.Enqueue<MediatorHangfireBridge>(bridge => bridge.Send(request));
    }

    public static string Enqueue(
        this IMediator mediator,
        string jobName,
        INotification notification)
    {
        var operationId = System.Diagnostics.Activity.Current?.Id;
        var client = new BackgroundJobClient();
        var jobId = client.Enqueue<MediatorHangfireBridge>(
            bridge => bridge.Publish(jobName, notification, operationId));
        return jobId;
    }

    public static string Enqueue(this IMediator mediator, INotification notification)
    {
        var client = new BackgroundJobClient();
        return client.Enqueue<MediatorHangfireBridge>(bridge => bridge.Publish(notification));
    }
}