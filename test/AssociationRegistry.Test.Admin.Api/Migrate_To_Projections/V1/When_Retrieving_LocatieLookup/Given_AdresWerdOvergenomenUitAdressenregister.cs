namespace AssociationRegistry.Test.Admin.Api.Migrate_To_Projections.V1.When_Retrieving_LocatieLookup;

using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using AssociationRegistry.Test.Admin.Api.Framework.templates;
using AssociationRegistry.Test.Common.Extensions;
using AssociationRegistry.Test.Common.Scenarios.EventsInDb;
using FluentAssertions;
using Xunit;

public class Given_AdresWerdOvergenomenUitAdressenregister_Setup : IAsyncLifetime
{
    private readonly EventsInDbScenariosFixture _fixture;

    public Given_AdresWerdOvergenomenUitAdressenregister_Setup(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
    }

    public async ValueTask InitializeAsync()
        => await _fixture.Initialize(_fixture.V073AdresWerdOvergenomenUitAdressenregister);

    public async ValueTask DisposeAsync()
    {
    }
}

[Collection(nameof(AdminApiCollection))]
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

        foreach (var adresWerdOvergenomenUitAdressenregister in _scenario.ExpectedLocaties)
        {
            locatieLookupResponseTemplate = locatieLookupResponseTemplate
               .WithLocatieLookup(adresWerdOvergenomenUitAdressenregister.LocatieId,
                                  new Uri(adresWerdOvergenomenUitAdressenregister.AdresId.Bronwaarde).Segments[^1].TrimEnd('/')
                );
        }

        var expected = locatieLookupResponseTemplate.Build();
        content.Should().BeEquivalentJson(expected);
    }
}
