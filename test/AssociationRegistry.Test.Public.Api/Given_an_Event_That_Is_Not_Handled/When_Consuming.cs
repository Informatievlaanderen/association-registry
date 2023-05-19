namespace AssociationRegistry.Test.Public.Api.Given_an_Event_That_Is_Not_Handled;

using AssociationRegistry.Public.ProjectionHost.Projections.Search;
using Fixtures;
using FluentAssertions;
using Marten;
using Moq;
using Xunit;
using Xunit.Categories;
using IEvent = AssociationRegistry.Framework.IEvent;

public class Given_an_unhandled_event_fixture : ProjectionHostFixture
{
    public const string VCode = "V0001001";

    public Given_an_unhandled_event_fixture() : base(nameof(Given_an_unhandled_event_fixture))
    {
    }

    public override async Task InitializeAsync()
    {
        await AddEvent(VCode, new AnUnhandledEvent());
    }

    public record AnUnhandledEvent : IEvent;
}

[UnitTest]
public class Given_an_unhandled_event : IClassFixture<Given_an_unhandled_event_fixture>
{
    private readonly IDocumentStore _documentStore;

    public Given_an_unhandled_event(Given_an_unhandled_event_fixture fixture)
    {
        _documentStore = fixture.DocumentStore;
    }

    [Fact]
    public async Task Then_No_exceptions_Are_Thrown()
    {
        var consumer = new MartenEventsConsumer(Mock.Of<IElasticRepository>(), new ElasticEventProjection(new VerenigingBrolFeeder()));
        var consumeForElastic = () =>
        {
            using var documentSession = _documentStore
                .OpenSession();
            return consumer.ConsumeAsync(
                new[]
                {
                    documentSession
                        .Events
                        .Append(Given_an_unhandled_event_fixture.VCode, new Given_an_unhandled_event_fixture.AnUnhandledEvent()),
                });
        };
        await consumeForElastic.Should().NotThrowAsync();
    }
}
