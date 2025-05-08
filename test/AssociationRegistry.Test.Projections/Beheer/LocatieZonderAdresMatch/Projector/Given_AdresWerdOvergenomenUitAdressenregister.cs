namespace AssociationRegistry.Test.Projections.Beheer.LocatieZonderAdresMatch.Projector;

using Admin.Schema.Detail;
using AutoFixture;
using Events;
using Framework.Fixtures;
using Marten;

[Collection(nameof(MultiStreamTestCollection))]
public class Given_AdresWerdOvergenomenUitAdressenregister : IClassFixture<GivenAdresWerdOvergenomenUitAdressenregisterFixture>
{
    private readonly GivenAdresWerdOvergenomenUitAdressenregisterFixture _fixture;

    public Given_AdresWerdOvergenomenUitAdressenregister(GivenAdresWerdOvergenomenUitAdressenregisterFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async ValueTask Then_A_Document_Should_Not_Contain_LocationId()
    {
        var session = _fixture.DocumentStore.LightweightSession();

        var doc = await session.Query<LocatieZonderAdresMatchDocument>()
                               .FirstOrDefaultAsync(d => d.VCode == "V9900014");

        doc.Should().NotBeNull();

        doc!.LocatieIds.Should().NotContain(1);
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
            Fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                VCode = vCode, Locaties = new[] { locatie },
            },
            Fixture.Create<AdresWerdOvergenomenUitAdressenregister>() with { LocatieId = locatie.LocatieId },
        });
    }
}
