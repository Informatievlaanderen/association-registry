namespace AssociationRegistry.Test.Admin.Api.TakeTwo.Given_A_Vereniging_Does_Not_Exist.When_registreer_a_new_vereniging;

using System.Net;
using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using Events;
using Framework;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;
using Xunit.Categories;

public sealed class When_RegistreerVereniging_WithAllFields {
    public readonly RegistreerVerenigingRequest Request;
    private HttpResponseMessage? _response;

    private When_RegistreerVereniging_WithAllFields()
    {
        var autoFixture = new Fixture();
        Request = new RegistreerVerenigingRequest
        {
            Naam = autoFixture.Create<string>(),
            KorteNaam = autoFixture.Create<string>(),
            KorteBeschrijving = autoFixture.Create<string>(),
            StartDatum = DateOnly.FromDateTime(DateTime.Today),
            KboNummer = "0123456749",
            Initiator = "OVO000001",
            ContactInfoLijst = new RegistreerVerenigingRequest.ContactInfo[]
            {
                new()
                {
                    Contactnaam = "Algemeen",
                    Email = "random@adress.be",
                    Telefoon = "0123456789",
                    Website = "www.website.be",
                    SocialMedia = "#social",
                },
            },
            Locaties = new[]
            {
                new RegistreerVerenigingRequest.Locatie
                {
                    Naam = "Kantoor",
                    Straatnaam = "dorpstraat",
                    Huisnummer = "69",
                    Busnummer = "42",
                    Postcode = "0123",
                    Gemeente = "Nothingham",
                    Land = "Belgie",
                    Hoofdlocatie = true,
                    Locatietype = Locatietypes.Correspondentie,
                },
            },
            Vertegenwoordigers = new[]
            {
                new RegistreerVerenigingRequest.Vertegenwoordiger
                {
                    Insz = InszTestSet.Insz1_WithCharacters,
                    Rol = "Voorzitter, Hoofdcoach",
                    Roepnaam = "QTPY",
                    PrimairContactpersoon = true,
                    ContactInfoLijst = new RegistreerVerenigingRequest.ContactInfo[]
                    {
                        new()
                        {
                            Contactnaam = "Algemeen",
                            Email = "qtpy@outlook.com",
                            Telefoon = "0123456789",
                            Website = "www.qt.py",
                            SocialMedia = "#QTPy",
                        },
                    },
                },
                new RegistreerVerenigingRequest.Vertegenwoordiger
                {
                    Insz = InszTestSet.Insz2_WithCharacters,
                    Rol = "Master",
                    Roepnaam = "Lara",
                    PrimairContactpersoon = false,
                    ContactInfoLijst = new RegistreerVerenigingRequest.ContactInfo[]
                    {
                        new()
                        {
                            Email = "master@outlook.com",
                            Telefoon = "9876543210",
                            Website = "www.master.lara",
                            SocialMedia = "#ScrumMaster",
                        },
                    },
                },
            },
        };
    }

    private static When_RegistreerVereniging_WithAllFields? called;
    public static When_RegistreerVereniging_WithAllFields Called
        => called ??= new When_RegistreerVereniging_WithAllFields();

    public HttpResponseMessage Response(AdminApiFixture2 fixture)
        => _response ??= fixture.DefaultClient.RegistreerVereniging(GetJsonBody(Request)).GetAwaiter().GetResult();

