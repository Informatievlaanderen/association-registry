namespace AssociationRegistry.Test.Magda.RegistreerInschrijvingService;

using AssociationRegistry.Framework;
using AssociationRegistry.Magda;
using AssociationRegistry.Magda.Models;
using AssociationRegistry.Magda.Models.RegistreerInschrijving;
using AssociationRegistry.Magda.Repertorium.RegistreerInschrijving;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ResultNet;
using Vereniging;
using Xunit;
using Xunit.Categories;
using AntwoordInhoudType = AssociationRegistry.Magda.Repertorium.RegistreerInschrijving.AntwoordInhoudType;

[UnitTest]
public class Given_NietGeslaagd_Uitzondering_Fout
{
    private readonly MagdaRegistreerInschrijvingService _service;
    private readonly Fixture _fixture;

    public Given_NietGeslaagd_Uitzondering_Fout()
    {
        _fixture = new Fixture().CustomizeAdminApi();

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
                Value = ResultaatEnumType.Item0,
                Beschrijving = "Niet geslaagd",
            },
        };

        responseEnvelope.Body.RegistreerInschrijvingResponse.Repliek.Antwoorden.Antwoord.Uitzonderingen = new[]
        {
            new UitzonderingType
            {
                Identificatie = "40088",
                Type = UitzonderingTypeType.FOUT,
                Diagnose = "Interne fout bij verwerking inschrijving in MAGDA repertorium",
                Oorsprong = "MAGDA",
            },
        };

        return responseEnvelope;
    }

    [Fact]
    public async Task Then_It_Returns_A_FailedResult()
    {
        var result = await _service.RegistreerInschrijving(_fixture.Create<KboNummer>(), _fixture.Create<CommandMetadata>(),
                                                           CancellationToken.None);

        result.IsSuccess().Should().BeFalse();
    }
}
