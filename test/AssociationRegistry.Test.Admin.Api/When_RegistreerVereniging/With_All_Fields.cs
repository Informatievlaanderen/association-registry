namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging;

using System.Net;
using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using Events;
using Fixtures;
using Framework;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Vereniging;
using Xunit;
using Xunit.Categories;

public sealed class When_RegistreerVereniging_WithAllFields
{
    public readonly RegistreerVerenigingRequest Request;
    public readonly HttpResponseMessage Response;

    private When_RegistreerVereniging_WithAllFields(AdminApiFixture fixture)
    {
        var autoFixture = new Fixture();
        Request = new RegistreerVerenigingRequest
        {
            Naam = autoFixture.Create<string>(),
            KorteNaam = autoFixture.Create<string>(),
            KorteBeschrijving = autoFixture.Create<string>(),
            Startdatum = DateOnly.FromDateTime(DateTime.Today),
            KboNummer = "0123456749",
            Initiator = "OVO000001",
            Contactgegevens = new RegistreerVerenigingRequest.Contactgegeven[]
            {
                new()
                {
                    Type = ContactgegevenType.Email,
                    Waarde = "random@example.org",
                    Omschrijving = "Algemeen",
                    IsPrimair = false,
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
                    Contactgegevens = new RegistreerVerenigingRequest.Contactgegeven[]
                    {
                        new()
                        {
                            Omschrijving = "Algemeen",
                            Waarde = "qtpy@outlook.com",
                            Type = ContactgegevenType.Email,
                            IsPrimair = false,
                        },
                    },
                },
                new RegistreerVerenigingRequest.Vertegenwoordiger
                {
                    Insz = InszTestSet.Insz2_WithCharacters,
                    Rol = "Master",
                    Roepnaam = "Lara",
                    PrimairContactpersoon = false,
                    Contactgegevens = new RegistreerVerenigingRequest.Contactgegeven[]
                    {
                        new()
                        {
                            Omschrijving = "Scrum",
                            Waarde = "master@outlook.com",
                            Type = ContactgegevenType.Email,
                            IsPrimair = true,
                        },
                    },
                },
            },
            HoofdactiviteitenVerenigingsloket = new[] { "BIAG", "BWWC" },
        };

        Response ??= fixture.DefaultClient.RegistreerVereniging(GetJsonBody(Request)).GetAwaiter().GetResult();
    }

    private static When_RegistreerVereniging_WithAllFields? called;

    public static When_RegistreerVereniging_WithAllFields Called(AdminApiFixture fixture)
        => called ??= new When_RegistreerVereniging_WithAllFields(fixture);

    private string GetJsonBody(RegistreerVerenigingRequest request)
        => GetType()
            .GetAssociatedResourceJson($"files.request.with_all_fields")
            .Replace("{{vereniging.naam}}", request.Naam)
            .Replace("{{vereniging.korteNaam}}", request.KorteNaam)
            .Replace("{{vereniging.korteBeschrijving}}", request.KorteBeschrijving)
            .Replace("{{vereniging.startdatum}}", request.Startdatum!.Value.ToString(WellknownFormats.DateOnly))
            .Replace("{{vereniging.kboNummer}}", request.KboNummer)
            .Replace("{{vereniging.initiator}}", request.Initiator)
            .Replace("{{vereniging.contactgegevens}}", JsonConvert.SerializeObject(request.Contactgegevens))
            .Replace("{{vereniging.locaties}}", JsonConvert.SerializeObject(request.Locaties))
            .Replace("{{vereniging.vertegenwoordigers}}", JsonConvert.SerializeObject(request.Vertegenwoordigers))
            .Replace("{{vereniging.hoofdactiviteitenLijst}}", JsonConvert.SerializeObject(request.HoofdactiviteitenVerenigingsloket));
}

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class With_All_Fields
{
    private readonly EventsInDbScenariosFixture _fixture;
    private readonly RegistreerVerenigingRequest.Vertegenwoordiger[] _vertegenwoordigers;

    private RegistreerVerenigingRequest Request
        => When_RegistreerVereniging_WithAllFields.Called(_fixture).Request;

    private HttpResponseMessage Response
        => When_RegistreerVereniging_WithAllFields.Called(_fixture).Response;

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
                Contactgegevens = new RegistreerVerenigingRequest.Contactgegeven[]
                {
                    new()
                    {
                        Omschrijving = "Algemeen",
                        Waarde = "qtpy@outlook.com",
                        Type = ContactgegevenType.Email,
                        IsPrimair = false,
                    },
                },
            },
            new RegistreerVerenigingRequest.Vertegenwoordiger
            {
                Insz = InszTestSet.Insz2,
                Rol = "Master",
                Roepnaam = "Lara",
                PrimairContactpersoon = false,
                Contactgegevens = new RegistreerVerenigingRequest.Contactgegeven[]
                {
                    new()
                    {
                        Omschrijving = "Scrum",
                        Waarde = "master@outlook.com",
                        Type = ContactgegevenType.Email,
                        IsPrimair = true,
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
        savedEvent.Startdatum.Should().Be(Request.Startdatum!.Value);
        savedEvent.KboNummer.Should().Be(Request.KboNummer);
        savedEvent.Contactgegevens.Should().HaveCount(1);
        savedEvent.Contactgegevens[0].Should().BeEquivalentTo(Request.Contactgegevens[0], options => options.ComparingEnumsByName());
        savedEvent.Locaties.Should().HaveCount(1);
        savedEvent.Locaties[0].Should().BeEquivalentTo(Request.Locaties[0]);
        savedEvent.Vertegenwoordigers.Should().BeEquivalentTo(_vertegenwoordigers, options => options.ComparingEnumsByName());
        savedEvent.HoofdactiviteitenVerenigingsloket.Should().BeEquivalentTo(
            new[]
            {
                new VerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket("BIAG", "Burgerinitiatief & Actiegroep"),
                new VerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket("BWWC", "Buurtwerking & Wijkcomité"),
            });
    }

    [Fact]
    public void Then_it_returns_an_accepted_response()
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
