using System.ComponentModel;
using System.Reflection;
using FluentAssertions;
using Hangfire.MediatR.Tests.Common;
using MediatR;
using Microsoft.Extensions.Hosting;

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
        spyNotificationHandler = (GetService<INotificationHandler<DummyNotification>>() as SpyNotificationHandler)!;
        spyRequestHandler = (GetService<IRequestHandler<DummyRequest>>() as SpyRequestHandler)!;
    }

    [Fact]
    public async Task TestNotification()
    {
        mediatr.Enqueue(DummyNotification.CreateInstance());

        await WaitWhileHandlerProcessing();

        spyNotificationHandler.HaveHandledNotification.Should().BeTrue();
    }

    [Fact]
    public async Task TestNotification2()
    {
        var jobId = mediatr.Enqueue(DummyNotification.CreateInstance());

        await WaitWhileHandlerProcessing();

        JobShouldBeSucceed(jobId);
    }


    [Fact]
    public async Task TestNotification3()
    {
        mediatr.Enqueue("CouCou", DummyNotification.CreateInstance());

        await WaitWhileHandlerProcessing();

        spyNotificationHandler.HaveHandledNotification.Should().BeTrue();
    }

    [Fact]
    public void TestNotification4()
    {
        var jobName = "CouCou";
        var argumentPosition = 0;

        var jobId = mediatr.Enqueue(jobName, DummyNotification.CreateInstance());

        var jobData = JobStorage.Current.GetMonitoringApi().JobDetails(jobId);

        var attribute = jobData.Job.Method?.GetCustomAttribute<DisplayNameAttribute>();
        var firstArgument = jobData.Job.Args[argumentPosition];

        Assert.Equal($"{{{argumentPosition}}}", attribute?.DisplayName);
        Assert.Equal(jobName, firstArgument);
    }

    [Fact]
    public async Task TestRequest()
    {
        mediatr.Enqueue(DummyRequest.CreateInstance());

        await WaitWhileHandlerProcessing();

        spyRequestHandler.HaveHandledNotification.Should().BeTrue();
    }

    [Fact]
    public async Task TestRequest2()
    {
        var jobId = mediatr.Enqueue(DummyNotification.CreateInstance());

        await WaitWhileHandlerProcessing();

        JobShouldBeSucceed(jobId);
    }

    private static void JobShouldBeSucceed(string jobId)
    {
        var jobData = JobStorage.Current.GetMonitoringApi().JobDetails(jobId);
        jobData.History.Should().ContainSingle(h => h.StateName == "Succeeded");
    }

    private static async Task WaitWhileHandlerProcessing()
        => await Task.Delay(TimeSpan.FromSeconds(1));
}

public class DummyNotification : INotification
{
    private DummyNotification()
    {
    }

    public static DummyNotification CreateInstance()
        => new();
}

public class SpyNotificationHandler : INotificationHandler<DummyNotification>
{
    public SpyNotificationHandler()
        => HaveHandledNotification = false;

    public bool HaveHandledNotification { get; private set; }

    public Task Handle(DummyNotification notification, CancellationToken cancellationToken)
    {
        HaveHandledNotification = true;

        return Task.CompletedTask;
    }
}

public class DummyRequest : IRequest
{
    private DummyRequest()
    {
    }

    public static DummyRequest CreateInstance()
        => new();
}

public class SpyRequestHandler : IRequestHandler<DummyRequest>
{
    public SpyRequestHandler()
        => HaveHandledNotification = false;

    public bool HaveHandledNotification { get; private set; }


    public Task Handle(DummyRequest request, CancellationToken cancellationToken)
    {
        HaveHandledNotification = true;

        return Task.CompletedTask;
    }
}