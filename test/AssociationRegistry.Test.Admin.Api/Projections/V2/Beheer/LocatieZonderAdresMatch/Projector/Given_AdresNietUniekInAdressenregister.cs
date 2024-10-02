namespace AssociationRegistry.Test.Admin.Api.Projections.V2.Beheer.LocatieZonderAdresMatch.Projector;

using AssociationRegistry.Admin.Schema.Detail;
using Events;
using AssociationRegistry.Framework;
using Framework.Fixtures;
using AutoFixture;
using FluentAssertions;
using Marten;
using Xunit;

[Collection(nameof(MultiStreamTestCollection))]
public class Given_AdresNietUniekInAdressenregister : IClassFixture<GivenAdresNietUniekInAdressenregisterFixture>
{
    private readonly GivenAdresNietUniekInAdressenregisterFixture _fixture;

    public Given_AdresNietUniekInAdressenregister(GivenAdresNietUniekInAdressenregisterFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Then_A_Document_Should_Be_Created()
    {
        var session = _fixture.DocumentStore.LightweightSession();
        var docs = await session.Query<LocatieZonderAdresMatchDocument>().ToListAsync();
        docs.Should().BeEmpty();
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
            Fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with { VCode = vCode, Locaties = new[] { locatie } },
            Fixture.Create<AdresNietUniekInAdressenregister>() with { LocatieId = locatie.LocatieId },
        });
    }
}
