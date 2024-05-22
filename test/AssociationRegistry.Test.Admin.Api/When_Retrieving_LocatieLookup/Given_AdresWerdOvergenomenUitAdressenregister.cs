﻿namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_LocatieLookup;

using Fixtures;
using Fixtures.Scenarios.EventsInDb;
using FluentAssertions;
using Framework;
using templates;
using Xunit;
using Xunit.Categories;

public class Given_AdresWerdOvergenomenUitAdressenregister_Setup: IAsyncLifetime
{
    private readonly EventsInDbScenariosFixture _fixture;

    public Given_AdresWerdOvergenomenUitAdressenregister_Setup(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task InitializeAsync()
        => await _fixture.Initialize(_fixture.V073AdresWerdOvergenomenUitAdressenregister);

    public async Task DisposeAsync()
    {
    }
}

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_AdresWerdOvergenomenUitAdressenregister : IClassFixture<Given_AdresWerdOvergenomenUitAdressenregister_Setup>
{
    private readonly AdminApiClient _superAdminApiClient;
    private readonly V073_AdresWerdOvergenomenUitAdressenregister _scenario;

    public Given_AdresWerdOvergenomenUitAdressenregister(EventsInDbScenariosFixture fixture)
    {
        _superAdminApiClient = fixture.SuperAdminApiClient;
        _scenario = fixture.V073AdresWerdOvergenomenUitAdressenregister;
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

        foreach (var adresWerdOvergenomenUitAdressenregister in _scenario.ExpectedLocaties)
        {
            locatieLookupResponseTemplate = locatieLookupResponseTemplate
               .WithLocatieLookup(adresWerdOvergenomenUitAdressenregister.LocatieId,
                                  adresWerdOvergenomenUitAdressenregister.OvergenomenAdresUitAdressenregister
                                                                         .AdresId
                                                                         .Bronwaarde);
        }

        var expected = locatieLookupResponseTemplate.Build();
        content.Should().BeEquivalentJson(expected);
    }
}
