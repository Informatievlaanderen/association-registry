namespace AssociationRegistry.Test.Admin.Api.TakeTwo.Given_A_Vereniging_Does_Not_Exist.When_registreer_a_new_vereniging;

using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using Events;
using Framework;
using AutoFixture;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class With_All_Fields
{
    private readonly GivenEventsFixture _fixture;
    private readonly RegistreerVerenigingRequest _request;

    public With_All_Fields(GivenEventsFixture fixture)
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
            Vertegenwoordigers = new[]
            {
                new RegistreerVerenigingRequest.Vertegenwoordiger
                {
                    Insz = "96123119307",
                    Rol = "Voorzitter, Hoofdcoach",
                    Roepnaam = "QTPY",
                    PrimairContactpersoon = true,
                },
                new RegistreerVerenigingRequest.Vertegenwoordiger
                {
                    Insz = "01131500149",
                    Rol = "Master",
                    Roepnaam = "Lara",
                    PrimairContactpersoon = false,
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
            .Replace("{{vereniging.locaties}}", JsonConvert.SerializeObject(request.Locaties))
            .Replace("{{vereniging.vertegenwoordigers}}", JsonConvert.SerializeObject(request.Vertegenwoordigers));

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
// TODO        savedEvent.Vertegenwoordigers!.Should().BeEquivalentTo(_request.Vertegenwoordigers);
    }
}
