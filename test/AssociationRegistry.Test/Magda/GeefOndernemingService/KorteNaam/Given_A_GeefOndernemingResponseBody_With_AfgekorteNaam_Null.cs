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
using Integrations.Magda.GeefOnderneming;
using Integrations.Magda.GeefOnderneming.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ResultNet;
using Vereniging;
using Xunit;

public class Given_A_GeefOndernemingResponseBody_With_AfgekorteNaam_Null
{
    private readonly Result _verenigingVolgensKbo;

    public Given_A_GeefOndernemingResponseBody_With_AfgekorteNaam_Null()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var magdaFacade = new Mock<IMagdaClient>();
        var responseEnvelope = fixture.Create<ResponseEnvelope<GeefOndernemingResponseBody>>();

        responseEnvelope.Body!.GeefOndernemingResponse!.Repliek.Antwoorden.Antwoord.Inhoud.Onderneming.Namen.AfgekorteNamen = new[]
        {
            new NaamOndernemingType
            {
                Naam = null,
                Taalcode = "nl",
            },
        };

        magdaFacade.Setup(facade => facade.GeefOnderneming(It.IsAny<string>(), AanroependeFunctie.RegistreerVerenigingMetRechtspersoonlijkheid,It.IsAny<CommandMetadata>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(responseEnvelope);

        var service =
            new MagdaGeefVerenigingService(magdaFacade.Object,
                                           new NullLogger<MagdaGeefVerenigingService>());

        _verenigingVolgensKbo =
            service.GeefVereniging(
                        fixture.Create<KboNummer>(),
                        AanroependeFunctie.RegistreerVerenigingMetRechtspersoonlijkheid,
                        fixture.Create<CommandMetadata>(),
                        CancellationToken.None)
                   .GetAwaiter().GetResult();
    }

    [Fact]
    public async ValueTask Then_It_Returns_A_VerenigingVolgensKbo_With_Empty_String_For_KorteNaam()
    {
        var result = (_verenigingVolgensKbo as Result<VerenigingVolgensKbo>).Data;
        result.KorteNaam.Should().BeEmpty();
        result.KorteNaam.Should().NotBeNull();
    }
}
