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

        _client = new GrarClient(new GrarOptionsSection()
        {
            Timeout = 30,
            BaseUrl = "http://localhost:8080",
        }, NullLogger<GrarClient>.Instance);
    }

    [Fact]
    public async Task Then_It_Returns_A_SuccessResult()
    {
       await  _client.GetAddress("Dendermonde", "Leopold II-laan", "99");
    }
}
