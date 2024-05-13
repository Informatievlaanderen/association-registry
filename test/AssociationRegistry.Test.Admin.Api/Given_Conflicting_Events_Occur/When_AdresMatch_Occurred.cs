namespace AssociationRegistry.Test.Admin.Api.Given_Conflicting_Events_Occur;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using EventStore;
using Framework;
using Marten;
using Marten.Events;
using NodaTime;
using Xunit;

public class When_AdresMatch_Occurred
{
    private readonly Fixture _fixture;

    public When_AdresMatch_Occurred()
    {
        _fixture = new Fixture().CustomizeAdminApi();
    }

    [Fact]
    public async Task ThenItSavesTheLocation()
    {
        using var documentStore = DocumentStore.For(options =>
        {
            options.Connection(GetConnectionString());
            options.Events.StreamIdentity = StreamIdentity.AsString;
        });

        var eventConflictResolver = new EventConflictResolver(new IEventConflictResolutionStrategy[]
        {
            new AddressMatchConflictResolutionStrategy()
        });

        await using var session = documentStore.LightweightSession();
        var eventStore = new EventStore(documentStore, eventConflictResolver);
        var feitelijkeVerenigingWerdGeregistreerd = _fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();
        var adresWerdOvergenomenUitAdressenregister = _fixture.Create<AdresWerdOvergenomenUitAdressenregister>();
        var locatieWerdToegevoegd = _fixture.Create<LocatieWerdToegevoegd>();

        var vCode = _fixture.Create<string>();

        await eventStore.Save(vCode, session, new CommandMetadata("brol", Instant.MinValue, Guid.NewGuid(), null), CancellationToken.None,
                              feitelijkeVerenigingWerdGeregistreerd, adresWerdOvergenomenUitAdressenregister);

        await eventStore.Save(vCode, session, new CommandMetadata("brol", Instant.MinValue, Guid.NewGuid(), 1), CancellationToken.None,
                              locatieWerdToegevoegd);

        var savedEvents = await session.Events.FetchStreamAsync(vCode);
        Assert.Equal(3, savedEvents.Count);
    }

    private static string GetConnectionString()
        => $"host=127.0.0.1;" +
           $"database=verenigingsregister;" +
           $"password=root;" +
           $"username=root";
}
