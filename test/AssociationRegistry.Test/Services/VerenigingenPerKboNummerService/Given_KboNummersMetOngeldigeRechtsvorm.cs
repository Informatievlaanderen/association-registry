namespace AssociationRegistry.Test.Services.VerenigingenPerKboNummerService;

using AssociationRegistry.Magda.Constants;
using AssociationRegistry.Magda.Services;
using AssociationRegistry.Services;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using Vereniging;
using Xunit;

public class Given_KboNummersMetOngeldigeRechtsvorm
{
    [Fact]
    public async Task Returns_VerenigingenPerKbo_With_NVT()
    {
        var fixture = new Fixture().CustomizeDomain();
        var ongeldigeRechtsvorm = fixture.Create<string>();
        var kboNummer = fixture.Create<KboNummer>();

        var service = new VerenigingenPerKboNummerService(new RechtsvormCodeService());

        var actual = await service.GetKboNummerInfo([
            new KboNummerMetRechtsvorm(kboNummer, ongeldigeRechtsvorm),
        ], CancellationToken.None);

        actual.Should().BeEquivalentTo([
            new VerenigingenPerKbo(kboNummer, VerenigingenPerKboNummerService.VCodeUitzonderingen.NietVanToepassing, false),
        ]);
    }
}
