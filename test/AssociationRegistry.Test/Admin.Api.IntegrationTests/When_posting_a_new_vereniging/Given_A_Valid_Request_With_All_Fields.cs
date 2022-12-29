namespace AssociationRegistry.Test.Admin.Api.IntegrationTests.When_posting_a_new_vereniging;

using System.Net;
using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using AutoFixture;
using Fixtures;
using FluentAssertions;
using Framework.Helpers;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Vereniging;
using Xunit;
using AppSettings = AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings.AppSettings;

public class Given_A_Valid_Request_With_All_Fields_Fixture : AdminApiFixture
{
    public Given_A_Valid_Request_With_All_Fields_Fixture() : base(
        nameof(Given_A_Valid_Request_With_All_Fields_Fixture))
    {
        var fixture = new Fixture();
        var request = new RegistreerVerenigingRequest
        {
            Naam = fixture.Create<string>(),
            KorteNaam = fixture.Create<string>(),
            KorteBeschrijving = fixture.Create<string>(),
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
                    HoofdLocatie = true,
                    LocatieType = LocatieTypes.Correspondentie,
                },
            },
        };
        Content = GetJsonBody(request).AsJsonContent();
        Request = request;
    }

    public RegistreerVerenigingRequest Request { get; }

    public StringContent Content { get; }

    private string GetJsonBody(RegistreerVerenigingRequest request)
        => GetType()
            .GetAssociatedResourceJson($"files.request.with_all_fields")
            .Replace("{{vereniging.naam}}", request.Naam)
            .Replace("{{vereniging.korteNaam}}", request.KorteNaam)
            .Replace("{{vereniging.korteBeschrijving}}", request.KorteBeschrijving)
            .Replace("{{vereniging.startdatum}}", request.StartDatum!.Value.ToString(WellknownFormats.DateOnly))
            .Replace("{{vereniging.kboNummer}}", request.KboNummer)
            .Replace("{{vereniging.initiator}}", request.Initiator)
            .Replace("{{vereniging.contacten}}", JsonConvert.SerializeObject(request.ContactInfoLijst))
            .Replace("{{vereniging.locaties}}", JsonConvert.SerializeObject(request.Locaties));
}

public class Given_A_Valid_Request_With_All_Fields : IClassFixture<Given_A_Valid_Request_With_All_Fields_Fixture>
{
    private readonly Given_A_Valid_Request_With_All_Fields_Fixture _apiFixture;

    public Given_A_Valid_Request_With_All_Fields(Given_A_Valid_Request_With_All_Fields_Fixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task Then_it_returns_an_accepted_response()
    {
        var response = await _apiFixture.AdminApiClient.RegistreerVereniging(_apiFixture.Content);
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Fact]
    public async Task Then_it_saves_the_events()
    {
        await _apiFixture.AdminApiClient.RegistreerVereniging(_apiFixture.Content);

        var savedEvent = await _apiFixture.DocumentStore
            .LightweightSession().Events
            .QueryRawEventDataOnly<VerenigingWerdGeregistreerd>()
            .SingleAsync(e => e.Naam == _apiFixture.Request.Naam);
        savedEvent.KorteNaam.Should().Be(_apiFixture.Request.KorteNaam);
        savedEvent.KorteBeschrijving.Should().Be(_apiFixture.Request.KorteBeschrijving);
        savedEvent.Startdatum.Should().Be(_apiFixture.Request.StartDatum);
        savedEvent.KboNummer.Should().Be(_apiFixture.Request.KboNummer);
        savedEvent.Contacten.Should().HaveCount(1);
        savedEvent.Contacten![0].Should().BeEquivalentTo(_apiFixture.Request.ContactInfoLijst[0]);
        savedEvent.Locaties.Should().HaveCount(1);
        savedEvent.Locaties![0].Should().BeEquivalentTo(_apiFixture.Request.Locaties[0]);
    }

    [Fact]
    public async Task Then_it_returns_a_location_header()
    {
        var response = await _apiFixture.AdminApiClient.RegistreerVereniging(_apiFixture.Content);
        response.Headers.Should().ContainKey(Microsoft.Net.Http.Headers.HeaderNames.Location);
        response.Headers.Location!.OriginalString.Should()
            .StartWith($"{_apiFixture.ServiceProvider.GetRequiredService<AppSettings>().BaseUrl}/v1/verenigingen/V");
    }

    [Fact]
    public async Task Then_it_returns_a_sequence_header()
    {
        var response = await _apiFixture.AdminApiClient.RegistreerVereniging(_apiFixture.Content);
        response.Headers.Should().ContainKey(WellknownHeaderNames.Sequence);
        var sequenceValues = response.Headers.GetValues(WellknownHeaderNames.Sequence).ToList();
        sequenceValues.Should().HaveCount(1);
        var sequence = Convert.ToInt64(sequenceValues.Single());
        sequence.Should().BeGreaterThan(0);
    }
}
