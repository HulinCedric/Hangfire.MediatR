using MediatR;

namespace Hangfire.MediatR.Tests;

public record DummyRequest : IRequest;

public class DummyRequestHandler : IRequestHandler<DummyRequest>
{
    public Task Handle(DummyRequest request, CancellationToken cancellationToken)
        => Task.CompletedTask;
}