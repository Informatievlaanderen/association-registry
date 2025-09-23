namespace AssociationRegistry.Test.Repositories;

using AssociationRegistry.Framework;
using AutoFixture;
using Common.Framework;
using DecentraalBeheer.Vereniging.Exceptions;
using EventStore.ConflictResolution;
using FluentAssertions;
using MartenDb.Store;
using Microsoft.Extensions.Logging.Abstractions;
using Resources;
using Xunit;

public class DubbelDetectieRepositoryTests
{
    [Fact]
    public async ValueTask With_Unknown_Aggregate_Id_Then_Throw_OngeldigBevestigingsToken()
    {
        var fixture = new Fixture();
        var documentStore = await TestDocumentStoreFactory.CreateAsync(nameof(DubbelDetectieRepositoryTests));
        var session = documentStore.LightweightSession();
        var eventStore = new EventStore(documentStore, new EventConflictResolver([], []), NullLogger<EventStore>.Instance);

        var sut = new DubbelDetectieRepository(eventStore);

        var exception = await Assert.ThrowsAsync<OngeldigBevestigingsToken>(() => sut.Save(fixture.Create<string>(), session, fixture.Create<CommandMetadata>(), CancellationToken.None, []));
        exception.Message.Should().Be(ExceptionMessages.OngeldigBevestigingsToken);
    }
}
