namespace AssociationRegistry.Test.Admin.Api.Middleware.Given_A_CorelationIdMiddleware;

using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Infrastructure.CommandMiddleware;
using AssociationRegistry.Admin.Api.Infrastructure.WebApi.Middleware;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Xunit;

public class When_The_Middleware_Pushes_The_CorrelationId_To_The_LogContext
{
    [Fact]
    public async Task With_A_Valid_CorrelationId_Then_The_Client_Id_Is_In_The_LogContext()
    {
        var correlationId = Guid.NewGuid().ToString();
        var context = new DefaultHttpContext();
        context.Request.Path = "/v1/verenigingen/zoeken";
        context.Request.Headers[WellknownHeaderNames.CorrelationId] = correlationId;

        var captured = await InvokeAndCapture(context);

        captured.Should().Be(correlationId);
    }

    [Fact]
    public async Task On_A_Non_Versioned_Path_Then_It_Falls_Back_To_TraceIdentifier()
    {
        var context = new DefaultHttpContext();
        context.Request.Path = "/health";

        var captured = await InvokeAndCapture(context);

        captured.Should().Be(context.TraceIdentifier);
    }

    private static async Task<string?> InvokeAndCapture(DefaultHttpContext context)
    {
        var sink = new CaptureCorrelationIdSink();

        var logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .Enrich.FromLogContext()
            .WriteTo.Sink(sink)
            .CreateLogger();

        var middleware = new CorrelationIdMiddleware(next: _ =>
        {
            logger.Information("snapshot");
            return Task.CompletedTask;
        });

        await middleware.Invoke(context, helper: null!, new CorrelationIdProvider());

        return sink.CorrelationId;
    }

    private class CaptureCorrelationIdSink : ILogEventSink
    {
        public string? CorrelationId { get; private set; }

        public void Emit(LogEvent logEvent)
        {
            if (logEvent.Properties.TryGetValue("CorrelationId", out var value))
                CorrelationId = (value as ScalarValue)?.Value as string;
        }
    }
}
