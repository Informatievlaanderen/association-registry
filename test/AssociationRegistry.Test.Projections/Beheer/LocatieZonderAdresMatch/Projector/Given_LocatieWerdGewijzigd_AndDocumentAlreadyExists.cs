namespace AssociationRegistry.Test.Projections.Beheer.LocatieZonderAdresMatch.Projector;

using Admin.Schema.Detail;
using AutoFixture;
using Events;
using Framework.Fixtures;
using Marten;
using Vereniging;

[Collection(nameof(MultiStreamTestCollection))]
public class Given_LocatieWerdGewijzigd_AndDocumentAlreadyExists : IClassFixture<GivenLocatieWerdGewijzigdAndDocumentAlreadyExistsFixture>
{
    private readonly GivenLocatieWerdGewijzigdAndDocumentAlreadyExistsFixture _fixture;

    public Given_LocatieWerdGewijzigd_AndDocumentAlreadyExists(GivenLocatieWerdGewijzigdAndDocumentAlreadyExistsFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async ValueTask Then_A_Document_Should_Contain_LocationId()
    {
        var session = _fixture.DocumentStore.LightweightSession();

        var doc = await session.Query<LocatieZonderAdresMatchDocument>()
                               .FirstOrDefaultAsync(d => d.VCode == "V9900002");

        doc.Should().NotBeNull();
        doc!.LocatieIds.Should().Contain(1);
    }
}

public class GivenLocatieWerdGewijzigdAndDocumentAlreadyExistsFixture : MultiStreamTestFixture
{
    public GivenLocatieWerdGewijzigdAndDocumentAlreadyExistsFixture()
    {
        var vCode = "V9900002";
        var locatie = Fixture.Create<Registratiedata.Locatie>() with { LocatieId = 1, Locatietype = Locatietype.Activiteiten.Waarde };

        Stream(vCode, new IEvent[]
        {
            Fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                VCode = vCode, Locaties = Array.Empty<Registratiedata.Locatie>(),
            },
            Fixture.Create<LocatieWerdToegevoegd>() with { Locatie = locatie },
            Fixture.Create<LocatieWerdGewijzigd>() with { Locatie = locatie },
        });
    }
}
