namespace AssociationRegistry.Test.Projections.Beheer.LocatieZonderAdresMatch.Projector;

using Admin.Schema.Detail;
using AutoFixture;
using Events;
using Framework.Fixtures;
using Marten;

[Collection(nameof(MultiStreamTestCollection))]
public class Given_AdresNietUniekInAdressenregister : IClassFixture<GivenAdresNietUniekInAdressenregisterFixture>
{
    private readonly GivenAdresNietUniekInAdressenregisterFixture _fixture;

    public Given_AdresNietUniekInAdressenregister(GivenAdresNietUniekInAdressenregisterFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async ValueTask Then_A_Document_Should_Not_Contain_LocationId()
    {
        var session = _fixture.DocumentStore.LightweightSession();

        var doc = await session.Query<LocatieZonderAdresMatchDocument>()
                               .FirstOrDefaultAsync(d => d.VCode == "V9900011");

        doc.Should().NotBeNull();
        doc!.LocatieIds.Should().NotContain(1);
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
            Fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                VCode = vCode, Locaties = new[] { locatie },
            },
            Fixture.Create<AdresNietUniekInAdressenregister>() with { LocatieId = locatie.LocatieId },
        });
    }
}
