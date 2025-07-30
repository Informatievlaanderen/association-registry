namespace AssociationRegistry.Test.Public.Api.Given_an_Event_That_Is_Not_Handled;

using AssociationRegistry.Public.ProjectionHost;
using AssociationRegistry.Public.ProjectionHost.Projections.Search;
using AutoFixture;
using Events;
using Fixtures;
using FluentAssertions;
using Hosts.Configuration.ConfigurationBindings;
using Marten;
using MartenDb;
using MartenDb.Subscriptions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Nest;
using Xunit;
using IEvent = AssociationRegistry.Events.IEvent;

public class Given_An_Unhandled_Event_Fixture : ProjectionHostFixture
{
    public const string VCode = "V0001001";

    public Given_An_Unhandled_Event_Fixture() : base(nameof(Given_An_Unhandled_Event_Fixture))
    {
    }

    public override async ValueTask InitializeAsync()
    {
    }

    public record AnUnhandledEvent : IEvent;
}

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
    public async ValueTask Then_No_exceptions_Are_Thrown()
    {
        var consumer = new PubliekZoekenEventsConsumer(_projectionHost.Services.GetRequiredService<IElasticClient>(),
                                                       new PubliekZoekProjectionHandler(),
                                                       _projectionHost.Services.GetRequiredService<ElasticSearchOptionsSection>(),
                                                       Mock.Of<ILogger<PubliekZoekenEventsConsumer>>());

        var consumeForElastic = () =>
        {
            using var documentSession = _documentStore.LightweightSession();

            var streamActions = documentSession
                               .Events
                               .Append(Given_An_Unhandled_Event_Fixture.VCode,
                                       new Given_An_Unhandled_Event_Fixture.AnUnhandledEvent());

            return consumer.ConsumeAsync(SubscriptionEventList.From(streamActions.Events.ToArray()));
        };

        await consumeForElastic.Should().NotThrowAsync();
    }
}
