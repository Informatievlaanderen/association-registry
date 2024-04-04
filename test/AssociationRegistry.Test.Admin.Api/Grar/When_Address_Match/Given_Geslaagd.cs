namespace AssociationRegistry.Test.Admin.Api.Grar.When_Address_Match;

using AssociationRegistry.Framework;
using AssociationRegistry.Grar;
using AssociationRegistry.Grar.Configuration;
using AssociationRegistry.Magda.Models;
using AssociationRegistry.Magda.Models.RegistreerInschrijving;
using AssociationRegistry.Magda.Repertorium.RegistreerInschrijving;
using AutoFixture;
using Framework;
using Microsoft.Extensions.Logging.Abstractions;
using Vereniging;
using Xunit;
using Xunit.Categories;
using AntwoordInhoudType = AssociationRegistry.Magda.Repertorium.RegistreerInschrijving.AntwoordInhoudType;

[UnitTest]
public class Given_Geslaagd
{
    private readonly IGrarClient _client;
    private readonly Fixture _fixture;
    private readonly string _verenigingNaam;

    public Given_Geslaagd()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _verenigingNaam = _fixture.Create<string>();

        var responseEnvelope = CreateResponseEnvelope();

        _client = new GrarClient(new GrarOptionsSection()
        {
            Timeout = 30,
            BaseUrl = "http://localhost:8080"
        }, NullLogger<GrarClient>.Instance);
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

        return responseEnvelope;
    }

    [Fact]
    public async Task Then_It_Returns_A_SuccessResult()
    {
    }
}
