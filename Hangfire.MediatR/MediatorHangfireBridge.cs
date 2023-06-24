using System.ComponentModel;
using System.Diagnostics;
using MediatR;

namespace Hangfire.MediatR;

public class MediatorHangfireBridge
{
    private readonly IMediator _mediator;

    public MediatorHangfireBridge(IMediator mediator)
        => _mediator = mediator;

    public async Task Send(IRequest command)
        => await _mediator.Send(command);

    public async Task Publish(INotification notification)
        => await _mediator.Publish(notification);

    [DisplayName("{0}")]
    public async Task Send(string jobName, IRequest command)
        => await _mediator.Send(command);

    [DisplayName("{0}")]
    public async Task Publish(string jobName, INotification notification, string? operationId = null)
    {
        Activity? activity = null;
        if (operationId is not null)
        {
            // Create and start a new Activity with the operation ID
            activity = new Activity(jobName)
                .SetParentId(operationId)
                .Start();
        }
       
        try
        {
            await _mediator.Publish(notification);
        }
        finally
        {
            // Stop the Activity
            activity?.Stop();
        }
    }
}