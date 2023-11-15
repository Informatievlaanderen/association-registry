namespace AssociationRegistry.Test.Admin.Api.Afdeling.When_RegistreerAfdeling;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer.Afdeling.RequestModels;
using AutoFixture;
using Fixtures;
using FluentAssertions;
using Framework;
using System.Net;
using Xunit;
using Xunit.Categories;

public sealed class When_RegistreerAfdeling_With_An_Onbekend_MoederKboNummer_Volgens_KBO
{
    private const string KboNummerNietGekendInKBO = "0000000196";
    public readonly RegistreerAfdelingRequest Request;
    public readonly HttpResponseMessage Response;

    public When_RegistreerAfdeling_With_An_Onbekend_MoederKboNummer_Volgens_KBO(EventsInDbScenariosFixture fixture)
    {
        var autoFixture = new Fixture().CustomizeAdminApi();

        Request = new RegistreerAfdelingRequest
        {
            Naam = autoFixture.Create<string>(),
            KboNummerMoedervereniging = KboNummerNietGekendInKBO,
        };

        Response ??= fixture.DefaultClient.RegistreerAfdeling(GetJsonBody(Request)).GetAwaiter().GetResult();
    }

    private string GetJsonBody(RegistreerAfdelingRequest request)
        => GetType()
          .GetAssociatedResourceJson("files.request.with_minimum_fields")
          .Replace("{{vereniging.naam}}", request.Naam)
          .Replace("{{vereniging.kboNummerMoedervereniging}}", request.KboNummerMoedervereniging);
}

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class With_An_Onbekend_MoederKboNummer_Volgens_KBO : IClassFixture<When_RegistreerAfdeling_With_An_Onbekend_MoederKboNummer_Volgens_KBO>
{
    private readonly EventsInDbScenariosFixture _fixture;
    private readonly RegistreerAfdelingRequest _request;
    private readonly HttpResponseMessage _response;

    public With_An_Onbekend_MoederKboNummer_Volgens_KBO(
        When_RegistreerAfdeling_With_An_Onbekend_MoederKboNummer_Volgens_KBO setup,
        EventsInDbScenariosFixture fixture)
    {
        _request = setup.Request;
        _response = setup.Response;
        _fixture = fixture;
    }

    [Fact]
    public void Then_it_returns_a_badRequest_response()
    {
        _response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
