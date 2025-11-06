namespace AssociationRegistry.Test.AcmBevraging.VerenigingenPerKboNummerService;

using Acm.Api.Queries.VerenigingenPerKbo;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using DecentraalBeheer.Vereniging;
using Events;
using EventStore.ConflictResolution;
using FluentAssertions;
using Integrations.Magda.Constants;
using Integrations.Magda.Services;
using MartenDb.Store;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Persoonsgegevens;
using Xunit;

public class Given_KboNummersMetGeldigeRechtsvorm_MaarKboNummerGekend
{
    [Fact]
    public async ValueTask Returns_VerenigingenPerKbo_With_VCode()
    {
        var fixture = new Fixture().CustomizeDomain();
        var rechtsvorm = RechtsvormCodes.IVZW;
        var kboNummer = fixture.Create<KboNummer>();

        var store = await TestDocumentStoreFactory.CreateAsync(nameof(Given_KboNummersMetGeldigeRechtsvorm_MaarKboNummerGekend));
        var eventStore = new EventStore(store.LightweightSession(), new EventConflictResolver([], []), Mock.Of<IVertegenwoordigerPersoonsgegevensRepository>() ,NullLogger<EventStore>.Instance);

        await using var session = store.LightweightSession();
        var vCode = fixture.Create<VCode>().Value;

        var verenigingMetRechtspersoonlijkheidWerdGeregistreerd = fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
        {
            VCode = vCode,
            KboNummer = kboNummer,
            Rechtsvorm = rechtsvorm,
        };
        session.Events.StartStream<KboNummer>(kboNummer, new { VCode = vCode });
        session.Events.Append(vCode, verenigingMetRechtspersoonlijkheidWerdGeregistreerd);
        await session.SaveChangesAsync();

        var service = new VerenigingenPerKboNummerService(new RechtsvormCodeService(), eventStore);

        var actual = await service.GetVerenigingenPerKbo([
            new KboNummerMetRechtsvorm(kboNummer, rechtsvorm),
        ], CancellationToken.None);

        actual.Should().BeEquivalentTo([
            new VerenigingenPerKbo(kboNummer, vCode, true),
        ]);
    }
}
