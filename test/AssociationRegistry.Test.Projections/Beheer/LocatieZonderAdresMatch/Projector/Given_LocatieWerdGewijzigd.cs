namespace AssociationRegistry.Test.Projections.Beheer.LocatieZonderAdresMatch.Projector;

using Admin.Schema.Detail;
using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using FluentAssertions;
using Framework.Fixtures;
using Marten;
using Vereniging;
using Xunit;

[Collection(nameof(MultiStreamTestCollection))]
public class Given_LocatieWerdGewijzigd : IClassFixture<GivenLocatieWerdGewijzigdFixture>
{
    private readonly GivenLocatieWerdGewijzigdFixture _fixture;

    public Given_LocatieWerdGewijzigd(GivenLocatieWerdGewijzigdFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Then_A_Document_Should_Be_Created()
    {
        var session = _fixture.DocumentStore.LightweightSession();
        var docs = await session.Query<LocatieZonderAdresMatchDocument>().ToListAsync();
        docs.Should().NotBeEmpty();
        docs.Should().HaveCount(1);
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
            Fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
            {
                VCode = vCode, Locaties = Array.Empty<Registratiedata.Locatie>(),
            },
            Fixture.Create<LocatieWerdGewijzigd>() with { Locatie = locatie },
        });
    }
}
