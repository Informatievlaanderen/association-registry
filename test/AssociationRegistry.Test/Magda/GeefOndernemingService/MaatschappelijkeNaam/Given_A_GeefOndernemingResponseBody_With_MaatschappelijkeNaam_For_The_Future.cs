namespace AssociationRegistry.Test.Magda.GeefOndernemingService.MaatschappelijkeNaam;

using AssociationRegistry.Framework;
using AssociationRegistry.Integrations.Magda;
using AssociationRegistry.Integrations.Magda.Models;
using AssociationRegistry.Integrations.Magda.Models.GeefOnderneming;
using AssociationRegistry.Integrations.Magda.Onderneming.GeefOnderneming;
using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ResultNet;
using Vereniging;
using Xunit;

public class Given_A_GeefOndernemingResponseBody_With_MaatschappelijkeNaam_For_The_Future
{
    private readonly MagdaGeefVerenigingService _service;
    private readonly Fixture _fixture;

    public Given_A_GeefOndernemingResponseBody_With_MaatschappelijkeNaam_For_The_Future()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        var magdaFacade = new Mock<IMagdaClient>();
        var responseEnvelope = _fixture.Create<ResponseEnvelope<GeefOndernemingResponseBody>>();

        responseEnvelope.Body!.GeefOndernemingResponse!.Repliek.Antwoorden.Antwoord.Inhoud.Onderneming.Namen.MaatschappelijkeNamen = new[]
        {
            new NaamOndernemingType
            {
                Naam = _fixture.Create<string>(),
                Taalcode = "nl",
                DatumBegin = "2100-01-01",
                DatumEinde = "2200-01-01",
            },
        };

        magdaFacade.Setup(facade => facade.GeefOnderneming(It.IsAny<string>(), It.IsAny<MagdaCallReference>()))
                   .ReturnsAsync(responseEnvelope);

        _service = new MagdaGeefVerenigingService(Mock.Of<IMagdaCallReferenceRepository>(), magdaFacade.Object,
                                                  new NullLogger<MagdaGeefVerenigingService>());
    }

    [Fact]
    public async ValueTask Then_It_Returns_A_FailureResult()
    {
        var result = await _service.GeefVereniging(_fixture.Create<KboNummer>(), _fixture.Create<CommandMetadata>(),
                                                   CancellationToken.None);

        result.IsFailure().Should().BeTrue();
    }
}
