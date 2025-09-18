namespace AssociationRegistry.Test.AcmBevraging.VerenigingenPerKboNummerService;

using Acm.Api.Queries.VerenigingenPerKbo;
using AssociationRegistry.Integrations.Magda.Constants;
using AssociationRegistry.Integrations.Magda.Services;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using DecentraalBeheer.Vereniging;
using EventStore.ConflictResolution;
using FluentAssertions;
using MartenDb.Store;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

public class Given_KboNummersMetGeldigeRechtsvorm_MaarKboNummerNietGekend
{
    [Fact]
    public async ValueTask Returns_VerenigingenPerKbo_With_NNB()
    {
        var fixture = new Fixture().CustomizeDomain();
        var rechtsvorm = RechtsvormCodes.IVZW;
        var kboNummer = fixture.Create<KboNummer>();

        var store = await TestDocumentStoreFactory.CreateAsync(nameof(Given_KboNummersMetGeldigeRechtsvorm_MaarKboNummerNietGekend));
        var eventStore = new EventStore(store, new EventConflictResolver([], []), NullLogger<EventStore>.Instance);

        var service = new VerenigingenPerKboNummerService(new RechtsvormCodeService(), eventStore);

        var actual = await service.GetVerenigingenPerKbo([
            new KboNummerMetRechtsvorm(kboNummer, rechtsvorm),
        ], CancellationToken.None);

        actual.Should().BeEquivalentTo([
            new VerenigingenPerKbo(kboNummer, VerenigingenPerKbo.VCodeUitzonderingen.NogNietBekend, false),
        ]);
    }
}
