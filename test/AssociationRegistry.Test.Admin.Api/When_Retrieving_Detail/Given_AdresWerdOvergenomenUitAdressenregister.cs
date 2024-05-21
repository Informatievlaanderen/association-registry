namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Detail;

using Fixtures;
using Fixtures.Scenarios.EventsInDb;
using FluentAssertions;
using Framework;
using templates;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_AdresWerdOvergenomenUitAdressenregister
{
    private readonly AdminApiClient _superAdminApiClient;
    private readonly V073_AdresWerdOvergenomenUitAdressenregister _scenario;

    public Given_AdresWerdOvergenomenUitAdressenregister(EventsInDbScenariosFixture fixture)
    {
        _superAdminApiClient = fixture.SuperAdminApiClient;
        _scenario = fixture.V073_AdresWerdOvergenomenUitAdressenregister;
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
                      .WithVCode(_scenario.VCode)
                      .WithLocatieLookup(_scenario.AdresWerdOvergenomenUitAdressenregister.LocatieId,
                                         _scenario.AdresWerdOvergenomenUitAdressenregister.OvergenomenAdresUitAdressenregister.AdresId
                                                  .Bronwaarde.Split('/').Last())
                      .Build();

        content.Should().BeEquivalentJson(expected);
    }
}
