namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_LocatieZonderAdresMatch.Projecting;

using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using Fixtures;
using FluentAssertions;
using Marten;
using Xunit;

[Collection(nameof(MultiStreamTestCollection))]
public class Given_AdresWerdOntkoppeldVanAdressenregister : IClassFixture<GivenAdresWerdOntkoppeldVanAdressenregisterFixture>
{
    private readonly GivenAdresWerdOntkoppeldVanAdressenregisterFixture _fixture;

    public Given_AdresWerdOntkoppeldVanAdressenregister(GivenAdresWerdOntkoppeldVanAdressenregisterFixture fixture)
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

public class GivenAdresWerdOntkoppeldVanAdressenregisterFixture : MultiStreamTestFixture
{
    public GivenAdresWerdOntkoppeldVanAdressenregisterFixture()
    {
        var vCode = "V9900013";
        var locatie = Fixture.Create<Registratiedata.Locatie>() with { LocatieId = 1 };

        Stream(vCode, new IEvent[]
        {
            Fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with { VCode = vCode, Locaties = new[] { locatie } },
            Fixture.Create<AdresWerdOntkoppeldVanAdressenregister>() with { LocatieId = locatie.LocatieId },
        });
    }
}
