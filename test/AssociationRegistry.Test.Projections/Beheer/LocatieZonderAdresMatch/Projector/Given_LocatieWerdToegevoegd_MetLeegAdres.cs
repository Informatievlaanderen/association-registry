namespace AssociationRegistry.Test.Projections.Beheer.LocatieZonderAdresMatch.Projector;

using Admin.Schema.Detail;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;
using Framework.Fixtures;
using Marten;
using Vereniging;

[Collection(nameof(MultiStreamTestCollection))]
public class Given_LocatieWerdToegevoegd_MetLeegAdres : IClassFixture<GivenLocatieWerdToegevoegdMetLeegAdresFixture>
{
    private readonly GivenLocatieWerdToegevoegdMetLeegAdresFixture _fixture;

    public Given_LocatieWerdToegevoegd_MetLeegAdres(GivenLocatieWerdToegevoegdMetLeegAdresFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async ValueTask Then_A_Document_Should_Not_Contain_LocationId()
    {
        var session = _fixture.DocumentStore.LightweightSession();

        var doc = await session.Query<LocatieZonderAdresMatchDocument>()
                               .FirstOrDefaultAsync(d => d.VCode == "V9900100");

        doc.Should().NotBeNull();
        doc!.LocatieIds.Should().NotContain(1);
    }
}

public class GivenLocatieWerdToegevoegdMetLeegAdresFixture : MultiStreamTestFixture
{
    public GivenLocatieWerdToegevoegdMetLeegAdresFixture()
    {
        var vCode = "V9900100";

        var locatie = Fixture.Create<Registratiedata.Locatie>() with
        {
            LocatieId = 1, Locatietype = Locatietype.Activiteiten, Adres = null,
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
