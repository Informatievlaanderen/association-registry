namespace AssociationRegistry.Test.When_Checking_If_AggregateExists;

using AssociationRegistry.Events;
using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Vereniging;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NodaTime;
using Xunit;
using Xunit.Categories;

[IntegrationTest]
public class Given_An_Existing_Aggregate
{
    private readonly Fixture _fixture;
    private readonly EventConflictResolver _conflictResolver;

    public Given_An_Existing_Aggregate()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _conflictResolver = new EventConflictResolver(
            [
                new AddressMatchConflictResolutionStrategy(),
            ],
            [
                new AddressMatchConflictResolutionStrategy(),
            ]);
    }

    [Fact]
    public async Task Then_it_Throws_Exception()
    {
        var documentStore = await TestDocumentStoreFactory.Create(nameof(Given_An_Existing_Aggregate));

        await using var session = documentStore.LightweightSession();
        var eventStore = new EventStore(documentStore, _conflictResolver, NullLogger<EventStore>.Instance);
        var feitelijkeVerenigingWerdGeregistreerd = _fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();


        await eventStore.Save(feitelijkeVerenigingWerdGeregistreerd.VCode, session, new CommandMetadata(Initiator: "brol", Instant.MinValue, Guid.NewGuid()),
                              CancellationToken.None,
                              feitelijkeVerenigingWerdGeregistreerd);

        (await eventStore.Exists(VCode.Create(feitelijkeVerenigingWerdGeregistreerd.VCode))).Should().BeTrue();

        await documentStore.DisposeAsync();
    }
}
