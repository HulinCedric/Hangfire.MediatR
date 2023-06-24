using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Hangfire.MediatR.Tests.Common;

public class BaseTest : IClassFixture<HostFixture>
{
    private readonly HostFixture fixture;

    protected BaseTest(HostFixture fixture)
        => this.fixture = fixture;

    protected T GetService<T>() where T : notnull
        => fixture.HostInstance.Services.GetRequiredService<T>();
}