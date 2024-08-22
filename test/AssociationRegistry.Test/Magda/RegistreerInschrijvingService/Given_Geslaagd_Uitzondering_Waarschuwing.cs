namespace AssociationRegistry.Test.Magda.RegistreerInschrijvingService;

using AssociationRegistry.Framework;
using AssociationRegistry.Magda;
using AssociationRegistry.Magda.Models;
using AssociationRegistry.Magda.Models.RegistreerInschrijving;
using AssociationRegistry.Magda.Repertorium.RegistreerInschrijving;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using Kbo;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ResultNet;
using Vereniging;
using Xunit;
using Xunit.Categories;
using AntwoordInhoudType = AssociationRegistry.Magda.Repertorium.RegistreerInschrijving.AntwoordInhoudType;

[UnitTest]
public class Given_Geslaagd_Uitzondering_Waarschuwing
{
    private readonly MagdaRegistreerInschrijvingService _service;
    private readonly Fixture _fixture;
    private readonly string _verenigingNaam;
    private readonly string _verenigingKorteNaam;
    private readonly DateOnly _startDatum;
    private readonly AdresVolgensKbo _adres;
    private readonly ContactgegevensVolgensKbo _contactgegevens;

    public Given_Geslaagd_Uitzondering_Waarschuwing()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _verenigingNaam = _fixture.Create<string>();
        _verenigingKorteNaam = _fixture.Create<string>();
        _startDatum = _fixture.Create<DateOnly>();
        _adres = _fixture.Create<AdresVolgensKbo>();
        _contactgegevens = _fixture.Create<ContactgegevensVolgensKbo>();

        var magdaClient = new Mock<IMagdaClient>();
        var responseEnvelope = CreateResponseEnvelope();

        magdaClient.Setup(facade => facade.RegistreerInschrijving(It.IsAny<string>(), It.IsAny<MagdaCallReference>()))
                   .ReturnsAsync(responseEnvelope);

        _service = new MagdaRegistreerInschrijvingService(Mock.Of<IMagdaCallReferenceRepository>(),
                                                          magdaClient.Object,
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

        responseEnvelope.Body.RegistreerInschrijvingResponse.Repliek.Antwoorden.Antwoord.Uitzonderingen = new[]
        {
            new UitzonderingType
            {
                Identificatie = "40143",
                Type = UitzonderingTypeType.WAARSCHUWING,
                Diagnose = "De inschrijving in het MAGDA repertorium bestaat al",
                Oorsprong = "MAGDA",
            },
        };

        return responseEnvelope;
    }

    [Fact]
    public async Task Then_It_Returns_A_SuccessResult()
    {
        var result = await _service.RegistreerInschrijving(_fixture.Create<KboNummer>(), _fixture.Create<CommandMetadata>(),
                                                           CancellationToken.None);

        result.IsSuccess().Should().BeTrue();
    }
}
