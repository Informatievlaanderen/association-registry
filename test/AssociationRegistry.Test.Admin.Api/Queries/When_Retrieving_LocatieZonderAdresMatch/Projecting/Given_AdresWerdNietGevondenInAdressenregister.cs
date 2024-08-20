<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Projections/When_Retrieving_LocatieZonderAdresMatch/Projector/Given_AdresWerdNietGevondenInAdressenregister.cs
﻿namespace AssociationRegistry.Test.Admin.Api.Projections.When_Retrieving_LocatieZonderAdresMatch.Projector;
========
﻿namespace AssociationRegistry.Test.Admin.Api.Queries.When_Retrieving_LocatieZonderAdresMatch.Projecting;
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Queries/When_Retrieving_LocatieZonderAdresMatch/Projecting/Given_AdresWerdNietGevondenInAdressenregister.cs

using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AutoFixture;
<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Projections/When_Retrieving_LocatieZonderAdresMatch/Projector/Given_AdresWerdNietGevondenInAdressenregister.cs
using Events;
========
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Queries/When_Retrieving_LocatieZonderAdresMatch/Projecting/Given_AdresWerdNietGevondenInAdressenregister.cs
using FluentAssertions;
using Framework.Fixtures;
using Marten;
using Xunit;

[Collection(nameof(MultiStreamTestCollection))]
public class Given_AdresWerdNietGevondenInAdressenregister : IClassFixture<GivenAdresWerdNietGevondenInAdressenregisterFixture>
{
    private readonly GivenAdresWerdNietGevondenInAdressenregisterFixture _fixture;

    public Given_AdresWerdNietGevondenInAdressenregister(GivenAdresWerdNietGevondenInAdressenregisterFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Then_A_Document_Should_Be_Created()
    {
        var session = _fixture.DocumentStore.LightweightSession();
        var docs = await session.Query<LocatieZonderAdresMatchDocument>().ToListAsync();
        docs.Should().BeEmpty();
    }
}

public class GivenAdresWerdNietGevondenInAdressenregisterFixture : MultiStreamTestFixture
{
    public GivenAdresWerdNietGevondenInAdressenregisterFixture()
    {
        var vCode = "V9900012";
        var locatie = Fixture.Create<Registratiedata.Locatie>() with { LocatieId = 1 };

        Stream(vCode, new IEvent[]
        {
            Fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with { VCode = vCode, Locaties = new[] { locatie } },
            Fixture.Create<AdresWerdNietGevondenInAdressenregister>() with { LocatieId = locatie.LocatieId },
        });
    }
}
