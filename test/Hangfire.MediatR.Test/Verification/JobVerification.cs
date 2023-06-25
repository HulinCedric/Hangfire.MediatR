using System.ComponentModel;
using System.Reflection;
using FluentAssertions;

namespace Hangfire.MediatR.Test.Verification;

public static class JobVerification
{
    public static void ShouldHaveDisplayName(this string jobId, string jobName)
    {
        const int argumentPosition = 0;

        var jobData = JobStorage.Current.GetMonitoringApi().JobDetails(jobId);

        var displayNameAttribute = jobData.Job.Method?.GetCustomAttribute<DisplayNameAttribute>()!;
        displayNameAttribute.DisplayName.Should().Be($"{{{argumentPosition}}}");

        var relatedJobArgument = jobData.Job.Args[argumentPosition];
        relatedJobArgument.Should().Be(jobName);
    }

    public static void ShouldBeSucceed(this string jobId)
    {
        var jobDetails = JobStorage.Current.GetMonitoringApi().JobDetails(jobId);
        jobDetails.History.Should().ContainSingle(h => h.StateName == "Succeeded");
    }
}