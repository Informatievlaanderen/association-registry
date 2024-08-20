namespace AssociationRegistry.Test.Admin.Api.Queries.When_Retrieving_LocatieLookup;

using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Test.Common.Scenarios.EventsInDb;
using FluentAssertions;
using Framework.Fixtures;
using Framework.templates;
using Xunit;
using Xunit.Categories;

public class Given_AdresWerdGewijzigdInAdressenregister_Setup: IAsyncLifetime
{
    private readonly EventsInDbScenariosFixture _fixture;

    public Given_AdresWerdGewijzigdInAdressenregister_Setup(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
    }

        public async Task InitializeAsync()
            => await _fixture.Initialize(_fixture.V075AdresWerdGewijzigdInAdressenregister);

        public async Task DisposeAsync()
        {
        }

}

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_AdresWerdGewijzigdInAdressenregister : IClassFixture<Given_AdresWerdGewijzigdInAdressenregister_Setup>
{
    private readonly AdminApiClient _superAdminApiClient;
    private readonly V075_AdresWerdGewijzigdInAdressenregister _scenario;

    public Given_AdresWerdGewijzigdInAdressenregister(EventsInDbScenariosFixture fixture)
    {
        _superAdminApiClient = fixture.SuperAdminApiClient;
        _scenario = fixture.V075AdresWerdGewijzigdInAdressenregister;
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


        locatieLookupResponseTemplate = locatieLookupResponseTemplate
           .WithLocatieLookup(_scenario.AdresWerdGewijzigdInAdressenregister.LocatieId,
                              new Uri(_scenario.AdresWerdGewijzigdInAdressenregister.AdresId.Bronwaarde).Segments[^1].TrimEnd('/')
            );

        var expected = locatieLookupResponseTemplate.Build();
        content.Should().BeEquivalentJson(expected);
    }
}
