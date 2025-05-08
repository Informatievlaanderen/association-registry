namespace AssociationRegistry.Test.AcmBevraging.VerenigingenPerKboNummerService;

using AssociationRegistry.AcmBevraging;
using AssociationRegistry.Magda.Constants;
using AssociationRegistry.Magda.Services;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using FluentAssertions;
using Vereniging;
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

        await using var session = store.LightweightSession();
        var vCode = fixture.Create<VCode>().Value;
        session.Events.StartStream<KboNummer>(kboNummer, new { VCode = vCode });
        await session.SaveChangesAsync();

        var service = new VerenigingenPerKboNummerService(new RechtsvormCodeService(), store);

        var actual = await service.GetVerenigingenPerKbo([
            new KboNummerMetRechtsvorm(kboNummer, rechtsvorm),
        ], CancellationToken.None);

        actual.Should().BeEquivalentTo([
            new VerenigingenPerKbo(kboNummer, vCode, true),
        ]);
    }
}
