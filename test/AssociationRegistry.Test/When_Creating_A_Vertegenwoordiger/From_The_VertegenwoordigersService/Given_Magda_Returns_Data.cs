namespace AssociationRegistry.Test.When_Creating_A_Vertegenwoordiger.From_The_VertegenwoordigersService;

using AutoFixture;
using ContactGegevens;
using FluentAssertions;
using FluentAssertions.Execution;
using Framework;
using INSZ;
using Magda;
using Moq;
using Vereniging.RegistreerVereniging;
using Vertegenwoordigers;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_Magda_Returns_Data
{
    [Fact]
    public async Task Then_it_returns_a_vertegenwoordiger()
    {
        var fixture = new Fixture().CustomizeAll();
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

        const string email = "iemand@digitaal.vlaanderen";
        var vertegenwoordiger = new RegistreerVerenigingCommand.Vertegenwoordiger(
            insz,
            fixture.Create<bool>(),
            fixture.Create<string>(),
            fixture.Create<string>(),
            new RegistreerVerenigingCommand.Contactgegeven[]
            {
                new(ContactgegevenType.Email, email, fixture.Create<int?>().ToString(), false),
            });
        var vertegenwoordigersLijst = await service.GetVertegenwoordigersLijst(new[] { vertegenwoordiger });

        using (new AssertionScope())
        {
            vertegenwoordigersLijst.Single().Insz.Should().Be(insz);
            vertegenwoordigersLijst.Single().Voornaam.Should().Be(magdaPersoon.Voornaam);
            vertegenwoordigersLijst.Single().Achternaam.Should().Be(magdaPersoon.Achternaam);
            vertegenwoordigersLijst.Single().Roepnaam.Should().Be(vertegenwoordiger.Roepnaam);
            vertegenwoordigersLijst.Single().Rol.Should().Be(vertegenwoordiger.Rol);
            vertegenwoordigersLijst.Single().PrimairContactpersoon.Should().Be(vertegenwoordiger.PrimairContactpersoon);
            foreach (var contactgegeven in vertegenwoordigersLijst.Single().Contactgegevens)
            {
                contactgegeven.Type.Should().Be(vertegenwoordiger.Contactgegevens.Single().Type);
                contactgegeven.Waarde.Should().Be(vertegenwoordiger.Contactgegevens.Single().Waarde);
                contactgegeven.Omschrijving.Should().Be(vertegenwoordiger.Contactgegevens.Single().Omschrijving);
                contactgegeven.ContactgegevenId.Should().Be(1);
                contactgegeven.IsPrimair.Should().Be(vertegenwoordiger.Contactgegevens.Single().IsPrimair);
            }
        }
    }
}
