using MediatR;

namespace Hangfire.MediatR.Test.Fakes;

public record SpyRequest : IRequest;

public class SpyRequestHandler : IRequestHandler<SpyRequest>
{
    public SpyRequestHandler()
        => HaveHandled = false;

    public bool HaveHandled { get; private set; }

    public Task Handle(SpyRequest request, CancellationToken cancellationToken)
    {
        HaveHandled = true;

        return Task.CompletedTask;
    }
}