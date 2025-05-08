namespace AssociationRegistry.Test.Projections.Beheer.LocatieZonderAdresMatch.Projector;

using Admin.Schema.Detail;
using AutoFixture;
using Events;
using Framework.Fixtures;
using Marten;
using Vereniging;

[Collection(nameof(MultiStreamTestCollection))]
public class Given_LocatieWerdToegevoegd_MaatschappelijkeZetelVolgensKbo : IClassFixture<
    GivenLocatieWerdToegevoegdMaatschappelijkeZetelFixture>
{
    private readonly GivenLocatieWerdToegevoegdMaatschappelijkeZetelFixture _fixture;

    public Given_LocatieWerdToegevoegd_MaatschappelijkeZetelVolgensKbo(GivenLocatieWerdToegevoegdMaatschappelijkeZetelFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async ValueTask Then_A_Document_Should_Not_Contain_LocationId()
    {
        var session = _fixture.DocumentStore.LightweightSession();

        var doc = await session.Query<LocatieZonderAdresMatchDocument>()
                               .FirstOrDefaultAsync(d => d.VCode == "V9900099");

        doc.Should().NotBeNull();
        doc!.LocatieIds.Should().NotContain(1);
    }
}

public class GivenLocatieWerdToegevoegdMaatschappelijkeZetelFixture : MultiStreamTestFixture
{
    public GivenLocatieWerdToegevoegdMaatschappelijkeZetelFixture()
    {
        var vCode = "V9900099";

        var locatie = Fixture.Create<Registratiedata.Locatie>() with
        {
            LocatieId = 1, Locatietype = Locatietype.MaatschappelijkeZetelVolgensKbo.Waarde,
        };

        Stream(vCode, new IEvent[]
        {
            Fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                VCode = vCode, Locaties = Array.Empty<Registratiedata.Locatie>(),
            },
            Fixture.Create<LocatieWerdToegevoegd>() with { Locatie = locatie },
        });
    }
}
