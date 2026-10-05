namespace AssociationRegistry.Test.Admin.Api.Middleware.Given_A_CorelationIdMiddleware;

using AssociationRegistry.Admin.Api.Infrastructure.CommandMiddleware;
using AssociationRegistry.Admin.Api.Infrastructure.WebApi.Middleware;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Xunit;

public class When_Resolving_The_CorrelationId_For_LogContext
{
    [Fact]
    public void With_A_Client_CorrelationId_Then_The_Client_Id_Wins()
    {
        var provider = new CorrelationIdProvider { CorrelationId = Guid.NewGuid().ToString() };
        var context = new DefaultHttpContext();

        var result = ClientCorrelationId.OrFallbackToTraceIdentifier(provider, context);

        result.Should().Be(provider.CorrelationId);
        result.Should().NotBe(context.TraceIdentifier);
    }

    [Fact]
    public void Without_A_Client_CorrelationId_Then_It_Falls_Back_To_TraceIdentifier()
    {
        var provider = new CorrelationIdProvider();
        var context = new DefaultHttpContext();

        var result = ClientCorrelationId.OrFallbackToTraceIdentifier(provider, context);

        result.Should().Be(context.TraceIdentifier);
    }
}
