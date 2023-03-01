namespace AssociationRegistry.Test.Admin.Api.Given_VerenigingWerdGeregistreerd.When_RegistreerVereniging;

using System.Net;
using AssociationRegistry.Admin.Api.Verenigingen;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using Fixtures;
using Framework;
using AutoFixture;
using Events;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;
using Xunit.Categories;

public sealed class When_RegistreerVereniging_With_Duplicate_But_Valid_Hash
{
    public readonly string VCode;
    public readonly string Naam;
    public readonly RegistreerVerenigingRequest Request;
    public readonly HttpResponseMessage Response;
    public string RequestAsJson { get; }


    private When_RegistreerVereniging_With_Duplicate_But_Valid_Hash(EventsInDbScenariosFixture fixture)
    {
        var autoFixture = new Fixture().CustomizeAll();
        var locatie = autoFixture.Create<RegistreerVerenigingRequest.Locatie>();

        locatie.Gemeente = fixture.VerenigingWerdGeregistreerdWithAllFieldsEventsInDbScenario.VerenigingWerdGeregistreerd.Locaties.First().Gemeente;
        Request = new RegistreerVerenigingRequest()
        {
            Naam = fixture.VerenigingWerdGeregistreerdWithAllFieldsEventsInDbScenario.VerenigingWerdGeregistreerd.Naam,
            Locaties = new[]
            {
                locatie,
            },
            Initiator = "OVO000001",
        };
        VCode = fixture.VerenigingWerdGeregistreerdWithAllFieldsEventsInDbScenario.VCode;
        Naam = fixture.VerenigingWerdGeregistreerdWithAllFieldsEventsInDbScenario.Naam;

        RequestAsJson = JsonConvert.SerializeObject(Request);
        Response = fixture.DefaultClient.RegistreerVereniging(RequestAsJson, BevestigingsTokenHelper.Calculate(Request)).GetAwaiter().GetResult();
    }

    private static When_RegistreerVereniging_With_Duplicate_But_Valid_Hash? called;

    public static When_RegistreerVereniging_With_Duplicate_But_Valid_Hash Called(EventsInDbScenariosFixture fixture)
        => called ??= new When_RegistreerVereniging_With_Duplicate_But_Valid_Hash(fixture);
}

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class When_Duplicate_But_Valid_Hash
{
    private readonly EventsInDbScenariosFixture _fixture;

    private HttpResponseMessage Response
        => When_RegistreerVereniging_With_Duplicate_But_Valid_Hash.Called(_fixture).Response;

    private string VCode
        => When_RegistreerVereniging_With_Duplicate_But_Valid_Hash.Called(_fixture).VCode;

    private string Naam
        => When_RegistreerVereniging_With_Duplicate_But_Valid_Hash.Called(_fixture).Naam;

    private RegistreerVerenigingRequest Request
        => When_RegistreerVereniging_With_Duplicate_But_Valid_Hash.Called(_fixture).Request;

    private string ResponseBody
        => @$"{{""bevestigingsToken"": ""{BevestigingsTokenHelper.Calculate(Request)}"", ""duplicaten"":[{{""vCode"":""V0001001"",""naam"":""{Naam}""}}]}}";

    public When_Duplicate_But_Valid_Hash(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Then_it_returns_a_conflict_response()
    {
        Response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Fact]
    public void Then_it_saves_an_extra_events()
    {
        using var session = _fixture.DocumentStore
            .LightweightSession();
        session.Events
            .QueryRawEventDataOnly<VerenigingWerdGeregistreerd>()
            .Where(@event => @event.Naam == Naam)
            .Should().HaveCount(2);
    }
}