    private string GetJsonBody(RegistreerVerenigingRequest request)
        => GetType()
            .GetAssociatedResourceJson($"files.request.with_all_fields")
            .Replace("{{vereniging.naam}}", request.Naam)
            .Replace("{{vereniging.korteNaam}}", request.KorteNaam)
            .Replace("{{vereniging.korteBeschrijving}}", request.KorteBeschrijving)
            .Replace("{{vereniging.startdatum}}", request.StartDatum!.Value.ToString(WellknownFormats.DateOnly))
            .Replace("{{vereniging.kboNummer}}", request.KboNummer)
            .Replace("{{vereniging.initiator}}", request.Initiator)
            .Replace("{{vereniging.contactInfoLijst}}", JsonConvert.SerializeObject(request.ContactInfoLijst))
            .Replace("{{vereniging.locaties}}", JsonConvert.SerializeObject(request.Locaties))
            .Replace("{{vereniging.vertegenwoordigers}}", JsonConvert.SerializeObject(request.Vertegenwoordigers));
}

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class With_All_Fields
{
    private readonly EventsInDbScenariosFixture _fixture;
    private readonly RegistreerVerenigingRequest.Vertegenwoordiger[] _vertegenwoordigers;

    private RegistreerVerenigingRequest Request
        => When_RegistreerVereniging_WithAllFields.Called.Request;
    private HttpResponseMessage Response
        => When_RegistreerVereniging_WithAllFields.Called.Response(_fixture);

    public With_All_Fields(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;

        _vertegenwoordigers = new[]
        {
            new RegistreerVerenigingRequest.Vertegenwoordiger
            {
                Insz = InszTestSet.Insz1,
                Rol = "Voorzitter, Hoofdcoach",
                Roepnaam = "QTPY",
                PrimairContactpersoon = true,
                ContactInfoLijst = new RegistreerVerenigingRequest.ContactInfo[]
                {
                    new()
                    {
                        Contactnaam = "Algemeen",
                        Email = "qtpy@outlook.com",
                        Telefoon = "0123456789",
                        Website = "www.qt.py",
                        SocialMedia = "#QTPy",
                    },
                },
            },
            new RegistreerVerenigingRequest.Vertegenwoordiger
            {
                Insz = InszTestSet.Insz2,
                Rol = "Master",
                Roepnaam = "Lara",
                PrimairContactpersoon = false,
                ContactInfoLijst = new RegistreerVerenigingRequest.ContactInfo[]
                {
                    new()
                    {
                        Email = "master@outlook.com",
                        Telefoon = "9876543210",
                        Website = "www.master.lara",
                        SocialMedia = "#ScrumMaster",
                    },
                },
            },
        };
    }

    [Fact]
    public void Then_it_saves_the_events()
    {
        using var session = _fixture.DocumentStore
            .LightweightSession();

        var savedEvent = session.Events
            .QueryRawEventDataOnly<VerenigingWerdGeregistreerd>()
            .Single(e => e.Naam == Request.Naam);

        savedEvent.KorteNaam.Should().Be(Request.KorteNaam);
        savedEvent.KorteBeschrijving.Should().Be(Request.KorteBeschrijving);
        savedEvent.Startdatum.Should().Be(Request.StartDatum);
        savedEvent.KboNummer.Should().Be(Request.KboNummer);
        savedEvent.ContactInfoLijst.Should().HaveCount(1);
        savedEvent.ContactInfoLijst![0].Should().BeEquivalentTo(Request.ContactInfoLijst[0]);
        savedEvent.Locaties.Should().HaveCount(1);
        savedEvent.Locaties![0].Should().BeEquivalentTo(Request.Locaties[0]);
        savedEvent.Vertegenwoordigers!.Should().BeEquivalentTo(_vertegenwoordigers);
    }

    [Fact]
    public void Then_it_returns_an_accepted_response_with_correct_headers()
    {
        Response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Fact]
    public void Then_it_returns_a_location_header()
    {
        Response.Headers.Should().ContainKey(Microsoft.Net.Http.Headers.HeaderNames.Location);
        Response.Headers.Location!.OriginalString.Should()
            .StartWith($"{_fixture.ServiceProvider.GetRequiredService<AppSettings>().BaseUrl}/v1/verenigingen/V");
    }

    [Fact]
    public void Then_it_returns_a_sequence_header()
    {
        Response.Headers.Should().ContainKey(WellknownHeaderNames.Sequence);
        var sequenceValues = Response.Headers.GetValues(WellknownHeaderNames.Sequence).ToList();
        sequenceValues.Should().HaveCount(1);
        var sequence = Convert.ToInt64(sequenceValues.Single());
        sequence.Should().BeGreaterThan(0);
    }
}
