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

public class When_Rapporteer_Multiple_DubbeleVerenigingen_With_Same_StreamKey
{
    private readonly Fixture _fixture;

    public When_Rapporteer_Multiple_DubbeleVerenigingen_With_Same_StreamKey()
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
        var duplicateVerenigingsRepository = new DubbelDetectieVerenigingsRepository(eventStore);

        var sut = new RapporteerDubbeleVerenigingenCommandHandler(duplicateVerenigingsRepository, session,
                                                                  NullLogger<RapporteerDubbeleVerenigingenCommandHandler>.Instance);

        var registreerDuplicateVerenigingenGedetecteerdCommand = _fixture.Create<RapporteerDubbeleVerenigingenCommand>();

        await sut.Handle(new CommandEnvelope<RapporteerDubbeleVerenigingenCommand>(
                             registreerDuplicateVerenigingenGedetecteerdCommand, _fixture.Create<CommandMetadata>()));

        await sut.Handle(new CommandEnvelope<RapporteerDubbeleVerenigingenCommand>(
                             registreerDuplicateVerenigingenGedetecteerdCommand, _fixture.Create<CommandMetadata>()));

        var events = await session.Events.FetchStreamAsync(registreerDuplicateVerenigingenGedetecteerdCommand.Key);
        events.Should().NotBeNull();
        events.Should().HaveCount(2);

        events[0].Data.Should().BeEquivalentTo(EventFactory.DubbeleVerenigingenWerdenGedetecteerd(
                                                        registreerDuplicateVerenigingenGedetecteerdCommand.Key,
                                                        registreerDuplicateVerenigingenGedetecteerdCommand.Naam,
                                                        registreerDuplicateVerenigingenGedetecteerdCommand.Locaties,
                                                        registreerDuplicateVerenigingenGedetecteerdCommand.GedetecteerdeDubbels));
        events[0].Data.Should().BeEquivalentTo(EventFactory.DubbeleVerenigingenWerdenGedetecteerd(
                                                        registreerDuplicateVerenigingenGedetecteerdCommand.Key,
                                                        registreerDuplicateVerenigingenGedetecteerdCommand.Naam,
                                                        registreerDuplicateVerenigingenGedetecteerdCommand.Locaties,
                                                        registreerDuplicateVerenigingenGedetecteerdCommand.GedetecteerdeDubbels));

        await documentStore.DisposeAsync();
    }
}
