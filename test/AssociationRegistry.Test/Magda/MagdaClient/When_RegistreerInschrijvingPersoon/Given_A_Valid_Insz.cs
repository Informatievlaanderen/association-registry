namespace AssociationRegistry.Test.Magda.MagdaClient.When_RegistreerInschrijvingPersoon;

using AssociationRegistry.Framework;
using Hosts.Configuration;
using AssociationRegistry.Integrations.Magda;
using AssociationRegistry.Integrations.Magda.Models;
using AssociationRegistry.Magda.Kbo;
using Common.Configuration;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;
using ResultaatEnumType = Integrations.Magda.Repertorium.RegistreerInschrijving0200.ResultaatEnumType;

public class Given_A_Valid_Insz
{
    private readonly Fixture _fixture = new();

    [Fact]
    public async Task Then_It_Returns_RegistreerInschrijvingResponseBody()
    {
        var insz = "01234567889";
        var magdaOptionsSection = ConfigurationHelper.GetConfiguration().GetMagdaOptionsSection("WiremockMagdaOptions");

        var magdaCallReferenceService = new Mock<IMagdaCallReferenceService>();
        var commandMetadata = _fixture.Create<CommandMetadata>();

        var aanroependeFunctie = AanroependeFunctie.RegistreerVerenigingMetRechtspersoonlijkheid;

        magdaCallReferenceService.Setup(x => x.CreateReference(commandMetadata.Initiator, commandMetadata.CorrelationId, insz,
                                                               ReferenceContext.RegistreerInschrijving0200(
                                                                   aanroependeFunctie),
                                                               It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(_fixture.Create<MagdaCallReference>());

        var client = new MagdaClient(magdaOptionsSection, magdaCallReferenceService.Object, new NullLogger<MagdaClient>());

        var response = await client.RegistreerInschrijvingPersoon(insz, aanroependeFunctie, commandMetadata, CancellationToken.None);

        using (new AssertionScope())
        {
            var repliek = response?.Body?.RegistreerInschrijvingResponse?.Repliek;
            repliek?.Uitzonderingen.Should().BeNullOrEmpty();

            var resultaat = repliek?.Antwoorden.Antwoord.Inhoud.Resultaat;

            resultaat.Should().NotBeNull();
            resultaat.Value.Should().BeOneOf(ResultaatEnumType.Item0, ResultaatEnumType.Item1);
        }
    }
}
