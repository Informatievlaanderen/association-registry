namespace AssociationRegistry.Test.When_Creating_A_Vertegenwoordiger.From_The_VertegenwoordigersService;

using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using Framework;
using Magda;
using Moq;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_Magda_Returns_Data
{
    [Fact]
    public async Task Then_it_returns_a_vertegenwoordiger()
    {
        var fixture = new Fixture().CustomizeAll();
        var magdaMock = new Mock<IMagdaFacade>();


        var service = new VertegenwoordigerService(magdaMock.Object);

        var vertegenwoordiger = fixture.Create<Vertegenwoordiger>();
        var magdaPersoon = new MagdaPersoon
        {
            Insz = vertegenwoordiger.Insz,
            Voornaam = fixture.Create<string>(),
            Achternaam = fixture.Create<string>(),
            IsOverleden = false,
        };
        magdaMock.Setup(m => m.GetByInsz(vertegenwoordiger.Insz, It.IsAny<CancellationToken>())).ReturnsAsync(
            magdaPersoon);

        var vertegenwoordigersLijst = await service.GetVertegenwoordigers(new[] { vertegenwoordiger });

        vertegenwoordigersLijst.Should().HaveCount(1);
        var result = vertegenwoordigersLijst.Single();

        using (new AssertionScope())
        {
            result.Insz.Should().Be(result.Insz);
            result.Voornaam.Should().Be(magdaPersoon.Voornaam);
            result.Achternaam.Should().Be(magdaPersoon.Achternaam);
            result.Roepnaam.Should().Be(result.Roepnaam);
            result.Rol.Should().Be(result.Rol);
            result.IsPrimair.Should().Be(result.IsPrimair);
            result.Email.Should().Be(result.Email);
            result.Telefoon.Should().Be(result.Telefoon);
            result.Mobiel.Should().Be(result.Mobiel);
            result.SocialMedia.Should().Be(result.SocialMedia);
        }
    }
}
