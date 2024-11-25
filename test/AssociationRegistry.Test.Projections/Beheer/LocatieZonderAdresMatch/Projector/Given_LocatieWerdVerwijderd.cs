namespace AssociationRegistry.Test.Projections.Beheer.LocatieZonderAdresMatch.Projector;

using Admin.Schema.Detail;
using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using FluentAssertions;
using Framework.Fixtures;
using Marten;
using Xunit;

[Collection(nameof(MultiStreamTestCollection))]
public class Given_LocatieWerdVerwijderd : IClassFixture<GivenLocatieWerdVerwijderdFixture>
{
    private readonly GivenLocatieWerdVerwijderdFixture _fixture;

    public Given_LocatieWerdVerwijderd(GivenLocatieWerdVerwijderdFixture fixture)
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

public class GivenLocatieWerdVerwijderdFixture : MultiStreamTestFixture
{
    public GivenLocatieWerdVerwijderdFixture()
    {
        var vCode = "V9900003";
        var locatie = Fixture.Create<Registratiedata.Locatie>() with { LocatieId = 1 };

        Stream(vCode, new IEvent[]
        {
            Fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with { VCode = vCode, Locaties = new[] { locatie } },
            Fixture.Create<LocatieWerdVerwijderd>() with { VCode = vCode, Locatie = locatie },
        });
    }
}
