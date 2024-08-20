<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Projections/When_Retrieving_LocatieZonderAdresMatch/Projector/Given_AdresNietUniekInAdressenregister.cs
﻿namespace AssociationRegistry.Test.Admin.Api.Projections.When_Retrieving_LocatieZonderAdresMatch.Projector;
========
﻿namespace AssociationRegistry.Test.Admin.Api.Queries.When_Retrieving_LocatieZonderAdresMatch.Projecting;
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Queries/When_Retrieving_LocatieZonderAdresMatch/Projecting/Given_AdresNietUniekInAdressenregister.cs

using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AutoFixture;
<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Projections/When_Retrieving_LocatieZonderAdresMatch/Projector/Given_AdresNietUniekInAdressenregister.cs
using Events;
========
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Queries/When_Retrieving_LocatieZonderAdresMatch/Projecting/Given_AdresNietUniekInAdressenregister.cs
using FluentAssertions;
using Framework.Fixtures;
using Marten;
using Xunit;

[Collection(nameof(MultiStreamTestCollection))]
public class Given_AdresNietUniekInAdressenregister : IClassFixture<GivenAdresNietUniekInAdressenregisterFixture>
{
    private readonly GivenAdresNietUniekInAdressenregisterFixture _fixture;

    public Given_AdresNietUniekInAdressenregister(GivenAdresNietUniekInAdressenregisterFixture fixture)
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

public class GivenAdresNietUniekInAdressenregisterFixture : MultiStreamTestFixture
{
    public GivenAdresNietUniekInAdressenregisterFixture()
    {
        var vCode = "V9900011";
        var locatie = Fixture.Create<Registratiedata.Locatie>() with { LocatieId = 1 };

        Stream(vCode, new IEvent[]
        {
            Fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with { VCode = vCode, Locaties = new[] { locatie } },
            Fixture.Create<AdresNietUniekInAdressenregister>() with { LocatieId = locatie.LocatieId },
        });
    }
}
