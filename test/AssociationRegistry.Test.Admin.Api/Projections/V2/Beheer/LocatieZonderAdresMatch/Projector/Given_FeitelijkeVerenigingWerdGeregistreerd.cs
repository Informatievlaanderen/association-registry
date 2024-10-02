namespace AssociationRegistry.Test.Admin.Api.Projections.V2.Beheer.LocatieZonderAdresMatch.Projector;

using AssociationRegistry.Admin.Schema.Detail;
using Events;
using Framework.Fixtures;
using Vereniging;
using AutoFixture;
using FluentAssertions;
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
