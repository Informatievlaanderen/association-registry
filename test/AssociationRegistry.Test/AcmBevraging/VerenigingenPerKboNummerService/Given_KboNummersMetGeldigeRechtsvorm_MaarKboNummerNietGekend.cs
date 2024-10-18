namespace AssociationRegistry.Test.Services.VerenigingenPerKboNummerService;

using AcmBevraging;
using AssociationRegistry.Magda.Constants;
using AssociationRegistry.Magda.Services;
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

        var actual = await service.GetVerenigingenPerKbo([
            new KboNummerMetRechtsvorm(kboNummer, rechtsvorm),
        ], CancellationToken.None);

        actual.Should().BeEquivalentTo([
            new VerenigingenPerKbo(kboNummer, VerenigingenPerKbo.VCodeUitzonderingen.NogNietBekend, false),
        ]);
    }
}
