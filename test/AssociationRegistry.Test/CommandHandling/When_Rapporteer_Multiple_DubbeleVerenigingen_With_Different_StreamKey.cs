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

public class When_Rapporteer_Multiple_DubbeleVerenigingen_With_Different_StreamKey
{
    private readonly Fixture _fixture;

    public When_Rapporteer_Multiple_DubbeleVerenigingen_With_Different_StreamKey()
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

        var firstRegistreerDuplicateVerenigingenGedetecteerdCommand = _fixture.Create<RapporteerDubbeleVerenigingenCommand>();
        var secondRegistreerDuplicateVerenigingenGedetecteerdCommand = _fixture.Create<RapporteerDubbeleVerenigingenCommand>();

        await sut.Handle(new CommandEnvelope<RapporteerDubbeleVerenigingenCommand>(
                             firstRegistreerDuplicateVerenigingenGedetecteerdCommand, _fixture.Create<CommandMetadata>()));

        await sut.Handle(new CommandEnvelope<RapporteerDubbeleVerenigingenCommand>(
                             secondRegistreerDuplicateVerenigingenGedetecteerdCommand, _fixture.Create<CommandMetadata>()));

        var eventsFromFirstStream = await session.Events.FetchStreamAsync(firstRegistreerDuplicateVerenigingenGedetecteerdCommand.Key);
        eventsFromFirstStream.Should().NotBeNull();
        eventsFromFirstStream.Should().HaveCount(1);

        eventsFromFirstStream[0].Data.Should().BeEquivalentTo(EventFactory.DubbeleVerenigingenWerdenGedetecteerd(
                                                                  firstRegistreerDuplicateVerenigingenGedetecteerdCommand.Key,
                                                                  firstRegistreerDuplicateVerenigingenGedetecteerdCommand.Naam,
                                                                  firstRegistreerDuplicateVerenigingenGedetecteerdCommand.Locaties,
                                                                  firstRegistreerDuplicateVerenigingenGedetecteerdCommand.GedetecteerdeDubbels));

        var eventsFromSecondStram = await session.Events.FetchStreamAsync(secondRegistreerDuplicateVerenigingenGedetecteerdCommand.Key);
        eventsFromSecondStram.Should().NotBeNull();
        eventsFromSecondStram.Should().HaveCount(1);
        eventsFromSecondStram[0].Data.Should().BeEquivalentTo(EventFactory.DubbeleVerenigingenWerdenGedetecteerd(
                                                                  secondRegistreerDuplicateVerenigingenGedetecteerdCommand.Key,
                                                                  secondRegistreerDuplicateVerenigingenGedetecteerdCommand.Naam,
                                                                  secondRegistreerDuplicateVerenigingenGedetecteerdCommand.Locaties,
                                                                  secondRegistreerDuplicateVerenigingenGedetecteerdCommand.GedetecteerdeDubbels));

        await documentStore.DisposeAsync();
    }
}
