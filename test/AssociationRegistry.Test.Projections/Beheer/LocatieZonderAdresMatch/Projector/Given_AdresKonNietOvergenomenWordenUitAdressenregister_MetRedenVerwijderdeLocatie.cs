namespace AssociationRegistry.Test.Projections.Beheer.LocatieZonderAdresMatch.Projector;

using Admin.Schema.Detail;
using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using Framework.Fixtures;
using Marten;
using System.Threading.Tasks;

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
