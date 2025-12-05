namespace AssociationRegistry.Test.Magda.MagdaClient.When_GeefPersoon;

using AssociationRegistry.Framework;
using AssociationRegistry.Hosts.Configuration;
using AssociationRegistry.Integrations.Magda;
using AssociationRegistry.Magda.Kbo;
using AssociationRegistry.Test.Common.Configuration;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using Integrations.Magda.Onderneming;
using Integrations.Magda.Shared.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;
using ResultaatEnumType = Integrations.Magda.Repertorium.RegistreerInschrijving0200.ResultaatEnumType;

public class Given_A_Persoon
{
    private readonly Fixture _fixture = new();

    [Fact]
    public async Task Then_It_Returns_ResponsePersoonsBody()
    {
        var insz = "01234567889";
        var magdaOptionsSection = ConfigurationHelper.GetConfiguration().GetMagdaOptionsSection("WiremockMagdaOptions");

        var magdaCallReferenceService = new Mock<IMagdaCallReferenceService>();
        var commandMetadata = _fixture.Create<CommandMetadata>();

        var aanroependeFunctie = AanroependeFunctie.RegistreerVerenigingMetRechtspersoonlijkheid;

        magdaCallReferenceService.Setup(x => x.CreateReference(commandMetadata.Initiator, commandMetadata.CorrelationId, insz,
                                                               ReferenceContext.GeefPersoon0200(
                                                                   aanroependeFunctie),
                                                               It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(_fixture.Create<MagdaCallReference>());

        var client = new MagdaClient(magdaOptionsSection, magdaCallReferenceService.Object, new NullLogger<MagdaClient>());

        var response = await client.GeefPersoon(insz, aanroependeFunctie, commandMetadata, CancellationToken.None);

        using (new AssertionScope())
        {
            var repliek = response?.Body?.GeefPersoonResponse?.Repliek;
            repliek?.Uitzonderingen.Should().BeNullOrEmpty();

            var resultaat = repliek?.Antwoorden.Antwoord.Inhoud.Persoon;

            resultaat.Should().NotBeNull();
            resultaat.INSZ.Should().Be(insz);
            resultaat.Naam.Achternamen.First().Value.Should().Be("Achternaam");
            resultaat.Naam.Voornamen.First().Value.Should().Be("Voornaam");
            resultaat.Overlijden.Should().BeNull();
        }
    }
}
