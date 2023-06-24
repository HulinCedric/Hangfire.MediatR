using System.ComponentModel;
using System.Reflection;
using FluentAssertions;
using Hangfire.MediatR.Tests.Common;
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
        var jobId = mediatr.Enqueue(new DummyNotification());

        await WaitWhileHandlerProcessing();

        JobShouldBeSucceed(jobId);
    }

    [Fact]
    public void TestNamedNotification2()
    {
        const string jobName = nameof(TestNamedNotification2);

        var jobId = mediatr.Enqueue(jobName, new DummyNotification());

        JobShouldHaveDisplayName(jobId, jobName);
    }

    [Fact]
    public async Task TestNamedNotification3()
    {
        const string jobName = nameof(TestNamedNotification3);

        var jobId = mediatr.Enqueue(jobName, new DummyNotification());

        await WaitWhileHandlerProcessing();

        JobShouldBeSucceed(jobId);
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
        var jobId = mediatr.Enqueue(new DummyRequest());

        await WaitWhileHandlerProcessing();

        JobShouldBeSucceed(jobId);
    }

    [Fact]
    public async Task TestNamedRequest2()
    {
        const string jobName = nameof(TestNamedRequest2);

        var jobId = mediatr.Enqueue(jobName, new DummyRequest());

        await WaitWhileHandlerProcessing();

        JobShouldBeSucceed(jobId);
    }

    [Fact]
    public void TestNamedRequest3()
    {
        const string jobName = nameof(TestNamedRequest3);

        var jobId = mediatr.Enqueue(jobName, new DummyRequest());

        JobShouldHaveDisplayName(jobId, jobName);
    }

    private static void JobShouldHaveDisplayName(string jobId, string jobName)
    {
        const int argumentPosition = 0;

        var jobData = JobStorage.Current.GetMonitoringApi().JobDetails(jobId);

        var attribute = jobData.Job.Method?.GetCustomAttribute<DisplayNameAttribute>();
        attribute!.DisplayName.Should().Be($"{{{argumentPosition}}}");

        var firstArgument = jobData.Job.Args[argumentPosition];
        firstArgument.Should().Be(jobName);
    }

    private static void JobShouldBeSucceed(string jobId)
    {
        var jobData = JobStorage.Current.GetMonitoringApi().JobDetails(jobId);
        jobData.History.Should().ContainSingle(h => h.StateName == "Succeeded");
    }

    private static async Task WaitWhileHandlerProcessing()
        => await Task.Delay(TimeSpan.FromMilliseconds(100));
}