namespace AssociationRegistry.Test.Projections.Beheer.LocatieZonderAdresMatch.Projector;

using Admin.Schema.Detail;
using AutoFixture;
using Events;
using Framework.Fixtures;
using Grar.Clients;
using Marten;

[Collection(nameof(MultiStreamTestCollection))]
public class Given_AdresKonNietOvergenomenWordenUitAdressenregister : IClassFixture<
    GivenAdresKonNietOvergenomenWordenUitAdressenregisterFixture>
{
    private readonly GivenAdresKonNietOvergenomenWordenUitAdressenregisterFixture _fixture;

    public Given_AdresKonNietOvergenomenWordenUitAdressenregister(GivenAdresKonNietOvergenomenWordenUitAdressenregisterFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Then_A_Document_Should_Be_Created()
    {
        var session = _fixture.DocumentStore.LightweightSession();

        var doc = await session.Query<LocatieZonderAdresMatchDocument>()
                               .FirstOrDefaultAsync(d => d.VCode == "V9900016");

        doc.Should().NotBeNull();
        doc!.LocatieIds.Should().Contain(1);
    }
}

public class GivenAdresKonNietOvergenomenWordenUitAdressenregisterFixture : MultiStreamTestFixture
{
    public GivenAdresKonNietOvergenomenWordenUitAdressenregisterFixture()
    {
        var vCode = "V9900016";
        var locatie = Fixture.Create<Registratiedata.Locatie>() with { LocatieId = 1 };

        Stream(vCode, new IEvent[]
        {
            Fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                VCode = vCode, Locaties = new[] { locatie },
            },
            Fixture.Create<AdresKonNietOvergenomenWordenUitAdressenregister>() with
            {
                VCode = vCode, LocatieId = locatie.LocatieId, Reden = GrarClient.OtherNonSuccessStatusCodeMessage,
            },
        });
    }
}
