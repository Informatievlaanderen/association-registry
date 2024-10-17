namespace AssociationRegistry.Test.Services.VerenigingenPerKboNummerService;

using AssociationRegistry.Magda.Constants;
using AssociationRegistry.Magda.Services;
using AssociationRegistry.Services;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using Vereniging;
using Xunit;

public class Given_KboNummersMetGeldigeRechtsvorm_MaarKboNummerNietGekend
{
    [Fact]
    public async Task Returns_VerenigingenPerKbo_With_NNB()
    {
        var fixture = new Fixture().CustomizeDomain();
        var rechtsvorm = RechtsvormCodes.IVZW;
        var kboNummer = fixture.Create<KboNummer>();

        var service = new VerenigingenPerKboNummerService(new RechtsvormCodeService());

        var actual = await service.GetKboNummerInfo([
            new KboNummerMetRechtsvorm(kboNummer, rechtsvorm),
        ], CancellationToken.None);

        actual.Should().BeEquivalentTo([
            new VerenigingenPerKbo(kboNummer, VerenigingenPerKboNummerService.VCodeUitzonderingen.NogNietBekend, false),
        ]);
    }
}
