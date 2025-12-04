namespace AssociationRegistry.Test.Magda.MagdaClient.When_GeefPersoon;

using AssociationRegistry.Framework;
using AssociationRegistry.Hosts.Configuration;
using AssociationRegistry.Integrations.Magda;
using AssociationRegistry.Integrations.Magda.Models;
using AssociationRegistry.Magda.Kbo;
using AssociationRegistry.Test.Common.Configuration;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

public class Given_A_Technische_Fout
{
    private readonly Fixture _fixture = new();

    [Fact]
    public async Task Then_It_Returns_RegistreerInschrijvingResponseBody()
    {
        var insz = "0725459040"; // see wiremock folder

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

        var result = await client.GeefPersoon(insz,
                                                          aanroependeFunctie,
                                                          commandMetadata,
                                                          CancellationToken.None);

        result.Body.GeefPersoonResponse.Repliek.Antwoorden.Antwoord.Uitzonderingen.Should().NotBeEmpty();
    }
}
