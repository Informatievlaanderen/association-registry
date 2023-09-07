namespace AssociationRegistry.Test.Admin.Api.Magda.MagdaKboService.When_Retrieving_VerenigingVolgensKbo;

using AssociationRegistry.Admin.Api.Magda;
using AssociationRegistry.Framework;
using AssociationRegistry.Magda;
using AssociationRegistry.Magda.Configuration;
using AssociationRegistry.Magda.Models;
using AssociationRegistry.Magda.Models.GeefOnderneming;
using AssociationRegistry.Magda.Onderneming.GeefOnderneming;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ResultNet;
using Test.Framework.Customizations;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_GeefOndernemingResponseBody_With_Fout_Uitzondering
{
    private readonly MagdaGeefVerenigingService _service;
    private readonly Fixture _fixture;
    private readonly Mock<ILogger<MagdaGeefVerenigingService>> _logger;

    public Given_GeefOndernemingResponseBody_With_Fout_Uitzondering()
    {
        _fixture = new Fixture().CustomizeDomain();

        var magdaFacade = new Mock<IMagdaFacade>();
        var envelope = _fixture.Create<ResponseEnvelope<GeefOndernemingResponseBody>>();

        envelope.Body!.GeefOndernemingResponse!.Repliek.Antwoorden.Antwoord.Uitzonderingen = new[]
        {
            new UitzonderingType
            {
                Type = UitzonderingTypeType.FOUT,
                Diagnose = "Er is een fout gebeurd",
            },
            new UitzonderingType
            {
                Type = UitzonderingTypeType.FOUT,
                Diagnose = "Er is nog een fout gebeurd",
            },
        };

        _logger = new Mock<ILogger<MagdaGeefVerenigingService>>();

        magdaFacade.Setup(facade => facade.GeefOnderneming(It.IsAny<string>(), It.IsAny<MagdaCallReference>()))
                   .ReturnsAsync(envelope);

        _service = new MagdaGeefVerenigingService(Mock.Of<IMagdaCallReferenceRepository>(), magdaFacade.Object,
                                                  new TemporaryMagdaVertegenwoordigersSection(), _logger.Object);
    }

    [Fact]
    public async Task Then_It_Returns_A_FailureResult()
    {
        var result = await _service.GeefVereniging(_fixture.Create<KboNummer>(), _fixture.Create<CommandMetadata>(),
                                                   CancellationToken.None);

        result.IsFailure().Should().BeTrue();
    }

    [Fact]
    public async Task Then_It_LogsTheUitzondering()
    {
        var kboNummer = _fixture.Create<KboNummer>();
        await _service.GeefVereniging(kboNummer, _fixture.Create<CommandMetadata>(), CancellationToken.None);

        _logger.Verify(
            expression: x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Information),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString() ==
                                              $"Uitzondering bij het aanroepen van de Magda GeefOnderneming service voor KBO-nummer {kboNummer}: 'Er is een fout gebeurd\nEr is nog een fout gebeurd'"),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!),
            Times.Once);
    }
}
