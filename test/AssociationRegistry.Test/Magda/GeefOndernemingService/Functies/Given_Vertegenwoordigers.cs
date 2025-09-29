namespace AssociationRegistry.Test.Magda.GeefOndernemingService.Functies;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using AssociationRegistry.Integrations.Magda;
using AssociationRegistry.Integrations.Magda.Constants;
using AssociationRegistry.Integrations.Magda.Models;
using AssociationRegistry.Integrations.Magda.Models.GeefOnderneming;
using AssociationRegistry.Integrations.Magda.Onderneming.GeefOnderneming;
using AssociationRegistry.Magda.Kbo;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ResultNet;
using Xunit;

public class Given_Vertegenwoordigers
{
    private readonly MagdaGeefVerenigingService _service;
    private readonly Fixture _fixture;
    private ResponseEnvelope<GeefOndernemingResponseBody>? _envelope;

    public Given_Vertegenwoordigers()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        var magdaFacade = new Mock<IMagdaClient>();
        _envelope = _fixture.Create<ResponseEnvelope<GeefOndernemingResponseBody>>();

        _envelope.Body!.GeefOndernemingResponse!.Repliek.Antwoorden.Antwoord.Inhoud.Onderneming.Functies =
        [
            _fixture.Create<FunctieType>(),
        ];

        magdaFacade.Setup(facade => facade.GeefOnderneming(It.IsAny<string>(), It.IsAny<MagdaCallReference>()))
                   .ReturnsAsync(_envelope);

        _service = new MagdaGeefVerenigingService(Mock.Of<IMagdaCallReferenceRepository>(), magdaFacade.Object,
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
    public async ValueTask Then_It_Returns_Vertegenwoordigers()
    {
        var kboNummer = _fixture.Create<KboNummer>();
        var result = await _service.GeefVereniging(kboNummer, _fixture.Create<CommandMetadata>(), CancellationToken.None);

        using (new AssertionScope())
        {
            var verenigingVolgensKbo = result.Should().BeOfType<Result<VerenigingVolgensKbo>>().Subject.Data;
            verenigingVolgensKbo.Vertegenwoordigers.Should().NotBeNullOrEmpty();

            verenigingVolgensKbo.Vertegenwoordigers.Should().BeEquivalentTo(_envelope.Body!.GeefOndernemingResponse!.Repliek.Antwoorden
                                                                                     .Antwoord.Inhoud.Onderneming.Functies.Select(
                                                                                          x => new VertegenwoordigerVolgensKbo()
                                                                                          {
                                                                                              Insz = x.Persoon.INSZ,
                                                                                              Voornaam = x.Persoon.VoorNaam,
                                                                                              Achternaam = x.Persoon.AchterNaam
                                                                                          }).ToArray());
        }
    }
}
