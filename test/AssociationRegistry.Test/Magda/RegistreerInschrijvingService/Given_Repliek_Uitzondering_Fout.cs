namespace AssociationRegistry.Test.Magda.RegistreerInschrijvingService;

using AssociationRegistry.Framework;
using AssociationRegistry.Integrations.Magda;
using AssociationRegistry.Magda.Kbo;
using Integrations.Magda.Repertorium.RegistreerInschrijving0201;
using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging;
using FluentAssertions;
using Integrations.Magda.Onderneming;
using Integrations.Magda.Onderneming.Models.RegistreerInschrijving;
using Integrations.Magda.Shared.Exceptions;
using Integrations.Magda.Shared.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Vereniging;
using Xunit;

public class Given_Repliek_Uitzondering_Fout
{
    private readonly MagdaRegistreerInschrijvingService _service;
    private readonly Fixture _fixture;
    private readonly string _diagnose;

    public Given_Repliek_Uitzondering_Fout()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        var magdaClient = new Mock<IMagdaClient>();
        _diagnose = _fixture.Create<string>();
        var responseEnvelope = CreateResponseEnvelope(_diagnose);


        magdaClient.Setup(facade => facade.RegistreerInschrijvingOnderneming(It.IsAny<string>(), AanroependeFunctie.RegistreerVerenigingMetRechtspersoonlijkheid, It.IsAny<CommandMetadata>(),It.IsAny<CancellationToken>()))
                   .ReturnsAsync(responseEnvelope);

        _service = new MagdaRegistreerInschrijvingService(magdaClient.Object,
                                                          new NullLogger<MagdaRegistreerInschrijvingService>());
    }

    private ResponseEnvelope<RegistreerInschrijvingResponseBody> CreateResponseEnvelope(string diagnose)
    {
        var responseEnvelope = _fixture.Create<ResponseEnvelope<RegistreerInschrijvingResponseBody>>();

        responseEnvelope.Body!.RegistreerInschrijvingResponse!.Repliek.Uitzonderingen =
        [
            new UitzonderingType()
            {
                Type = UitzonderingTypeType.FOUT,
                Diagnose = diagnose,
            },
        ];

        return responseEnvelope;
    }

    [Fact]
    public async ValueTask Then_It_Throws_A_MagdaException()
    {
        var result = async () => await _service.RegistreerInschrijving(_fixture.Create<KboNummer>(), AanroependeFunctie.RegistreerVerenigingMetRechtspersoonlijkheid, _fixture.Create<CommandMetadata>(),
                                                           CancellationToken.None);

        var exception = await Assert.ThrowsAsync<MagdaException>(result);
        exception.InnerException!.Should().BeOfType<MagdaRepliekException>();
        exception.InnerException!.Message.Should().Be($"De volgende fouten hebben zich voorgedaan bij het aanroepen van de Magda RegistreerInschrijvingDienst.{Environment.NewLine}{_diagnose}{Environment.NewLine}");
    }
}
