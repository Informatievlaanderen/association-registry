namespace AssociationRegistry.Test.Projections.Beheer.LocatieZonderAdresMatch.Projector;

using Admin.Schema.Detail;
using AutoFixture;
using Events;
using Framework.Fixtures;
using Marten;
using Vereniging;

[Collection(nameof(MultiStreamTestCollection))]
public class Given_VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd : IClassFixture<
    GivenVerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdFixture>
{
    private readonly GivenVerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdFixture _fixture;

    public Given_VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd(
        GivenVerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async ValueTask Then_A_Document_Should_Not_Contain_LocationId()
    {
        var session = _fixture.DocumentStore.LightweightSession();

        var doc = await session.Query<LocatieZonderAdresMatchDocument>()
                               .FirstOrDefaultAsync(d => d.VCode == "V9900001");

        doc.Should().NotBeNull();
        doc!.LocatieIds.Should().Contain(1);
    }
}

public class GivenVerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdFixture : MultiStreamTestFixture
{
    public GivenVerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdFixture()
    {
        var vCode = "V9900001";

        Stream(vCode, new[]
        {
            Fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                VCode = vCode,
                Locaties = new[]
                {
                    Fixture.Create<Registratiedata.Locatie>() with
                    {
                        LocatieId = 1,
                        Locatietype = Locatietype.Activiteiten.Waarde,
                        Adres = null,
                    },
                    Fixture.Create<Registratiedata.Locatie>() with
                    {
                        LocatieId = 2,
                        Locatietype = Locatietype.MaatschappelijkeZetelVolgensKbo.Waarde,
                    },
                    Fixture.Create<Registratiedata.Locatie>()with
                    {
                        LocatieId = 3,
                        Locatietype = Locatietype.Activiteiten.Waarde,
                    },
                },
            },
        });
    }
}
