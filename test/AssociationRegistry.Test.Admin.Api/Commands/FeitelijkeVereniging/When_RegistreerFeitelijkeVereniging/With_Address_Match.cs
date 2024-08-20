namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_RegistreerFeitelijkeVereniging;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Vereniging;
using AutoFixture;
using FluentAssertions;
using Framework.Fixtures;
using Newtonsoft.Json;
using System.Net;
using Xunit;
using Xunit.Categories;
using Adres = AssociationRegistry.Admin.Api.Verenigingen.Common.Adres;

public sealed class When_RegistreerFeitelijkeVereniging_WithAddressMatch
{
    private static When_RegistreerFeitelijkeVereniging_WithAddressMatch? called;
    public readonly RegistreerFeitelijkeVerenigingRequest Request;
    public readonly HttpResponseMessage Response;

    private When_RegistreerFeitelijkeVereniging_WithAddressMatch(AdminApiFixture fixture)
    {
        var autoFixture = new Fixture().CustomizeAdminApi();

        Request = new RegistreerFeitelijkeVerenigingRequest
        {
            Naam = autoFixture.Create<string>(),
            Locaties = new[]
            {
                new ToeTeVoegenLocatie
                {
                    Naam = "Kantoor",
                    Adres = new Adres
                    {
                        Straatnaam = "Leopold II-laan",
                        Huisnummer = "99",
                        Postcode = "9200",
                        Gemeente = "Dendermonde",
                        Land = "België",
                    },
                    IsPrimair = true,
                    Locatietype = Locatietype.Correspondentie,
                },
            },
        };

        Response ??= fixture.DefaultClient.RegistreerFeitelijkeVereniging(GetJsonBody(Request)).GetAwaiter().GetResult();
    }

    public static When_RegistreerFeitelijkeVereniging_WithAddressMatch Called(AdminApiFixture fixture)
        => called ??= new When_RegistreerFeitelijkeVereniging_WithAddressMatch(fixture);

    private string GetJsonBody(RegistreerFeitelijkeVerenigingRequest request)
        => GetType()
          .GetAssociatedResourceJson("files.request.with_address_match")
          .Replace(oldValue: "{{vereniging.naam}}", request.Naam)
          .Replace(oldValue: "{{vereniging.locaties}}", JsonConvert.SerializeObject(request.Locaties));
}

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class With_Address_Match
{
    private readonly EventsInDbScenariosFixture _fixture;

    public With_Address_Match(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
    }

    private RegistreerFeitelijkeVerenigingRequest Request
        => When_RegistreerFeitelijkeVereniging_WithAllFields.Called(_fixture).Request;

    private HttpResponseMessage Response
        => When_RegistreerFeitelijkeVereniging_WithAllFields.Called(_fixture).Response;

    [Fact]
    public void Then_it_returns_an_accepted_response()
    {
        Response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }
}
