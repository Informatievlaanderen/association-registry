namespace AssociationRegistry.Test.Projections.Beheer.LocatieZonderAdresMatch.Projector;

using Admin.Schema.Detail;
using AutoFixture;
using Events;
using Framework.Fixtures;
using Marten;

[Collection(nameof(MultiStreamTestCollection))]
public class Given_AdresWerdOntkoppeldVanAdressenregister : IClassFixture<GivenAdresWerdOntkoppeldVanAdressenregisterFixture>
{
    private readonly GivenAdresWerdOntkoppeldVanAdressenregisterFixture _fixture;

    public Given_AdresWerdOntkoppeldVanAdressenregister(GivenAdresWerdOntkoppeldVanAdressenregisterFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async ValueTask Then_A_Document_Should_Not_Contain_LocationId()
    {
        var session = _fixture.DocumentStore.LightweightSession();

        var doc = await session.Query<LocatieZonderAdresMatchDocument>()
                               .FirstOrDefaultAsync(d => d.VCode == "V9900013");

        doc.Should().NotBeNull();

        doc!.LocatieIds.Should().NotContain(1);
    }
}

public class GivenAdresWerdOntkoppeldVanAdressenregisterFixture : MultiStreamTestFixture
{
    public GivenAdresWerdOntkoppeldVanAdressenregisterFixture()
    {
        var vCode = "V9900013";
        var locatie = Fixture.Create<Registratiedata.Locatie>() with { LocatieId = 1 };

        Stream(vCode, new IEvent[]
        {
            Fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                VCode = vCode, Locaties = new[] { locatie },
            },
            Fixture.Create<AdresWerdOntkoppeldVanAdressenregister>() with { LocatieId = locatie.LocatieId },
        });
    }
}
