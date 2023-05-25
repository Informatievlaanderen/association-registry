namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_RegistreerFeitelijkeVereniging;

using System.Net;
using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.DecentraalBeheerdeVereniging;
using Events;
using Fixtures;
using Framework;
using Vereniging;
using AutoFixture;
using FluentAssertions;
using JasperFx.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Xunit;
using Xunit.Categories;

public sealed class When_RegistreerFeitelijkeVereniging_WithAllFields
{
    private static When_RegistreerFeitelijkeVereniging_WithAllFields? called;
    public readonly RegistreerDecentraalBeheerdeVerenigingRequest Request;
    public readonly HttpResponseMessage Response;

    private When_RegistreerFeitelijkeVereniging_WithAllFields(AdminApiFixture fixture)
    {
        var autoFixture = new Fixture().CustomizeAll();
        Request = new RegistreerDecentraalBeheerdeVerenigingRequest
        {
            Naam = autoFixture.Create<string>(),
            KorteNaam = autoFixture.Create<string>(),
            KorteBeschrijving = autoFixture.Create<string>(),
            Startdatum = DateOnly.FromDateTime(DateTime.Today),
            Contactgegevens = new ToeTeVoegenContactgegeven[]
            {
                new()
                {
                    Type = ContactgegevenType.Email,
                    Waarde = "random@example.org",
                    Beschrijving = "Algemeen",
                    IsPrimair = false,
                },
            },
            Locaties = new[]
            {
                new ToeTeVoegenLocatie
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
                new ToeTeVoegenVertegenwoordiger
                {
                    Insz = autoFixture.Create<Insz>(),
                    Rol = "Voorzitter, Hoofdcoach",
                    Roepnaam = "QTPY",
                    IsPrimair = true,
                    Email = "qtpy@example.org",
                    Telefoon = "0123456789",
                    Mobiel = "987654321",
                    SocialMedia = "http://example.com",
                },
                new ToeTeVoegenVertegenwoordiger
                {
                    Insz = autoFixture.Create<Insz>(),
                    Rol = "Master",
                    Roepnaam = "Lara",
                    IsPrimair = false,
                    Email = "master@example.org",
                    Telefoon = "0000000000",
                    Mobiel = "6666666666",
                    SocialMedia = "http://example.com/scrum",
                },
            },
            HoofdactiviteitenVerenigingsloket = new[] { "BIAG", "BWWC" },
        };

        Response ??= fixture.DefaultClient.RegistreerFeitelijkeVereniging(GetJsonBody(Request)).GetAwaiter().GetResult();
    }

    public static When_RegistreerFeitelijkeVereniging_WithAllFields Called(AdminApiFixture fixture)
        => called ??= new When_RegistreerFeitelijkeVereniging_WithAllFields(fixture);

    private string GetJsonBody(RegistreerDecentraalBeheerdeVerenigingRequest request)
        => GetType()
            .GetAssociatedResourceJson("files.request.with_all_fields")
            .Replace("{{vereniging.naam}}", request.Naam)
            .Replace("{{vereniging.korteNaam}}", request.KorteNaam)
            .Replace("{{vereniging.korteBeschrijving}}", request.KorteBeschrijving)
            .Replace("{{vereniging.startdatum}}", request.Startdatum!.Value.ToString(WellknownFormats.DateOnly))
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

    public With_All_Fields(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
    }

    private RegistreerDecentraalBeheerdeVerenigingRequest Request
        => When_RegistreerFeitelijkeVereniging_WithAllFields.Called(_fixture).Request;

    private HttpResponseMessage Response
        => When_RegistreerFeitelijkeVereniging_WithAllFields.Called(_fixture).Response;

    [Fact]
    public void Then_it_saves_the_events()
    {
        using var session = _fixture.DocumentStore
            .LightweightSession();

        var savedEvent = session.Events
            .QueryRawEventDataOnly<FeitelijkeVerenigingWerdGeregistreerd>()
            .Single(e => e.Naam == Request.Naam);

        savedEvent.KorteNaam.Should().Be(Request.KorteNaam);
        savedEvent.KorteBeschrijving.Should().Be(Request.KorteBeschrijving);
        savedEvent.Startdatum.Should().Be(Request.Startdatum!.Value);
        savedEvent.Contactgegevens.Should().HaveCount(expected: 1);
        savedEvent.Contactgegevens[0].Should().BeEquivalentTo(Request.Contactgegevens[0], options => options.ComparingEnumsByName());
        savedEvent.Locaties.Should().HaveCount(expected: 1);
        savedEvent.Locaties[0].Should().BeEquivalentTo(Request.Locaties[0]);
        savedEvent.Vertegenwoordigers.Should().BeEquivalentTo(Request.Vertegenwoordigers, options => options.ComparingEnumsByName());
        savedEvent.Vertegenwoordigers.ForEach(x => x.VertegenwoordigerId.Should().BePositive());
        savedEvent.Vertegenwoordigers.Select(x => x.VertegenwoordigerId).ToList().Should().OnlyHaveUniqueItems();
        savedEvent.HoofdactiviteitenVerenigingsloket.Should().BeEquivalentTo(
            new[]
            {
                new FeitelijkeVerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket("BIAG", "Burgerinitiatief & Actiegroep"),
                new FeitelijkeVerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket("BWWC", "Buurtwerking & Wijkcomité"),
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
        Response.Headers.Should().ContainKey(HeaderNames.Location);
        Response.Headers.Location!.OriginalString.Should()
            .StartWith($"{_fixture.ServiceProvider.GetRequiredService<AppSettings>().BaseUrl}/v1/verenigingen/V");
    }

    [Fact]
    public void Then_it_returns_a_sequence_header()
    {
        Response.Headers.Should().ContainKey(WellknownHeaderNames.Sequence);
        var sequenceValues = Response.Headers.GetValues(WellknownHeaderNames.Sequence).ToList();
        sequenceValues.Should().HaveCount(expected: 1);
        var sequence = Convert.ToInt64(sequenceValues.Single());
        sequence.Should().BeGreaterThan(expected: 0);
    }
}
