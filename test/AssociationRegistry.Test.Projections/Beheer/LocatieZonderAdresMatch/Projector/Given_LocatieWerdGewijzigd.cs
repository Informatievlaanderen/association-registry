namespace AssociationRegistry.Test.Projections.Beheer.LocatieZonderAdresMatch.Projector;

using Admin.Schema.Detail;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;
using Framework.Fixtures;
using Marten;
using Vereniging;

[Collection(nameof(MultiStreamTestCollection))]
public class Given_LocatieWerdGewijzigd : IClassFixture<GivenLocatieWerdGewijzigdFixture>
{
    private readonly GivenLocatieWerdGewijzigdFixture _fixture;

    public Given_LocatieWerdGewijzigd(GivenLocatieWerdGewijzigdFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async ValueTask Then_LocatieId_Should_Be_Added_To_Document()
    {
        var session = _fixture.DocumentStore.LightweightSession();

        var doc = await session.Query<LocatieZonderAdresMatchDocument>()
                               .SingleAsync(d => d.VCode == "V9900002");

        doc.Should().NotBeNull();

        doc!.LocatieIds.Should().Contain(1);
    }
}

public class GivenLocatieWerdGewijzigdFixture : MultiStreamTestFixture
{
    public GivenLocatieWerdGewijzigdFixture()
    {
        var vCode = "V9900002";
        var locatie = Fixture.Create<Registratiedata.Locatie>() with { LocatieId = 1, Locatietype = Locatietype.Activiteiten.Waarde };

        Stream(vCode, new IEvent[]
        {
            Fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                VCode = vCode, Locaties = Array.Empty<Registratiedata.Locatie>(),
            },
            Fixture.Create<LocatieWerdGewijzigd>() with { Locatie = locatie },
        });
    }
}
