namespace AssociationRegistry.Test.Admin.Api.When_posting_a_new_vereniging;

using System.Net;
using AutoFixture;
using Events;
using Fixtures;
using FluentAssertions;
using Framework;
using global::AssociationRegistry.Admin.Api.Constants;
using global::AssociationRegistry.Admin.Api.Infrastructure;
using global::AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;
using AppSettings = global::AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings.AppSettings;

public class Given_A_Valid_Request_With_All_Fields_Fixture : AdminApiFixture
{
    public RegistreerVerenigingRequest Request { get; }
    public HttpResponseMessage Response { get; private set; } = null!;

    public Given_A_Valid_Request_With_All_Fields_Fixture() : base(
        nameof(Given_A_Valid_Request_With_All_Fields_Fixture))
    {
        var fixture = new Fixture();
        Request = new RegistreerVerenigingRequest
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
                    Hoofdlocatie = true,
                    Locatietype = Locatietypes.Correspondentie,
                },
            },
        };
    }

    protected override Task Given()
        => Task.CompletedTask;

    protected override async Task When()
    {
        Response = await AdminApiClient.RegistreerVereniging(GetJsonBody(Request));
    }

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
    public void Then_it_returns_an_accepted_response()
    {
        _apiFixture.Response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Fact]
    public async Task Then_it_saves_the_events()
    {
        var savedEvent = await _apiFixture.DocumentStore
            .LightweightSession().Events
            .QueryRawEventDataOnly<VerenigingWerdGeregistreerd>()
            .SingleAsync(e => e.Naam == _apiFixture.Request.Naam);
        savedEvent.KorteNaam.Should().Be(_apiFixture.Request.KorteNaam);
        savedEvent.KorteBeschrijving.Should().Be(_apiFixture.Request.KorteBeschrijving);
        savedEvent.Startdatum.Should().Be(_apiFixture.Request.StartDatum);
        savedEvent.KboNummer.Should().Be(_apiFixture.Request.KboNummer);
        savedEvent.ContactInfoLijst.Should().HaveCount(1);
        savedEvent.ContactInfoLijst![0].Should().BeEquivalentTo(_apiFixture.Request.ContactInfoLijst[0]);
        savedEvent.Locaties.Should().HaveCount(1);
        savedEvent.Locaties![0].Should().BeEquivalentTo(_apiFixture.Request.Locaties[0]);
    }

    [Fact]
    public void Then_it_returns_a_location_header()
    {
        _apiFixture.Response.Headers.Should().ContainKey(Microsoft.Net.Http.Headers.HeaderNames.Location);
        _apiFixture.Response.Headers.Location!.OriginalString.Should()
            .StartWith($"{_apiFixture.ServiceProvider.GetRequiredService<AppSettings>().BaseUrl}/v1/verenigingen/V");
    }

    [Fact]
    public void Then_it_returns_a_sequence_header()
    {
        _apiFixture.Response.Headers.Should().ContainKey(WellknownHeaderNames.Sequence);
        var sequenceValues = _apiFixture.Response.Headers.GetValues(WellknownHeaderNames.Sequence).ToList();
        sequenceValues.Should().HaveCount(1);
        var sequence = Convert.ToInt64(sequenceValues.Single());
        sequence.Should().BeGreaterThan(0);
    }
}
