namespace AssociationRegistry.Test.Admin.Api.Queries.When_Retrieving_LocatieZonderAdresMatch.Projecting;

using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AutoFixture;
using FluentAssertions;
using Framework.Fixtures;
using Marten;
using Xunit;

[Collection(nameof(MultiStreamTestCollection))]
public class Given_VerenigingWerdVerwijderd : IClassFixture<GivenVerenigingWerdVerwijderdFixture>
{
    private readonly GivenVerenigingWerdVerwijderdFixture _fixture;

    public Given_VerenigingWerdVerwijderd(GivenVerenigingWerdVerwijderdFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Then_Multiple_Documents_Should_Be_Deleted()
    {
        var session = _fixture.DocumentStore.LightweightSession();
        var docs = await session.Query<LocatieZonderAdresMatchDocument>().ToListAsync();
        docs.Should().BeEmpty();
    }
}

public class GivenVerenigingWerdVerwijderdFixture : MultiStreamTestFixture
{
    public GivenVerenigingWerdVerwijderdFixture()
    {
        var vCode = "V9900004";

        Stream(vCode, new IEvent[]
        {
            Fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
            {
                VCode = vCode,
                Locaties = new[]
                {
                    Fixture.Create<Registratiedata.Locatie>() with { LocatieId = 1 },
                    Fixture.Create<Registratiedata.Locatie>() with { LocatieId = 2 },
                    Fixture.Create<Registratiedata.Locatie>() with { LocatieId = 3 },
                },
            },
            Fixture.Create<VerenigingWerdVerwijderd>() with { VCode = vCode },
        });
    }
}
