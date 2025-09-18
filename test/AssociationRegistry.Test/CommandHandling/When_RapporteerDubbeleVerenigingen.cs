namespace AssociationRegistry.Test.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.DubbelDetectie;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Events.Factories;
using EventStore.ConflictResolution;
using FluentAssertions;
using MartenDb.Store;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

public class When_RegistreerGedetecteerdeDubbels
{
    private readonly Fixture _fixture;

    public When_RegistreerGedetecteerdeDubbels()
    {
        _fixture = new Fixture().CustomizeAdminApi();
    }

    [Fact]
    public async Task Then_It_Saved_A_New_Stream_Of_GedeteceerdeDubbels()
    {
        var documentStore = await TestDocumentStoreFactory.CreateAsync(nameof(When_RegistreerGedetecteerdeDubbels));

        documentStore.Storage.ApplyAllConfiguredChangesToDatabaseAsync().GetAwaiter().GetResult();

        var eventConflictResolver = new EventConflictResolver([new AddressMatchConflictResolutionStrategy(),], []);

        await using var session = documentStore.LightweightSession();
        var eventStore = new EventStore(documentStore, eventConflictResolver, NullLogger<EventStore>.Instance);
        var duplicateVerenigingsRepository = new DuplicateVerenigingsRepository(eventStore);

        var sut = new RapporteerDubbeleVerenigingenCommandHandler(duplicateVerenigingsRepository, session, NullLogger<RapporteerDubbeleVerenigingenCommandHandler>.Instance);

        var registreerDuplicateVerenigingenGedetecteerdCommand = _fixture.Create<RapporteerDubbeleVerenigingenCommand>();

        await sut.Handle(new CommandEnvelope<RapporteerDubbeleVerenigingenCommand>(
                             registreerDuplicateVerenigingenGedetecteerdCommand, _fixture.Create<CommandMetadata>()));await sut.Handle(new CommandEnvelope<RapporteerDubbeleVerenigingenCommand>(
                             registreerDuplicateVerenigingenGedetecteerdCommand, _fixture.Create<CommandMetadata>()));await sut.Handle(new CommandEnvelope<RapporteerDubbeleVerenigingenCommand>(
                             registreerDuplicateVerenigingenGedetecteerdCommand, _fixture.Create<CommandMetadata>()));await sut.Handle(new CommandEnvelope<RapporteerDubbeleVerenigingenCommand>(
                             registreerDuplicateVerenigingenGedetecteerdCommand, _fixture.Create<CommandMetadata>()));

        var events = await session.Events.FetchStreamAsync("DD0001");
        events.Should().NotBeNull();

        events.First().Data.Should().BeEquivalentTo(EventFactory.DubbeleVerenigingenWerdenGedetecteerd(
                                                   "DD0001", registreerDuplicateVerenigingenGedetecteerdCommand.Naam,
                                                   registreerDuplicateVerenigingenGedetecteerdCommand.Locaties,
                                                   registreerDuplicateVerenigingenGedetecteerdCommand.GedetecteerdeDubbels));
        await documentStore.DisposeAsync();
    }
}
