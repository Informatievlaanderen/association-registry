namespace AssociationRegistry.Test.Admin.Api.Queries.When_Retrieving_LocatieZonderAdresMatch.Projecting;

using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Vereniging;
using AutoFixture;
using FluentAssertions;
using Framework.Fixtures;
using Marten;
using Xunit;

[Collection(nameof(MultiStreamTestCollection))]
public class Given_LocatieWerdToegevoegd_MaatschappelijkeZetelVolgensKbo : IClassFixture<GivenLocatieWerdToegevoegdMaatschappelijkeZetelFixture>
{
    private readonly GivenLocatieWerdToegevoegdMaatschappelijkeZetelFixture _fixture;

    public Given_LocatieWerdToegevoegd_MaatschappelijkeZetelVolgensKbo(GivenLocatieWerdToegevoegdMaatschappelijkeZetelFixture fixture)
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

public class GivenLocatieWerdToegevoegdMaatschappelijkeZetelFixture : MultiStreamTestFixture
{
    public GivenLocatieWerdToegevoegdMaatschappelijkeZetelFixture()
    {
        var vCode = "V9900099";
        var locatie = Fixture.Create<Registratiedata.Locatie>() with { LocatieId = 1, Locatietype = Locatietype.MaatschappelijkeZetelVolgensKbo.Waarde };

        Stream(vCode, new IEvent[]
        {
            Fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with { VCode = vCode, Locaties = Array.Empty<Registratiedata.Locatie>()},
            Fixture.Create<LocatieWerdToegevoegd>() with { Locatie = locatie},
        });
    }
}
