namespace AssociationRegistry.Test.Admin.Api.WhenDetectingDuplicates;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using AutoFixture;
using DuplicateVerenigingDetection;
using Fixtures;
using Fixtures.Scenarios.EventsInDb;
using FluentAssertions;
using Framework;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using Vereniging;
using Xunit;
using Xunit.Categories;
using Adres = AssociationRegistry.Admin.Api.Verenigingen.Common.Adres;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_Some_FeitelijkeVerenigingenGeregistreerd
{
    private readonly V051_FeitelijkeVerenigingWerdGeregistreerd_WithMinimalFields _scenario;
    private readonly AdminApiClient _adminApiClient;
    private readonly IDuplicateVerenigingDetectionService _duplicateDetectionService;
    private readonly Fixture _fixture;

    /// <remarks>
    /// Hoofdletter ongevoelig → Vereniging = verEniging
    /// Spatie ongevoelig
    /// Leading spaces → Grote vereniging =  Grote vereniging
    /// Trailing spaces → Grote vereniging = Grote vereniging
    /// Dubbele spaces → Grote vereniging = Grote     vereniging
    /// Accent ongevoelig → Cafésport = Cafesport
    /// Leesteken ongevoelig → Sint-Servaas = Sint Servaas
    /// Functiewoorden weglaten → De pottestampers = Pottestampers : de, het, van, … idealiter is deze lijst configureerbaar
    /// Fuzzy search = kleine schrijfverschillen negeren. Deze zijn de elastic mogelijkheden:
    /// Ander karakter gebruiken → Uitnodiging = Uitnodiding
    /// 1 karakter minder → Vereniging = Verenging
    /// 1 karakter meer → Vereniging = Vereeniging
    /// 2 karakters van plaats wisselen → Pottestampers = Pottestapmers
    /// </remarks>
    public Given_Some_FeitelijkeVerenigingenGeregistreerd(EventsInDbScenariosFixture fixture)
    {
        _fixture = new Fixture().CustomizeAdminApi();
        _scenario = fixture.V051FeitelijkeVerenigingWerdGeregistreerdWithMinimalFields;
        _adminApiClient = fixture.AdminApiClient;
        _duplicateDetectionService = fixture.ServiceProvider.GetRequiredService<IDuplicateVerenigingDetectionService>();
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
        => (await _adminApiClient.Search(_scenario.VCode)).Should().BeSuccessful();

    [Theory]
    [InlineData("Grote vereniging", "Grote Vereniging")]
    [InlineData("GROTE VERENIGING", "Grote Vereniging")]
    [InlineData("grote vereniging", "Grote Vereniging")]
    [InlineData("De Pottestampers", "De pottestampers")]
    [InlineData("DE POTTESTAMPERS", "De pottestampers")]
    [InlineData("de pottestampers", "De pottestampers")]
    public async Task? Then_A_DuplicateIsDetected_WithDifferentCapitalization(string naamClean, string naamOriginal)
    {
        var request = MaakRegistreerFeitelijkeVerenigingRequest(naamClean);
        var response = await _adminApiClient.RegistreerFeitelijkeVereniging(JsonConvert.SerializeObject(request));
        var responseContent = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var duplicates = JsonConvert.DeserializeObject<PotentialDuplicatesResponse>(responseContent);
        duplicates!.MogelijkeDuplicateVerenigingen.Any(x => x.Naam == naamOriginal).Should().BeTrue();
    }

    [Theory]
    [InlineData("Grote-Vereniging", "Grote Vereniging")]
    [InlineData("Sint Servaas", "Sint-Servaas")]
    [InlineData("Grote Vereniging!", "Grote Vereniging")]
    [InlineData("Sint-Servaas!", "Sint-Servaas")]
    [InlineData("Grote-Vereniging!", "Grote Vereniging")]
    [InlineData("Sint Servaas!", "Sint-Servaas")]
    public async Task? Then_A_DuplicateIsDetected_WithDifferentPunctuation(string naamClean, string naamOriginal)
    {
        var request = MaakRegistreerFeitelijkeVerenigingRequest(naamClean);
        var response = await _adminApiClient.RegistreerFeitelijkeVereniging(JsonConvert.SerializeObject(request));
        var responseContent = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var duplicates = JsonConvert.DeserializeObject<PotentialDuplicatesResponse>(responseContent);
        duplicates!.MogelijkeDuplicateVerenigingen.Any(x => x.Naam == naamOriginal).Should().BeTrue();
    }

    [Theory]
    [InlineData("   Grote Vereniging", "Grote Vereniging")]
    [InlineData("Grote Vereniging   ", "Grote Vereniging")]
    [InlineData("Grote    Vereniging", "Grote Vereniging")]
    [InlineData(" Grote  Vereniging ", "Grote Vereniging")]
    public async Task? Then_A_DuplicateIsDetected_WithAdditionalSpaces(string naamClean, string naamOriginal)
    {
        var request = MaakRegistreerFeitelijkeVerenigingRequest(naamClean);
        var response = await _adminApiClient.RegistreerFeitelijkeVereniging(JsonConvert.SerializeObject(request));
        var responseContent = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var duplicates = JsonConvert.DeserializeObject<PotentialDuplicatesResponse>(responseContent);
        duplicates!.MogelijkeDuplicateVerenigingen.Any(x => x.Naam == naamOriginal).Should().BeTrue();
    }

    [Theory]
    [InlineData("Vereniging van Technologieenthousiasten: Innovacie & Ontwikkeling",
                "Vereniging van Technologïeënthusiasten: Inováçie & Ëntwikkeling")]
    [InlineData("Cafesport", "Cafésport")]
    public async Task? Then_A_DuplicateIsDetected_WithNoAccents(string naamClean, string naamOriginal)
    {
        var request = MaakRegistreerFeitelijkeVerenigingRequest(naamClean);
        var response = await _adminApiClient.RegistreerFeitelijkeVereniging(JsonConvert.SerializeObject(request));
        var responseContent = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var duplicates = JsonConvert.DeserializeObject<PotentialDuplicatesResponse>(responseContent);
        duplicates!.MogelijkeDuplicateVerenigingen.Any(x => x.Naam == naamOriginal).Should().BeTrue();
    }

    [Theory]
    [InlineData("Grote Veréniging", "Grote Vereniging")]
    [InlineData("Dé pottestampers", "De pottestampers")]
    public async Task? Then_A_DuplicateIsDetected_WithMoreAccents(string naamClean, string naamOriginal)
    {
        var request = MaakRegistreerFeitelijkeVerenigingRequest(naamClean);
        var response = await _adminApiClient.RegistreerFeitelijkeVereniging(JsonConvert.SerializeObject(request));
        var responseContent = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var duplicates = JsonConvert.DeserializeObject<PotentialDuplicatesResponse>(responseContent);
        duplicates!.MogelijkeDuplicateVerenigingen.Any(x => x.Naam == naamOriginal).Should().BeTrue();
    }

    private RegistreerFeitelijkeVerenigingRequest MaakRegistreerFeitelijkeVerenigingRequest(string naam)
    {
        var request = new RegistreerFeitelijkeVerenigingRequest
        {
            Naam = naam,
            Startdatum = DateOnly.FromDateTime(DateTime.Now),
            KorteNaam = "",
            KorteBeschrijving = "",
            Locaties = new[]
            {
                new ToeTeVoegenLocatie
                {
                    Locatietype = Locatietype.Correspondentie,
                    Adres = new Adres
                    {
                        Straatnaam = _fixture.Create<string>(),
                        Huisnummer = _fixture.Create<string>(),
                        Postcode = "9832",
                        Gemeente = _fixture.Create<string>(),
                        Land = _fixture.Create<string>(),
                    },
                },
            },
        };

        // var toeTeVoegenLocatie = _fixture.Create<ToeTeVoegenLocatie>();
        // toeTeVoegenLocatie.Adres!.Postcode = "9832";
        // var request = _fixture.Create<RegistreerFeitelijkeVerenigingRequest>();
        // request.Naam = naam;
        //
        // request.Locaties = new[]
        // {
        //     toeTeVoegenLocatie,
        // };

        return request;
    }
}
