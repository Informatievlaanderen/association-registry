<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Projections/When_Searching/Given_AdresWerdGewijzigdInAdressenregister.cs
﻿namespace AssociationRegistry.Test.Admin.Api.Projections.When_Searching;

using Common.Scenarios.EventsInDb;
using FluentAssertions;
using Formats;
using Framework;
using Framework.Fixtures;
using Framework.templates;
using JsonLdContext;
using Vereniging;
========
﻿namespace AssociationRegistry.Test.Admin.Api.Queries.When_Searching;

using AssociationRegistry.Formats;
using AssociationRegistry.JsonLdContext;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Test.Common.Scenarios.EventsInDb;
using AssociationRegistry.Vereniging;
using FluentAssertions;
using Framework.Fixtures;
using Framework.templates;
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Queries/When_Searching/Given_AdresWerdGewijzigdInAdressenregister.cs
using Xunit;
using Xunit.Categories;

public class Given_AdresWerdGewijzigdInAdressenregister_Setup : IAsyncLifetime
{
    private readonly EventsInDbScenariosFixture _fixture;

    public Given_AdresWerdGewijzigdInAdressenregister_Setup(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task InitializeAsync()
        => await _fixture.Initialize(_fixture.V076AdresWerdGewijzigdInAdressenregister);

    public async Task DisposeAsync()
    {
    }
}

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_AdresWerdGewijzigdInAdressenregister : IClassFixture<Given_AdresWerdGewijzigdInAdressenregister_Setup>
{
    private readonly V076_AdresWerdGewijzigdInAdressenregister _scenario;
    private readonly AdminApiClient _adminApiClient;

    public Given_AdresWerdGewijzigdInAdressenregister(EventsInDbScenariosFixture fixture)
    {
        _scenario = fixture.V076AdresWerdGewijzigdInAdressenregister;
        _adminApiClient = fixture.AdminApiClient;
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
        => (await _adminApiClient.Search(_scenario.VCode)).Should().BeSuccessful();

    [Fact]
    public async Task? Then_we_retrieve_one_vereniging_with_the_added_Locatie()
    {
        var response = await _adminApiClient.Search(_scenario.VCode);
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = new ZoekVerenigingenResponseTemplate()
                          .FromQuery(_scenario.VCode)
                          .WithVereniging(
                               v =>
                               {
                                   v.WithVCode(_scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode)
                                    .WithJsonLdType(JsonLdType.FeitelijkeVereniging)
                                    .WithType(Verenigingstype.FeitelijkeVereniging)
                                    .WithNaam(_scenario.FeitelijkeVerenigingWerdGeregistreerd.Naam)
                                    .WithKorteNaam(_scenario.FeitelijkeVerenigingWerdGeregistreerd.KorteNaam)
                                    .WithStartdatum(_scenario.FeitelijkeVerenigingWerdGeregistreerd.Startdatum)
                                    .WithDoelgroep(_scenario.VCode)
                                    .WithLocatie(_scenario.Locatie.Locatietype,
                                                 _scenario.Locatie.Naam,
                                                 _scenario.AdresWerdGewijzigdInAdressenregister
                                                          .Adres.ToAdresString(),
                                                 _scenario.AdresWerdGewijzigdInAdressenregister.Adres
                                                          .Postcode,
                                                 _scenario.AdresWerdGewijzigdInAdressenregister.Adres
                                                          .Gemeente,
                                                 _scenario.VCode,
                                                 _scenario.Locatie.LocatieId,
                                                 _scenario.Locatie.IsPrimair
                                     );

                                   return v;
                               });

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
