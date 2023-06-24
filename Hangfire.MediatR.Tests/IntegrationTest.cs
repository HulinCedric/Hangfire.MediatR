using FluentAssertions;
using Hangfire.MediatR.Tests.Common;
using Hangfire.MediatR.Tests.Fakes;
using Hangfire.MediatR.Tests.Verifications;
using MediatR;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Hangfire.MediatR.Tests;

public class IntegrationTest : BaseTest
{
    private readonly IMediator mediatr;
    private readonly SpyNotificationHandler spyNotificationHandler;
    private readonly SpyRequestHandler spyRequestHandler;

    public IntegrationTest(HostFixture fixture) : base(fixture)
    {
        fixture.HostInstance.Start();

        mediatr = GetService<IMediator>();
        spyNotificationHandler = (GetService<INotificationHandler<SpyNotification>>() as SpyNotificationHandler)!;
        spyRequestHandler = (GetService<IRequestHandler<SpyRequest>>() as SpyRequestHandler)!;
    }

    [Fact]
    public async Task TestNotification()
    {
        mediatr.Enqueue(new SpyNotification());

        await WaitWhileHandlerProcessing();

        spyNotificationHandler.HaveHandled.Should().BeTrue();
    }

    [Fact]
    public async Task TestNotification2()
    {
        var job = mediatr.Enqueue(new DummyNotification());

        await WaitWhileHandlerProcessing();

        job.ShouldBeSucceed();
    }

    [Fact]
    public void TestNamedNotification2()
    {
        const string jobName = nameof(TestNamedNotification2);

        var job = mediatr.Enqueue(jobName, new DummyNotification());

        job.ShouldHaveDisplayName(jobName);
    }

    [Fact]
    public async Task TestNamedNotification3()
    {
        const string jobName = nameof(TestNamedNotification3);

        var job = mediatr.Enqueue(jobName, new DummyNotification());

        await WaitWhileHandlerProcessing();

        job.ShouldBeSucceed();
    }

    [Fact]
    public async Task TestRequest()
    {
        mediatr.Enqueue(new SpyRequest());

        await WaitWhileHandlerProcessing();

        spyRequestHandler.HaveHandled.Should().BeTrue();
    }

    [Fact]
    public async Task TestRequest2()
    {
        var job = mediatr.Enqueue(new DummyRequest());

        await WaitWhileHandlerProcessing();

        job.ShouldBeSucceed();
    }

    [Fact]
    public async Task TestNamedRequest2()
    {
        const string jobName = nameof(TestNamedRequest2);

        var job = mediatr.Enqueue(jobName, new DummyRequest());

        await WaitWhileHandlerProcessing();

        job.ShouldBeSucceed();
    }

    [Fact]
    public void TestNamedRequest3()
    {
        const string jobName = nameof(TestNamedRequest3);

        var job = mediatr.Enqueue(jobName, new DummyRequest());

        job.ShouldHaveDisplayName(jobName);
    }

    private static async Task WaitWhileHandlerProcessing()
        => await Task.Delay(TimeSpan.FromMilliseconds(100));
}