namespace AssociationRegistry.Test.Magda.MagdaClient.When_RegistreerInschrijvingPersoon;

using AssociationRegistry.Framework;
using Hosts.Configuration;
using AssociationRegistry.Integrations.Magda;
using AssociationRegistry.Magda.Kbo;
using Common.Configuration;
using AutoFixture;
using FluentAssertions;
using Integrations.Magda.Onderneming;
using Integrations.Magda.Shared.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

public class Given_A_Technische_Fout
{
    private readonly Fixture _fixture = new();

    [Fact]
    public async Task Then_It_Returns_RegistreerInschrijvingResponseBody()
    {
        var insz = "09876543210"; // see wiremock folder

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

        var result = await client.RegistreerInschrijvingPersoon(insz,
                                                          aanroependeFunctie,
                                                          commandMetadata,
                                                          CancellationToken.None);

        result.Body.RegistreerInschrijvingResponse.Repliek.Antwoorden.Antwoord.Uitzonderingen.Should().NotBeEmpty();
    }
}
