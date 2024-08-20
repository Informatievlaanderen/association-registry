<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Projections/When_Retrieving_LocatieZonderAdresMatch/Projector/Given_AdresWerdOvergenomenUitAdressenregister.cs
﻿namespace AssociationRegistry.Test.Admin.Api.Projections.When_Retrieving_LocatieZonderAdresMatch.Projector;
========
﻿namespace AssociationRegistry.Test.Admin.Api.Queries.When_Retrieving_LocatieZonderAdresMatch.Projecting;
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Queries/When_Retrieving_LocatieZonderAdresMatch/Projecting/Given_AdresWerdOvergenomenUitAdressenregister.cs

using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AutoFixture;
<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Projections/When_Retrieving_LocatieZonderAdresMatch/Projector/Given_AdresWerdOvergenomenUitAdressenregister.cs
using Events;
========
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Queries/When_Retrieving_LocatieZonderAdresMatch/Projecting/Given_AdresWerdOvergenomenUitAdressenregister.cs
using FluentAssertions;
using Framework.Fixtures;
using Marten;
using Xunit;

[Collection(nameof(MultiStreamTestCollection))]
public class Given_AdresWerdOvergenomenUitAdressenregister : IClassFixture<GivenAdresWerdOvergenomenUitAdressenregisterFixture>
{
    private readonly GivenAdresWerdOvergenomenUitAdressenregisterFixture _fixture;

    public Given_AdresWerdOvergenomenUitAdressenregister(GivenAdresWerdOvergenomenUitAdressenregisterFixture fixture)
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

public class GivenAdresWerdOvergenomenUitAdressenregisterFixture : MultiStreamTestFixture
{
    public GivenAdresWerdOvergenomenUitAdressenregisterFixture()
    {
        var vCode = "V9900014";
        var locatie = Fixture.Create<Registratiedata.Locatie>() with { LocatieId = 1 };

        Stream(vCode, new IEvent[]
        {
            Fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with { VCode = vCode, Locaties = new[] { locatie } },
            Fixture.Create<AdresWerdOvergenomenUitAdressenregister>() with { LocatieId = locatie.LocatieId },
        });
    }
}
