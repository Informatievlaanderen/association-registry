namespace AssociationRegistry.Test.Admin.Api.WhenDetectingDuplicates;

using DuplicateVerenigingDetection;
using Fixtures;
using Fixtures.Scenarios.EventsInDb;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Vereniging;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_An_Accented_Word
{
    private readonly V051_FeitelijkeVerenigingWerdGeregistreerd_WithMinimalFields _scenario;
    private readonly AdminApiClient _adminApiClient;
    private readonly IDuplicateVerenigingDetectionService _duplicateDetectionService;

    public Given_An_Accented_Word(EventsInDbScenariosFixture fixture)
    {
        _scenario = fixture.V051FeitelijkeVerenigingWerdGeregistreerdWithMinimalFields;
        _adminApiClient = fixture.AdminApiClient;
        _duplicateDetectionService = fixture.ServiceProvider.GetRequiredService<IDuplicateVerenigingDetectionService>();
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
        => (await _adminApiClient.Search(_scenario.VCode)).Should().BeSuccessful();

    [Fact]
    public async Task? Then_A_DuplicateIsDetected_WithExactlyTheSameName()
    {
        var duplicates =
            await _duplicateDetectionService.GetDuplicates(
                VerenigingsNaam.Create("Vereniging van Technologïeënthusiasten: Inováçie & Ëntwikkeling"),
                Array.Empty<Locatie>());

        duplicates.Should().HaveCount(1);

        duplicates.Single().Naam.Should().Be("Vereniging van Technologïeënthusiasten: Inováçie & Ëntwikkeling");
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
