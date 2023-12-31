﻿using MediatR;

namespace Hangfire.MediatR.Test.Fakes;

public record DummyNotification : INotification;

public class DummyNotificationHandler : INotificationHandler<DummyNotification>
{
    public Task Handle(DummyNotification notification, CancellationToken cancellationToken)
        => Task.CompletedTask;
}