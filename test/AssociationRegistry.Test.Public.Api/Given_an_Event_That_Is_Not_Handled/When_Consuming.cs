namespace AssociationRegistry.Test.Public.Api.Given_an_Event_That_Is_Not_Handled;

using AssociationRegistry.Public.ProjectionHost;
using AssociationRegistry.Public.ProjectionHost.Projections.Search;
using Fixtures;
using FluentAssertions;
using Marten;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Xunit.Categories;
using IEvent = AssociationRegistry.Events.IEvent;

public class Given_An_Unhandled_Event_Fixture : ProjectionHostFixture
{
    public const string VCode = "V0001001";

    public Given_An_Unhandled_Event_Fixture() : base(nameof(Given_An_Unhandled_Event_Fixture))
    {
    }

    public override async ValueTask InitializeAsync()
    {
        await AddEvent(VCode, new AnUnhandledEvent());
    }

    public record AnUnhandledEvent : IEvent;
}

[UnitTest]
public class Given_An_Unhandled_Event : IClassFixture<Given_An_Unhandled_Event_Fixture>
{
    private readonly IDocumentStore _documentStore;
    private readonly WebApplicationFactory<Program> _projectionHost;

    public Given_An_Unhandled_Event(Given_An_Unhandled_Event_Fixture fixture)
    {
        _projectionHost = fixture.ProjectionHost;
        _documentStore = fixture.DocumentStore;
    }

    [Fact]
    public async Task Then_No_exceptions_Are_Thrown()
    {
        var consumer = new MartenEventsConsumer(_projectionHost.Services.GetRequiredService<PubliekZoekProjectionHandler>(),
                                                Mock.Of<ILogger<MartenEventsConsumer>>());

        var consumeForElastic = () =>
        {
            using var documentSession = _documentStore.LightweightSession();

            return consumer.ConsumeAsync(
                new[]
                {
                    documentSession
                       .Events
                       .Append(Given_An_Unhandled_Event_Fixture.VCode, new Given_An_Unhandled_Event_Fixture.AnUnhandledEvent()),
                });
        };

        await consumeForElastic.Should().NotThrowAsync();
    }
}
