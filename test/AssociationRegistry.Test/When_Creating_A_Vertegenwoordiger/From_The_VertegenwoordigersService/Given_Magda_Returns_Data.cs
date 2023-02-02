namespace AssociationRegistry.Test.When_Creating_A_Vertegenwoordiger.From_The_VertegenwoordigersService;

using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using Framework;
using INSZ;
using Magda;
using Moq;
using Vereniging.RegistreerVereniging;
using Vertegenwoordigers;
using Xunit;

public class Given_Magda_Returns_Data
{

    [Fact]
    public async Task Then_it_returns_a_vertegenwoordiger()
    {
        var fixture = new Fixture();
        var insz = Insz.Create(InszTestSet.Insz1);

        var magdaMock = new Mock<IMagdaFacade>();
        var magdaPersoon = new MagdaPersoon
        {
            Insz = insz,
            Voornaam = fixture.Create<string>(),
            Achternaam = fixture.Create<string>(),
            IsOverleden = false,
        };
        magdaMock.Setup(m => m.GetByInsz(insz, It.IsAny<CancellationToken>())).ReturnsAsync(
            magdaPersoon);

        var service = new VertegenwoordigerService(magdaMock.Object);

        var vertegenwoordiger = new RegistreerVerenigingCommand.Vertegenwoordiger(
            insz,
            fixture.Create<bool>(),
            fixture.Create<string>(),
            fixture.Create<string>(),
            fixture.CreateMany<RegistreerVerenigingCommand.ContactInfo>());

        var vertegenwoordigersLijst = await service.GetVertegenwoordigersLijst(new[] { vertegenwoordiger });

        using (new AssertionScope())
        {
            vertegenwoordigersLijst.Single().Insz.Should().Be(insz);
            vertegenwoordigersLijst.Single().Voornaam.Should().Be(magdaPersoon.Voornaam);
            vertegenwoordigersLijst.Single().Achternaam.Should().Be(magdaPersoon.Achternaam);
            vertegenwoordigersLijst.Single().Roepnaam.Should().Be(vertegenwoordiger.Roepnaam);
            vertegenwoordigersLijst.Single().Rol.Should().Be(vertegenwoordiger.Rol);
            vertegenwoordigersLijst.Single().PrimairContactpersoon.Should().Be(vertegenwoordiger.PrimairContactpersoon);
            vertegenwoordigersLijst.Single().ContactInfoLijst.Should().BeEquivalentTo(vertegenwoordiger.ContactInfoLijst);
        }
    }
}
