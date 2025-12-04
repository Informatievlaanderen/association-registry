namespace AssociationRegistry.Test.Magda.GeefOndernemingService.Rechtsvorm;

using AssociationRegistry.Framework;
using AssociationRegistry.Integrations.Magda;
using AssociationRegistry.Integrations.Magda.Onderneming.GeefOnderneming;
using AssociationRegistry.Magda.Kbo;
using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging;
using FluentAssertions;
using Integrations.Magda.Onderneming;
using Integrations.Magda.Onderneming.Models;
using Integrations.Magda.Onderneming.Models.GeefOnderneming;
using Integrations.Magda.Shared.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ResultNet;
using Vereniging;
using Xunit;

public class Given_A_GeefOndernemingResponseBody_Which_Is_Not_A_Rechtspersoon
{
    private readonly MagdaGeefVerenigingService _service;
    private readonly Fixture _fixture;

    public Given_A_GeefOndernemingResponseBody_Which_Is_Not_A_Rechtspersoon()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        var magdaFacade = new Mock<IMagdaClient>();
        var envelope = _fixture.Create<ResponseEnvelope<GeefOndernemingResponseBody>>();

        envelope.Body!.GeefOndernemingResponse!.Repliek.Antwoorden.Antwoord.Inhoud.Onderneming.SoortOnderneming = new SoortOndernemingType
        {
            Code = new CodeSoortOndernemingType
            {
                Value = _fixture.Create<string>(),
            },
        };

        magdaFacade.Setup(facade => facade.GeefOnderneming(It.IsAny<string>(), AanroependeFunctie.RegistreerVerenigingMetRechtspersoonlijkheid,It.IsAny<CommandMetadata>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(envelope);

        _service = new MagdaGeefVerenigingService(magdaFacade.Object,
                                                  new NullLogger<MagdaGeefVerenigingService>());
    }

    [Fact]
    public async ValueTask Then_It_Returns_A_FailureResult()
    {
        var result = await _service.GeefVereniging(_fixture.Create<KboNummer>(), AanroependeFunctie.RegistreerVerenigingMetRechtspersoonlijkheid,_fixture.Create<CommandMetadata>(),
                                                   CancellationToken.None);

        result.IsFailure().Should().BeTrue();
    }
}
