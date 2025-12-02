namespace AssociationRegistry.Test.Magda.RegistreerInschrijvingService;

using AssociationRegistry.Framework;
using AssociationRegistry.Integrations.Magda;
using AssociationRegistry.Integrations.Magda.Models;
using AssociationRegistry.Integrations.Magda.Models.RegistreerInschrijving;
using AssociationRegistry.Magda.Kbo;
using Integrations.Magda.Repertorium.RegistreerInschrijving0201;
using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ResultNet;
using Vereniging;
using Xunit;
using AntwoordInhoudType = Integrations.Magda.Repertorium.RegistreerInschrijving0201.AntwoordInhoudType;

public class Given_Geslaagd
{
    private readonly MagdaRegistreerInschrijvingService _service;
    private readonly Fixture _fixture;

    public Given_Geslaagd()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        var magdaClient = new Mock<IMagdaClient>();
        var responseEnvelope = CreateResponseEnvelope();

        magdaClient.Setup(facade => facade.RegistreerInschrijvingOnderneming(It.IsAny<string>(), AanroependeFunctie.RegistreerVerenigingMetRechtspersoonlijkheid, It.IsAny<CommandMetadata>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(responseEnvelope);

        _service = new MagdaRegistreerInschrijvingService(magdaClient.Object,
                                                          new NullLogger<MagdaRegistreerInschrijvingService>());
    }

    private ResponseEnvelope<RegistreerInschrijvingResponseBody> CreateResponseEnvelope()
    {
        var responseEnvelope = _fixture.Create<ResponseEnvelope<RegistreerInschrijvingResponseBody>>();

        responseEnvelope.Body!.RegistreerInschrijvingResponse!.Repliek.Antwoorden.Antwoord.Inhoud = new AntwoordInhoudType
        {
            Resultaat = new ResultaatCodeType
            {
                Value = ResultaatEnumType.Item1,
                Beschrijving = "Wel geslaagd",
            },
        };

        return responseEnvelope;
    }

    [Fact]
    public async ValueTask Then_It_Returns_A_SuccessResult()
    {
        var result = await _service.RegistreerInschrijving(_fixture.Create<KboNummer>(), AanroependeFunctie.RegistreerVerenigingMetRechtspersoonlijkheid, _fixture.Create<CommandMetadata>(),
                                                           CancellationToken.None);

        result.IsSuccess().Should().BeTrue();
    }
}
