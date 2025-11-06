namespace AssociationRegistry.Test.AcmBevraging.VerenigingenPerKboNummerService;

using Acm.Api.Queries.VerenigingenPerKbo;
using AssociationRegistry.Integrations.Magda.Services;
using Common.Framework;
using EventStore.ConflictResolution;
using FluentAssertions;
using MartenDb.Store;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Persoonsgegevens;
using Xunit;

public class Given_No_KboNummersMetRechtsvorm
{
    [Fact]
    public async ValueTask Returns_No_VerenigingenPerKbo()
    {
        var store = await TestDocumentStoreFactory.CreateAsync(nameof(Given_No_KboNummersMetRechtsvorm));
        var eventStore = new EventStore(store, new EventConflictResolver([], []), Mock.Of<IVertegenwoordigerPersoonsgegevensRepository>(), NullLogger<EventStore>.Instance);

        var service = new VerenigingenPerKboNummerService(new RechtsvormCodeService(), eventStore);
        var result = await service.GetVerenigingenPerKbo([], CancellationToken.None);

        result.Should().BeEmpty();
    }
}
