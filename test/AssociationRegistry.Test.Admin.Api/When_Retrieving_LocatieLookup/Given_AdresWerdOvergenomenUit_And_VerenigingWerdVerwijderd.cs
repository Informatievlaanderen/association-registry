﻿namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_LocatieLookup;

using Fixtures;
using Fixtures.Scenarios.EventsInDb;
using FluentAssertions;
using Framework;
using templates;
using Xunit;
using Xunit.Categories;

public class Given_AdresWerdOvergenomenUitAdressenregister_And_VerenigingWerdVerwijderd_Setup: IAsyncLifetime
{
    private readonly EventsInDbScenariosFixture _fixture;

    public Given_AdresWerdOvergenomenUitAdressenregister_And_VerenigingWerdVerwijderd_Setup(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
        fixture.Initialize(fixture.V074AdresWerdOvergenomenUitAdressenregisterAndVerenigingWerdVerwijderd).GetAwaiter().GetResult();
    }

        public async Task InitializeAsync()
            => await _fixture.Initialize(_fixture.V074AdresWerdOvergenomenUitAdressenregisterAndVerenigingWerdVerwijderd);

        public async Task DisposeAsync()
        {
        }

}

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_AdresWerdOvergenomenUitAdressenregister_And_VerenigingWerdVerwijderd : IClassFixture<Given_AdresWerdOvergenomenUitAdressenregister_And_VerenigingWerdVerwijderd_Setup>
{
    private readonly AdminApiClient _superAdminApiClient;
    private readonly V074_AdresWerdOvergenomenUitAdressenregister_And_VerenigingWerdVerwijderd _scenario;

    public Given_AdresWerdOvergenomenUitAdressenregister_And_VerenigingWerdVerwijderd(EventsInDbScenariosFixture fixture)
    {
        _superAdminApiClient = fixture.SuperAdminApiClient;
        _scenario = fixture.V074AdresWerdOvergenomenUitAdressenregisterAndVerenigingWerdVerwijderd;
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

        var expected = new LocatieLookupResponseTemplate()
                      .WithVCode(_scenario.VCode).Build();

        content.Should().BeEquivalentJson(expected);
    }
}
