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

public class Given_KboNummersMetGeldigeRechtsvorm_MaarKboNummerNietGekend
{
    [Fact]
    public async Task Returns_VerenigingenPerKbo_With_NNB()
    {
        var fixture = new Fixture().CustomizeDomain();
        var rechtsvorm = RechtsvormCodes.IVZW;
        var kboNummer = fixture.Create<KboNummer>();

        var store = await TestDocumentStoreFactory.CreateAsync(nameof(Given_KboNummersMetGeldigeRechtsvorm_MaarKboNummerNietGekend));

        var service = new VerenigingenPerKboNummerService(new RechtsvormCodeService(), store);

        var actual = await service.GetVerenigingenPerKbo([
            new KboNummerMetRechtsvorm(kboNummer, rechtsvorm),
        ], CancellationToken.None);

        actual.Should().BeEquivalentTo([
            new VerenigingenPerKbo(kboNummer, VerenigingenPerKbo.VCodeUitzonderingen.NogNietBekend, false),
        ]);
    }
}
