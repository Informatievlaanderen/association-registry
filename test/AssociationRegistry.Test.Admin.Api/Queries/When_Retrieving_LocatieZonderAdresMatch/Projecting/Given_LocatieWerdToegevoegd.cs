<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Projections/When_Retrieving_LocatieZonderAdresMatch/Projector/Given_LocatieWerdToegevoegd.cs
﻿namespace AssociationRegistry.Test.Admin.Api.Projections.When_Retrieving_LocatieZonderAdresMatch.Projector;
========
﻿namespace AssociationRegistry.Test.Admin.Api.Queries.When_Retrieving_LocatieZonderAdresMatch.Projecting;
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Queries/When_Retrieving_LocatieZonderAdresMatch/Projecting/Given_LocatieWerdToegevoegd.cs

using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Vereniging;
using AutoFixture;
<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Projections/When_Retrieving_LocatieZonderAdresMatch/Projector/Given_LocatieWerdToegevoegd.cs
using Events;
========
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Queries/When_Retrieving_LocatieZonderAdresMatch/Projecting/Given_LocatieWerdToegevoegd.cs
using FluentAssertions;
using Framework.Fixtures;
using Marten;
using Xunit;

[Collection(nameof(MultiStreamTestCollection))]
public class Given_LocatieWerdToegevoegd : IClassFixture<GivenLocatieWerdToegevoegdFixture>
{
    private readonly GivenLocatieWerdToegevoegdFixture _fixture;

    public Given_LocatieWerdToegevoegd(GivenLocatieWerdToegevoegdFixture fixture)
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

public class GivenLocatieWerdToegevoegdFixture : MultiStreamTestFixture
{
    public GivenLocatieWerdToegevoegdFixture()
    {
        var vCode = "V9900002";
        var locatie = Fixture.Create<Registratiedata.Locatie>() with { LocatieId = 1, Locatietype = Locatietype.Activiteiten.Waarde };

        Stream(vCode, new IEvent[]
        {
            Fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
            {
                VCode = vCode, Locaties = Array.Empty<Registratiedata.Locatie>(),
            },
            Fixture.Create<LocatieWerdToegevoegd>() with { Locatie = locatie },
        });
    }
}
