namespace AssociationRegistry.Test.Projections.Beheer.LocatieZonderAdresMatch.Projector;

using Admin.Schema.Detail;
using AutoFixture;
using Events;
using Framework.Fixtures;
using Marten;

[Collection(nameof(MultiStreamTestCollection))]
public class Given_LocatieDuplicaatWerdVerwijderdNaAdresMatch : IClassFixture<GivenLocatieDuplicaatWerdVerwijderdNaAdresMatchFixture>
{
    private readonly GivenLocatieDuplicaatWerdVerwijderdNaAdresMatchFixture _fixture;

    public Given_LocatieDuplicaatWerdVerwijderdNaAdresMatch(GivenLocatieDuplicaatWerdVerwijderdNaAdresMatchFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async ValueTask Then_A_Document_Should_Not_Contain_LocationId()
    {
        var session = _fixture.DocumentStore.LightweightSession();

        var doc = await session.Query<LocatieZonderAdresMatchDocument>()
                               .FirstOrDefaultAsync(d => d.VCode == "V9900015");

        if (doc != null)
            doc!.LocatieIds.Should().NotContain(1);
    }
}

public class GivenLocatieDuplicaatWerdVerwijderdNaAdresMatchFixture : MultiStreamTestFixture
{
    public GivenLocatieDuplicaatWerdVerwijderdNaAdresMatchFixture()
    {
        var vCode = "V9900015";
        var locatie = Fixture.Create<Registratiedata.Locatie>() with { LocatieId = 1 };

        Stream(vCode, new IEvent[]
        {
            Fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                VCode = vCode, Locaties = new[] { locatie },
            },
            Fixture.Create<LocatieDuplicaatWerdVerwijderdNaAdresMatch>() with { VerwijderdeLocatieId = locatie.LocatieId },
        });
    }
}
