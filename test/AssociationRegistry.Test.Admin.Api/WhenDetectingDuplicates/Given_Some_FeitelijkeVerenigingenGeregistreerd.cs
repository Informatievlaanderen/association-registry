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
using Locatie = Vereniging.Locatie;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_Some_FeitelijkeVerenigingenGeregistreerd
{
    private readonly V051_FeitelijkeVerenigingWerdGeregistreerd_WithMinimalFields _scenario;
    private readonly AdminApiClient _adminApiClient;
    private readonly IDuplicateVerenigingDetectionService _duplicateDetectionService;
    private readonly Fixture _fixture;

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

    [Fact]
    public async Task? Then_A_DuplicateIsDetected_WithLeadingSpaces()
    {
        var request = _fixture.Create<RegistreerFeitelijkeVerenigingRequest>();
        request.Naam = " Grote vereniging";

        var toeTeVoegenLocatie = _fixture.Create<ToeTeVoegenLocatie>();
        toeTeVoegenLocatie.Adres!.Postcode = "9832";

        request.Locaties = new[]
        {
            toeTeVoegenLocatie,
        };

        var response = await _adminApiClient.RegistreerFeitelijkeVereniging(JsonConvert.SerializeObject(request));

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var responseContent = await response.Content.ReadAsStringAsync();
        var duplicates = JsonConvert.DeserializeObject<PotentialDuplicatesResponse>(responseContent);

        duplicates!.MogelijkeDuplicateVerenigingen.Any(x => x.Naam == "Grote vereniging").Should().BeTrue();
    }

    [Fact]
    public async Task? Then_A_DuplicateIsDetected_WithNoAccents()
    {
        var duplicates =
            await _duplicateDetectionService.GetDuplicates(
                VerenigingsNaam.Create("Vereniging van Technologieenthousiasten: Innovacie & Ontwikkeling"),
                Array.Empty<Locatie>());

        duplicates.Should().HaveCount(1);

        duplicates.Single().Naam.Should().Be("Vereniging van Technologïeënthusiasten: Inováçie & Ëntwikkeling");
    }

    [Fact]
    public async Task? Then_A_DuplicateIsDetected_WithMoreAccents()
    {
        var duplicates =
            await _duplicateDetectionService.GetDuplicates(
                VerenigingsNaam.Create("Vërëniging van Technologïeënthusiasten: Inováçie & Ëntwikkeling"),
                Array.Empty<Locatie>());

        duplicates.Should().HaveCount(1);

        duplicates.Single().Naam.Should().Be("Vereniging van Technologïeënthusiasten: Inováçie & Ëntwikkeling");
    }
}
