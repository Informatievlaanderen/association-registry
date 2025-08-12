namespace AssociationRegistry.Test.Projections.Beheer.LocatieZonderAdresMatch.Projector;

using Admin.Schema.Detail;
using AutoFixture;
using Events;
using Framework.Fixtures;
using AssociationRegistry.Integrations.Grar.Clients;
using Marten;

[Collection(nameof(MultiStreamTestCollection))]
public class Given_AdresKonNietOvergenomenWordenUitAdressenregister_WithBadRequest : IClassFixture<
    GivenAdresKonNietOvergenomenWordenUitAdressenregisterWithBadRequestFixture>
{
    private readonly GivenAdresKonNietOvergenomenWordenUitAdressenregisterWithBadRequestFixture _fixture;

    public Given_AdresKonNietOvergenomenWordenUitAdressenregister_WithBadRequest(
        GivenAdresKonNietOvergenomenWordenUitAdressenregisterWithBadRequestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async ValueTask Then_A_Document_Should_Not_Contain_LocationId()
    {
        var session = _fixture.DocumentStore.LightweightSession();

        var doc = await session.Query<LocatieZonderAdresMatchDocument>()
                               .FirstOrDefaultAsync(d => d.VCode == "V9900017");

        doc.Should().NotBeNull();
        doc!.LocatieIds.Should().NotContain(1);
    }
}

public class GivenAdresKonNietOvergenomenWordenUitAdressenregisterWithBadRequestFixture : MultiStreamTestFixture
{
    public GivenAdresKonNietOvergenomenWordenUitAdressenregisterWithBadRequestFixture()
    {
        var vCode = "V9900017";
        var locatie = Fixture.Create<Registratiedata.Locatie>() with { LocatieId = 1 };

        Stream(vCode, new IEvent[]
        {
            Fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                VCode = vCode, Locaties = new[] { locatie },
            },
            Fixture.Create<AdresKonNietOvergenomenWordenUitAdressenregister>() with
            {
                VCode = vCode, LocatieId = locatie.LocatieId, Reden = GrarClient.BadRequestSuccessStatusCodeMessage,
            },
        });
    }
}
