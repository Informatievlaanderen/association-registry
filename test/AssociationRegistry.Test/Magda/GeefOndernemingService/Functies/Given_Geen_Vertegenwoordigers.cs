namespace AssociationRegistry.Test.Magda.GeefOndernemingService.Functies;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using AssociationRegistry.Integrations.Magda;
using AssociationRegistry.Integrations.Magda.Constants;
using AssociationRegistry.Integrations.Magda.Models;
using AssociationRegistry.Integrations.Magda.Onderneming.GeefOnderneming;
using AssociationRegistry.Magda.Kbo;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using CommandHandling.Magda;
using FluentAssertions;
using FluentAssertions.Execution;
using Integrations.Magda.GeefOnderneming;
using Integrations.Magda.GeefOnderneming.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ResultNet;
using Xunit;

public class Given_Geen_Vertegenwoordigers
{
    private readonly MagdaGeefVerenigingService _service;
    private readonly Fixture _fixture;

    public Given_Geen_Vertegenwoordigers()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        var magdaFacade = new Mock<IMagdaClient>();
        var envelope = _fixture.Create<ResponseEnvelope<GeefOndernemingResponseBody>>();

        envelope.Body!.GeefOndernemingResponse!.Repliek.Antwoorden.Antwoord.Inhoud.Onderneming.Functies = [];

        magdaFacade.Setup(facade => facade.GeefOnderneming(It.IsAny<string>(), It.IsAny<CommandMetadata>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(envelope);

        _service = new MagdaGeefVerenigingService(magdaFacade.Object,
                                                  new NullLogger<MagdaGeefVerenigingService>());
    }

    [Fact]
    public async ValueTask Then_It_Returns_A_SuccessResult()
    {
        var result = await _service.GeefVereniging(_fixture.Create<KboNummer>(), _fixture.Create<CommandMetadata>(),
                                                   CancellationToken.None);

        result.IsSuccess().Should().BeTrue();
    }

    [Fact]
    public async ValueTask Then_It_Returns_Empty_Vertegenwoordigers()
    {
        var kboNummer = _fixture.Create<KboNummer>();
        var result = await _service.GeefVereniging(kboNummer, _fixture.Create<CommandMetadata>(), CancellationToken.None);

        using (new AssertionScope())
        {
            var verenigingVolgensKbo = result.Should().BeOfType<Result<VerenigingVolgensKbo>>().Subject.Data;
            verenigingVolgensKbo.Vertegenwoordigers.Should().BeEmpty();
        }
    }
}
