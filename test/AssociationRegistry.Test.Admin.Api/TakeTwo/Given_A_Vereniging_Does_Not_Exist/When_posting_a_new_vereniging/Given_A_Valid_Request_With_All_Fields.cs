namespace AssociationRegistry.Test.Admin.Api.TakeTwo.Given_A_Vereniging_Does_Not_Exist.When_posting_a_new_vereniging;

using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using Events;
using Framework;
using AutoFixture;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

[Collection(nameof(AdminApiCollection))]
public class Given_A_Valid_Request_With_All_Fields
{
    private readonly GivenEventsFixture _fixture;
    private readonly RegistreerVerenigingRequest _request;

    public Given_A_Valid_Request_With_All_Fields(GivenEventsFixture fixture)
    {
        _fixture = fixture;
        var autoFixture = new Fixture();
        _request = new RegistreerVerenigingRequest
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
        };
        _fixture.AdminApiClient.RegistreerVereniging(GetJsonBody(_request)).GetAwaiter().GetResult();
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

    [Fact]
    public void Then_it_saves_the_events()
    {
        using var session = _fixture.DocumentStore
            .LightweightSession();
        var savedEvent = session.Events
            .QueryRawEventDataOnly<VerenigingWerdGeregistreerd>()
            .Single(e => e.Naam == _request.Naam);

        savedEvent.KorteNaam.Should().Be(_request.KorteNaam);
        savedEvent.KorteBeschrijving.Should().Be(_request.KorteBeschrijving);
        savedEvent.Startdatum.Should().Be(_request.StartDatum);
        savedEvent.KboNummer.Should().Be(_request.KboNummer);
        savedEvent.ContactInfoLijst.Should().HaveCount(1);
        savedEvent.ContactInfoLijst![0].Should().BeEquivalentTo(_request.ContactInfoLijst[0]);
        savedEvent.Locaties.Should().HaveCount(1);
        savedEvent.Locaties![0].Should().BeEquivalentTo(_request.Locaties[0]);
    }
}
