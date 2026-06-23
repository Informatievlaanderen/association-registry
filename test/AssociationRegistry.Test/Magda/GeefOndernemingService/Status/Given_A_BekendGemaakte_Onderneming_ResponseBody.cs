namespace AssociationRegistry.Test.Magda.GeefOndernemingService.Status;

using AssociationRegistry.Framework;
using AssociationRegistry.Magda.Kbo;
using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging;
using FluentAssertions;
using Integrations.Magda;
using Integrations.Magda.Onderneming;
using Integrations.Magda.Onderneming.GeefOnderneming;
using Integrations.Magda.Onderneming.Models.GeefOnderneming;
using Integrations.Magda.Shared.Constants;
using Integrations.Magda.Shared.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ResultNet;
using Xunit;

public class Given_A_BekendGemaakte_Onderneming_ResponseBody
{
    private readonly MagdaGeefVerenigingService _service;
    private readonly Fixture _fixture;

    public Given_A_BekendGemaakte_Onderneming_ResponseBody()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        var magdaFacade = new Mock<IMagdaClient>();
        var responseEnvelope = _fixture.Create<ResponseEnvelope<GeefOndernemingResponseBody>>();

        responseEnvelope.Body!.GeefOndernemingResponse!.Repliek.Antwoorden.Antwoord.Inhoud.Onderneming.StatusKBO = new StatusKBOType
        {
            Code = new CodeStatusKBOType
            {
                Value = StatusKBOCodes.BekendGemaakt,
            },
        };

        magdaFacade.Setup(facade => facade.GeefOnderneming(It.IsAny<string>(), AanroependeFunctie.RegistreerVerenigingMetRechtspersoonlijkheid,It.IsAny<CommandMetadata>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(responseEnvelope);
        _service = new MagdaGeefVerenigingService(magdaFacade.Object,
                                                  new NullLogger<MagdaGeefVerenigingService>());
    }

    [Fact]
    public async ValueTask Then_It_Returns_Onderneming_Is_Actief()
    {
        var result = await _service.GeefVereniging(_fixture.Create<KboNummer>(), AanroependeFunctie.RegistreerVerenigingMetRechtspersoonlijkheid,_fixture.Create<CommandMetadata>(),
                                                   CancellationToken.None) as Result<VerenigingVolgensKbo>;;

        result.IsSuccess().Should().BeTrue();
        result.Data.IsActief.Should().BeTrue();
    }
}
