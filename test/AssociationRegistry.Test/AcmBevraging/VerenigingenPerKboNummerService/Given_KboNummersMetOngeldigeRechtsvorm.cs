namespace AssociationRegistry.Test.AcmBevraging.VerenigingenPerKboNummerService;

using Acm.Api.Queries.VerenigingenPerKbo;
using AssociationRegistry.Integrations.Magda.Services;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using DecentraalBeheer.Vereniging;
using EventStore.ConflictResolution;
using FluentAssertions;
using MartenDb.Store;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Persoonsgegevens;
using Vereniging;
using Xunit;

public class Given_KboNummersMetOngeldigeRechtsvorm
{
    [Fact]
    public async ValueTask Returns_VerenigingenPerKbo_With_NVT()
    {
        var fixture = new Fixture().CustomizeDomain();
        var ongeldigeRechtsvorm = fixture.Create<string>();
        var kboNummer = fixture.Create<KboNummer>();

        var store = await TestDocumentStoreFactory.CreateAsync(nameof(Given_KboNummersMetOngeldigeRechtsvorm));
        var eventStore = new EventStore(store.LightweightSession(), new EventConflictResolver([], []), Mock.Of<IVertegenwoordigerPersoonsgegevensRepository>(), NullLogger<EventStore>.Instance);

        var service = new VerenigingenPerKboNummerService(new RechtsvormCodeService(), eventStore);

        var actual = await service.GetVerenigingenPerKbo([
            new KboNummerMetRechtsvorm(kboNummer, ongeldigeRechtsvorm),
        ], CancellationToken.None);

        actual.Should().BeEquivalentTo([
            new VerenigingenPerKbo(kboNummer, VerenigingenPerKbo.VCodeUitzonderingen.NietVanToepassing, false),
        ]);
    }
}
