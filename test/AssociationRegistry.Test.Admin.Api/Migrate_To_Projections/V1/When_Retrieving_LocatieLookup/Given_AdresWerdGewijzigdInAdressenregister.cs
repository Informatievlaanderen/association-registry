namespace AssociationRegistry.Test.Admin.Api.Migrate_To_Projections.V1.When_Retrieving_LocatieLookup;

using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using AssociationRegistry.Test.Admin.Api.Framework.templates;
using AssociationRegistry.Test.Common.Extensions;
using AssociationRegistry.Test.Common.Scenarios.EventsInDb;
using FluentAssertions;
using Xunit;

public class Given_AdresWerdGewijzigdInAdressenregister_Setup : IAsyncLifetime
{
    private readonly EventsInDbScenariosFixture _fixture;

    public Given_AdresWerdGewijzigdInAdressenregister_Setup(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
    }

    public async ValueTask InitializeAsync()
        => await _fixture.Initialize(_fixture.V075AdresWerdGewijzigdInAdressenregister);

    public async ValueTask DisposeAsync()
    {
    }
}

[Collection(nameof(AdminApiCollection))]
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
    public async ValueTask Then_we_get_a_successful_response()
        => (await _superAdminApiClient.GetLocatieLookup(_scenario.VCode, _scenario.Result.Sequence))
          .Should().BeSuccessful();

    [Fact]
    public async ValueTask Then_we_get_a_locatie_lookup_response()
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
