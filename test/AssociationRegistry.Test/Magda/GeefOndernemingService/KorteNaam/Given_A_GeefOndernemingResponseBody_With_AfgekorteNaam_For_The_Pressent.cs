namespace AssociationRegistry.Test.Magda.GeefOndernemingService.KorteNaam;

using AssociationRegistry.Framework;
using AssociationRegistry.Integrations.Magda;
using AssociationRegistry.Magda.Kbo;
using AssociationRegistry.Integrations.Magda.Models;
using AssociationRegistry.Integrations.Magda.Onderneming.GeefOnderneming;
using AutoFixture;
using CommandHandling.Magda;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging;
using FluentAssertions;
using FluentAssertions.Execution;
using Integrations.Magda.GeefOnderneming;
using Integrations.Magda.GeefOnderneming.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ResultNet;
using Vereniging;
using Xunit;

public class Given_A_GeefOndernemingResponseBody_With_AfgekorteNaam_For_The_Pressent
{
    private readonly MagdaGeefVerenigingService _service;
    private readonly Fixture _fixture;
    private readonly string verenigingNaam;

    public Given_A_GeefOndernemingResponseBody_With_AfgekorteNaam_For_The_Pressent()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        var magdaFacade = new Mock<IMagdaClient>();
        var responseEnvelope = _fixture.Create<ResponseEnvelope<GeefOndernemingResponseBody>>();

        verenigingNaam = _fixture.Create<string>();

        responseEnvelope.Body!.GeefOndernemingResponse!.Repliek.Antwoorden.Antwoord.Inhoud.Onderneming.Namen.AfgekorteNamen = new[]
        {
            new NaamOndernemingType
            {
                Naam = verenigingNaam,
                Taalcode = "nl",
                DatumBegin = "2000-01-01",
                DatumEinde = "2100-01-01",
            },
        };

        magdaFacade.Setup(facade => facade.GeefOnderneming(It.IsAny<string>(), AanroependeFunctie.RegistreerVerenigingMetRechtspersoonlijkheid,It.IsAny<CommandMetadata>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(responseEnvelope);
        _service = new MagdaGeefVerenigingService(magdaFacade.Object,
                                                  new NullLogger<MagdaGeefVerenigingService>());
    }

    [Fact]
    public async ValueTask Then_It_Returns_A_SuccessResult()
    {
        var result = await _service.GeefVereniging(_fixture.Create<KboNummer>(),AanroependeFunctie.RegistreerVerenigingMetRechtspersoonlijkheid, _fixture.Create<CommandMetadata>(),
                                                   CancellationToken.None);

        result.IsSuccess().Should().BeTrue();
    }

    [Fact]
    public async ValueTask Then_It_Returns_A_VerenigingVolgensKbo()
    {
        var kboNummer = _fixture.Create<KboNummer>();
        var result = await _service.GeefVereniging(kboNummer, AanroependeFunctie.RegistreerVerenigingMetRechtspersoonlijkheid,_fixture.Create<CommandMetadata>(), CancellationToken.None);

        using (new AssertionScope())
        {
            var verenigingVolgensKbo = result.Should().BeOfType<Result<VerenigingVolgensKbo>>().Subject.Data;
            verenigingVolgensKbo.KboNummer.Should().BeEquivalentTo(kboNummer);
            verenigingVolgensKbo.KorteNaam.Should().Be(verenigingNaam);
        }
    }
}
