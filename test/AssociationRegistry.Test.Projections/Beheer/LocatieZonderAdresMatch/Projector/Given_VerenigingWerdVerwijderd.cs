namespace AssociationRegistry.Test.Projections.Beheer.LocatieZonderAdresMatch.Projector;

using Admin.Schema.Detail;
using AutoFixture;
using Events;
using Framework.Fixtures;
using Marten;

[Collection(nameof(MultiStreamTestCollection))]
public class Given_VerenigingWerdVerwijderd : IClassFixture<GivenVerenigingWerdVerwijderdFixture>
{
    private readonly GivenVerenigingWerdVerwijderdFixture _fixture;

    public Given_VerenigingWerdVerwijderd(GivenVerenigingWerdVerwijderdFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Then_A_Document_Should_Not_Contain_LocationId()
    {
        var session = _fixture.DocumentStore.LightweightSession();

        var doc = await session.Query<LocatieZonderAdresMatchDocument>()
                               .FirstOrDefaultAsync(d => d.VCode == "V9900004");

        doc.Should().BeNull();
    }
}

public class GivenVerenigingWerdVerwijderdFixture : MultiStreamTestFixture
{
    public GivenVerenigingWerdVerwijderdFixture()
    {
        var vCode = "V9900004";

        Stream(vCode, new IEvent[]
        {
            Fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                VCode = vCode,
                Locaties = new[]
                {
                    Fixture.Create<Registratiedata.Locatie>() with { LocatieId = 1 },
                    Fixture.Create<Registratiedata.Locatie>() with { LocatieId = 2 },
                    Fixture.Create<Registratiedata.Locatie>() with { LocatieId = 3 },
                },
            },
            Fixture.Create<VerenigingWerdVerwijderd>() with { VCode = vCode },
        });
    }
}
