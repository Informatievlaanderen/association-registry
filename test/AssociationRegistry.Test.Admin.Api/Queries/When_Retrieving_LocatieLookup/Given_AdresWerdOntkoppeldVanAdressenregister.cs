namespace AssociationRegistry.Test.Admin.Api.Queries.When_Retrieving_LocatieLookup;

using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Test.Common.Scenarios.EventsInDb;
using FluentAssertions;
using Framework.Fixtures;
using Framework.templates;
using Xunit;
using Xunit.Categories;

public class Given_AdresWerdOntkoppeldVanAdressenregister_Setup: IAsyncLifetime
{
    private readonly EventsInDbScenariosFixture _fixture;

    public Given_AdresWerdOntkoppeldVanAdressenregister_Setup(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
    }

        public async Task InitializeAsync()
            => await _fixture.Initialize(_fixture.V078AdresWerdOntkoppeldVanAdressenregister);

        public async Task DisposeAsync()
        {
        }

}

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_AdresWerdOntkoppeldVanAdressenregister : IClassFixture<Given_AdresWerdOntkoppeldVanAdressenregister_Setup>
{
    private readonly AdminApiClient _superAdminApiClient;
    private readonly V078_AdresWerdOntkoppeldVanAdressenregister _scenario;

    public Given_AdresWerdOntkoppeldVanAdressenregister(EventsInDbScenariosFixture fixture)
    {
        _superAdminApiClient = fixture.SuperAdminApiClient;
        _scenario = fixture.V078AdresWerdOntkoppeldVanAdressenregister;
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

        foreach (var adresWerdOvergenomenUitAdressenregister in _scenario.AdresWerdOvergenomenUitAdressenregisterList)
        {
            if (adresWerdOvergenomenUitAdressenregister != _scenario.AdresWerdOvergenomenUitAdressenregisterList.First())
            {
                locatieLookupResponseTemplate = locatieLookupResponseTemplate
                   .WithLocatieLookup(adresWerdOvergenomenUitAdressenregister.LocatieId,
                                      new Uri(adresWerdOvergenomenUitAdressenregister.AdresId.Bronwaarde).Segments[^1].TrimEnd('/')
                    );
            }
        }

        var expected = locatieLookupResponseTemplate.Build();
        content.Should().BeEquivalentJson(expected);
    }
}
