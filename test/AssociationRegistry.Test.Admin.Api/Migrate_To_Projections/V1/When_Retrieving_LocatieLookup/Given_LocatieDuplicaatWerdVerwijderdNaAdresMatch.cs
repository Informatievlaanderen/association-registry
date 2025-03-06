namespace AssociationRegistry.Test.Admin.Api.Migrate_To_Projections.V1.When_Retrieving_LocatieLookup;

using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using AssociationRegistry.Test.Admin.Api.Framework.templates;
using AssociationRegistry.Test.Common.Extensions;
using AssociationRegistry.Test.Common.Scenarios.EventsInDb;
using FluentAssertions;
using System.Threading.Tasks;
using Xunit;
using Xunit.Categories;

public class Given_LocatieDuplicaatWerdVerwijderdNaAdresMatch_Setup : IAsyncLifetime
{
    private readonly EventsInDbScenariosFixture _fixture;

    public Given_LocatieDuplicaatWerdVerwijderdNaAdresMatch_Setup(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
    }

    public async ValueTask InitializeAsync()
        => await _fixture.Initialize(_fixture.V077LocatieDuplicaatWerdVerwijderdNaAdresMatch);

    public async ValueTask DisposeAsync()
    {
    }
}

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_LocatieDuplicaatWerdVerwijderdNaAdresMatch : IClassFixture<Given_LocatieDuplicaatWerdVerwijderdNaAdresMatch_Setup>
{
    private readonly AdminApiClient _superAdminApiClient;
    private readonly V077_LocatieDuplicaatWerdVerwijderdNaAdresMatch _scenario;

    public Given_LocatieDuplicaatWerdVerwijderdNaAdresMatch(EventsInDbScenariosFixture fixture)
    {
        _superAdminApiClient = fixture.SuperAdminApiClient;
        _scenario = fixture.V077LocatieDuplicaatWerdVerwijderdNaAdresMatch;
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
        => (await _superAdminApiClient.GetLocatieLookup(_scenario.VCode, _scenario.Result.Sequence))
          .Should().BeSuccessful();

    [Fact]
    public async Task Then_we_get_a_locatie_lookup_response()
    {
        var response = await _superAdminApiClient.GetLocatieLookup(_scenario.VCode);
        var content = await response.Content.ReadAsStringAsync();

        var locatieLookupResponseTemplate = new LocatieLookupResponseTemplate()
           .WithVCode(_scenario.VCode);

        var expected = locatieLookupResponseTemplate.Build();
        content.Should().BeEquivalentJson(expected);
    }
}
