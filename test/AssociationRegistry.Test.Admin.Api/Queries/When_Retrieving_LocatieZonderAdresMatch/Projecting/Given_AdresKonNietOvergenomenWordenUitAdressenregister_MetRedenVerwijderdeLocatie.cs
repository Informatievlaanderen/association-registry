<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Projections/When_Retrieving_LocatieZonderAdresMatch/Projector/Given_AdresKonNietOvergenomenWordenUitAdressenregister_MetRedenVerwijderdeLocatie.cs
﻿namespace AssociationRegistry.Test.Admin.Api.Projections.When_Retrieving_LocatieZonderAdresMatch.Projector;
========
﻿namespace AssociationRegistry.Test.Admin.Api.Queries.When_Retrieving_LocatieZonderAdresMatch.Projecting;
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Queries/When_Retrieving_LocatieZonderAdresMatch/Projecting/Given_AdresKonNietOvergenomenWordenUitAdressenregister_MetRedenVerwijderdeLocatie.cs

using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AutoFixture;
<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Projections/When_Retrieving_LocatieZonderAdresMatch/Projector/Given_AdresKonNietOvergenomenWordenUitAdressenregister_MetRedenVerwijderdeLocatie.cs
using Events;
========
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Queries/When_Retrieving_LocatieZonderAdresMatch/Projecting/Given_AdresKonNietOvergenomenWordenUitAdressenregister_MetRedenVerwijderdeLocatie.cs
using FluentAssertions;
using Framework.Fixtures;
using Marten;
using Xunit;

[Collection(nameof(MultiStreamTestCollection))]
public class
    Given_AdresKonNietOvergenomenWordenUitAdressenregister_MetRedenVerwijderdeLocatie : IClassFixture<GivenLocatieWerdVerwijderdFixture>
{
    private readonly GivenLocatieWerdVerwijderdFixture _fixture;

    public Given_AdresKonNietOvergenomenWordenUitAdressenregister_MetRedenVerwijderdeLocatie(GivenLocatieWerdVerwijderdFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Then_A_Document_Should_Be_Deleted()
    {
        var session = _fixture.DocumentStore.LightweightSession();
        var docs = await session.Query<LocatieZonderAdresMatchDocument>().ToListAsync();
        docs.Should().BeEmpty();
    }
}

public class GivenLocatieWerdVerwijderdBugFixture : MultiStreamTestFixture
{
    public GivenLocatieWerdVerwijderdBugFixture()
    {
        var vCode = "V9900003";
        var locatie = Fixture.Create<Registratiedata.Locatie>() with { LocatieId = 1 };

        Stream(vCode, new IEvent[]
        {
            Fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with { VCode = vCode, Locaties = new[] { locatie } },
            Fixture.Create<LocatieWerdVerwijderd>() with { VCode = vCode, Locatie = locatie },
            Fixture.Create<AdresKonNietOvergenomenWordenUitAdressenregister>() with
            {
                VCode = vCode, LocatieId = locatie.LocatieId,
                Reden = AdresKonNietOvergenomenWordenUitAdressenregister.RedenLocatieWerdVerwijderd,
            },
        });
    }
}
