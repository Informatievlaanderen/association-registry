<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Projections/When_Retrieving_LocatieZonderAdresMatch/Projector/Given_FeitelijkeVerenigingWerdGeregistreerd.cs
﻿namespace AssociationRegistry.Test.Admin.Api.Projections.When_Retrieving_LocatieZonderAdresMatch.Projector;
========
﻿namespace AssociationRegistry.Test.Admin.Api.Queries.When_Retrieving_LocatieZonderAdresMatch.Projecting;
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Queries/When_Retrieving_LocatieZonderAdresMatch/Projecting/Given_FeitelijkeVerenigingWerdGeregistreerd.cs

using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Events;
using AssociationRegistry.Vereniging;
using AutoFixture;
<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Projections/When_Retrieving_LocatieZonderAdresMatch/Projector/Given_FeitelijkeVerenigingWerdGeregistreerd.cs
using Events;
========
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Queries/When_Retrieving_LocatieZonderAdresMatch/Projecting/Given_FeitelijkeVerenigingWerdGeregistreerd.cs
using FluentAssertions;
using Framework.Fixtures;
using Marten;
using Xunit;

[Collection(nameof(MultiStreamTestCollection))]
public class Given_FeitelijkeVerenigingWerdGeregistreerd : IClassFixture<GivenFeitelijkeVerenigingWerdGeregistreerdFixture>
{
    private readonly GivenFeitelijkeVerenigingWerdGeregistreerdFixture _fixture;

    public Given_FeitelijkeVerenigingWerdGeregistreerd(GivenFeitelijkeVerenigingWerdGeregistreerdFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Then_Multiple_Documents_Should_Be_Created()
    {
        var session = _fixture.DocumentStore.LightweightSession();
        var docs = await session.Query<LocatieZonderAdresMatchDocument>().ToListAsync();
        docs.Should().NotBeEmpty();
        docs.Count().Should().Be(1);
    }
}

public class GivenFeitelijkeVerenigingWerdGeregistreerdFixture : MultiStreamTestFixture
{
    public GivenFeitelijkeVerenigingWerdGeregistreerdFixture()
    {
        var vCode = "V9900001";

        Stream(vCode, new[]
        {
            Fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
            {
                VCode = vCode,
                Locaties = new[]
                {
                    Fixture.Create<Registratiedata.Locatie>() with
                    {
                        LocatieId = 1,
                        Locatietype = Locatietype.Activiteiten.Waarde,
                        Adres = null,
                    },
                    Fixture.Create<Registratiedata.Locatie>() with
                    {
                        LocatieId = 2,
                        Locatietype = Locatietype.MaatschappelijkeZetelVolgensKbo.Waarde,
                    },
                    Fixture.Create<Registratiedata.Locatie>()with
                    {
                        LocatieId = 3,
                        Locatietype = Locatietype.Activiteiten.Waarde,
                    },
                },
            },
        });
    }
}
