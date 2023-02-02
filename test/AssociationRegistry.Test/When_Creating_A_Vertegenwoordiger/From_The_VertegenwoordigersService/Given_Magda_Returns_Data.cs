namespace AssociationRegistry.Test.When_Creating_A_Vertegenwoordiger.From_The_VertegenwoordigersService;

using ContactInfo;
using FluentAssertions;
using FluentAssertions.Execution;
using Framework;
using INSZ;
using Magda;
using Moq;
using Vertegenwoordigers;
using Xunit;

public class Given_Magda_Returns_Data
{

    [Fact]
    public async Task Then_it_returns_a_vertegenwoordiger()
    {
        var insz = Insz.Create(InszTestSet.Insz1);
        var contactLijst = ContactLijst.Create(new[] { ContactInfo.CreateInstance(null, "loki@trikster.be", "123456798", "www.mischief.loki", "#LocoLoki") });

        var magdaMock = new Mock<IMagdaFacade>();
        magdaMock.Setup(m => m.GetByInsz(insz, It.IsAny<CancellationToken>())).ReturnsAsync(
            new MagdaPersoon
            {
                Insz = insz,
                Voornaam = "Loki",
                Achternaam = "Odinson",
                IsOverleden = false,
            });

        var service = new VertegenwoordigerService(magdaMock.Object);

        var vertegenwoordiger = await service.CreateVertegenwoordiger(
            insz,
            false,
            "god of mischief",
            "Trikstergod",
            contactLijst);

        using (new AssertionScope())
        {
            vertegenwoordiger.Insz.Should().Be(insz);
            vertegenwoordiger.Voornaam.Should().Be("Loki");
            vertegenwoordiger.Achternaam.Should().Be("Odinson");
            vertegenwoordiger.Roepnaam.Should().Be("god of mischief");
            vertegenwoordiger.Rol.Should().Be("Trikstergod");
            vertegenwoordiger.PrimairContactpersoon.Should().BeFalse();
            vertegenwoordiger.ContactInfoLijst.Should().BeEquivalentTo(contactLijst);
        }
    }
}
