namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_RegistreerFeitelijkeVereniging;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using AutoFixture;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using FluentAssertions;
using Formats;
using Framework;
using Framework.Categories;
using Framework.Fixtures;
using Newtonsoft.Json;
using System.Collections;
using System.Net;
using Xunit;
using Xunit.Categories;

public class When_RegistreerFeitelijkeVereniging_WithHtmlFields_Data : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        var autoFixture = new Fixture().CustomizeAdminApi();

        var request1 = autoFixture.Create<RegistreerFeitelijkeVerenigingRequest>();
        request1.Naam = $"<h1>{autoFixture.Create<string>()}</h1>";

        yield return new object[] { request1 };

        var request2 = autoFixture.Create<RegistreerFeitelijkeVerenigingRequest>();
        request2.HoofdactiviteitenVerenigingsloket = new[] { "<script>alert('HELP!')</script>" };

        yield return new object[] { request2 };

        var request3 = autoFixture.Create<RegistreerFeitelijkeVerenigingRequest>();
        request3.KorteNaam = "<a href='http://www/example.org'>Click here</a>";

        yield return new object[] { request3 };

        var request4 = autoFixture.Create<RegistreerFeitelijkeVerenigingRequest>();
        var contactgegeven = autoFixture.Create<ToeTeVoegenContactgegeven>();

        contactgegeven.Beschrijving = "<pre></pre>";
        contactgegeven.Beschrijving = $"<h2>{autoFixture.Create<string>()}</h2>";
        contactgegeven.Waarde = "<span>Test</span>";

        request4.Contactgegevens = request4.Contactgegevens.Append(contactgegeven).ToArray();

        yield return new object[] { request4 };
    }

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTestToRefactor]
[IntegrationTest]
public class With_Html_Fields
{
    private readonly EventsInDbScenariosFixture _fixture;

    public With_Html_Fields(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
    }

    [Theory]
    [ClassData(typeof(When_RegistreerFeitelijkeVereniging_WithHtmlFields_Data))]
    public async Task Then_it_returns_a_bad_request_response(RegistreerFeitelijkeVerenigingRequest request)
    {
        var response = await _fixture.DefaultClient.RegistreerFeitelijkeVereniging(GetJsonBody(request));
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var body = JsonConvert.DeserializeObject<ValidationProblemDetails>(await response.Content.ReadAsStringAsync());

        body.HttpStatus.Should().Be(400);
        body.Detail.Should().ContainAny("Validatie mislukt!");
    }

    private string GetJsonBody(RegistreerFeitelijkeVerenigingRequest request)
        => GetType()
          .GetAssociatedResourceJson("files.request.with_all_fields")
          .Replace(oldValue: "{{vereniging.naam}}", request.Naam)
          .Replace(oldValue: "{{vereniging.korteNaam}}", request.KorteNaam)
          .Replace(oldValue: "{{vereniging.korteBeschrijving}}", request.KorteBeschrijving)
          .Replace(oldValue: "{{vereniging.isUitgeschrevenUitPubliekeDatastroom}}",
                   request.IsUitgeschrevenUitPubliekeDatastroom.ToString().ToLower())
          .Replace(oldValue: "{{vereniging.startdatum}}", request.Startdatum!.Value.ToString(WellknownFormats.DateOnly))
          .Replace(oldValue: "{{vereniging.doelgroep.minimumleeftijd}}", request.Doelgroep!.Minimumleeftijd.ToString())
          .Replace(oldValue: "{{vereniging.doelgroep.maximumleeftijd}}", request.Doelgroep!.Maximumleeftijd.ToString())
          .Replace(oldValue: "{{vereniging.contactgegevens}}", JsonConvert.SerializeObject(request.Contactgegevens))
          .Replace(oldValue: "{{vereniging.locaties}}", JsonConvert.SerializeObject(request.Locaties))
          .Replace(oldValue: "{{vereniging.vertegenwoordigers}}", JsonConvert.SerializeObject(request.Vertegenwoordigers))
          .Replace(oldValue: "{{vereniging.hoofdactiviteitenLijst}}",
                   JsonConvert.SerializeObject(request.HoofdactiviteitenVerenigingsloket));
}
