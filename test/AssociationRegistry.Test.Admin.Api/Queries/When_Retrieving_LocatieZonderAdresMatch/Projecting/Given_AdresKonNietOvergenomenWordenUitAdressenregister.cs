<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Projections/When_Retrieving_LocatieZonderAdresMatch/Projector/Given_AdresKonNietOvergenomenWordenUitAdressenregister.cs
﻿namespace AssociationRegistry.Test.Admin.Api.Projections.When_Retrieving_LocatieZonderAdresMatch.Projector;
========
﻿namespace AssociationRegistry.Test.Admin.Api.Queries.When_Retrieving_LocatieZonderAdresMatch.Projecting;
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Queries/When_Retrieving_LocatieZonderAdresMatch/Projecting/Given_AdresKonNietOvergenomenWordenUitAdressenregister.cs

using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AutoFixture;
<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Projections/When_Retrieving_LocatieZonderAdresMatch/Projector/Given_AdresKonNietOvergenomenWordenUitAdressenregister.cs
using Events;
using FluentAssertions;
using Framework.Fixtures;
using Grar;
========
using FluentAssertions;
using Framework.Fixtures;
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Queries/When_Retrieving_LocatieZonderAdresMatch/Projecting/Given_AdresKonNietOvergenomenWordenUitAdressenregister.cs
using Marten;
using Xunit;

[Collection(nameof(MultiStreamTestCollection))]
public class Given_AdresKonNietOvergenomenWordenUitAdressenregister : IClassFixture<
    GivenAdresKonNietOvergenomenWordenUitAdressenregisterFixture>
{
    private readonly GivenAdresKonNietOvergenomenWordenUitAdressenregisterFixture _fixture;

    public Given_AdresKonNietOvergenomenWordenUitAdressenregister(GivenAdresKonNietOvergenomenWordenUitAdressenregisterFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Then_A_Document_Should_Be_Created()
    {
        var session = _fixture.DocumentStore.LightweightSession();
        var docs = await session.Query<LocatieZonderAdresMatchDocument>().ToListAsync();
        docs.Should().NotBeEmpty();
    }
}

public class GivenAdresKonNietOvergenomenWordenUitAdressenregisterFixture : MultiStreamTestFixture
{
    public GivenAdresKonNietOvergenomenWordenUitAdressenregisterFixture()
    {
        var vCode = "V9900016";
        var locatie = Fixture.Create<Registratiedata.Locatie>() with { LocatieId = 1 };

        Stream(vCode, new IEvent[]
        {
            Fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with { VCode = vCode, Locaties = new[] { locatie } },
            Fixture.Create<AdresKonNietOvergenomenWordenUitAdressenregister>() with
            {
                VCode = vCode, LocatieId = locatie.LocatieId, Reden = GrarClient.OtherNonSuccessStatusCodeMessage,
            },
        });
    }
}
