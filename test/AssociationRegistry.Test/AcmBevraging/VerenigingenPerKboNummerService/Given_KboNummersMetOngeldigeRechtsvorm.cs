namespace AssociationRegistry.Test.AcmBevraging.VerenigingenPerKboNummerService;

using AssociationRegistry.AcmBevraging;
using AssociationRegistry.Magda.Services;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using FluentAssertions;
using System.Threading;
using System.Threading.Tasks;
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

        var store = await TestDocumentStoreFactory.CreateAsync(nameof(Given_KboNummersMetOngeldigeRechtsvorm));

        var service = new VerenigingenPerKboNummerService(new RechtsvormCodeService(), store);

        var actual = await service.GetVerenigingenPerKbo([
            new KboNummerMetRechtsvorm(kboNummer, ongeldigeRechtsvorm),
        ], CancellationToken.None);

        actual.Should().BeEquivalentTo([
            new VerenigingenPerKbo(kboNummer, VerenigingenPerKbo.VCodeUitzonderingen.NietVanToepassing, false),
        ]);
    }
}
