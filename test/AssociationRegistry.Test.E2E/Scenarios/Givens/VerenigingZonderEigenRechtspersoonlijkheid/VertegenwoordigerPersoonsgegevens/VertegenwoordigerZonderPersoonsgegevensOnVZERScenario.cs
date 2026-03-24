namespace AssociationRegistry.Test.E2E.Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid.VertegenwoordigerPersoonsgegevens;

using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;
using EventStore;
using Framework.TestClasses;

public class VertegenwoordigerZonderPersoonsgegevensOnVZERScenario : IScenario
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens { get; set; }
    public VertegenwoordigerWerdToegevoegdZonderPersoonsgegevens VertegenwoordigerWerdToegevoegdZonderPersoonsgegevens { get; set; }
    public VertegenwoordigerWerdGewijzigdZonderPersoonsgegevens VertegenwoordigerWerdGewijzigdZonderPersoonsgegevens { get; set; }
    public VertegenwoordigerWerdVerwijderdZonderPersoonsgegevens VertegenwoordigerWerdVerwijderdZonderPersoonsgegevens { get; set; }

    public VertegenwoordigerWerdToegevoegdZonderPersoonsgegevens VertegenwoordigerWerdToegevoegdVoorKszNietGekend { get; set; }
    public KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekendZonderPersoonsgegevens KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekendZonderPersoonsgegevens { get; set; }
    public VertegenwoordigerWerdToegevoegdZonderPersoonsgegevens VertegenwoordigerWerdToegevoegdVoorKszOverledenZonderPersoonsgegevens { get; set; }

    public KszSyncHeeftVertegenwoordigerAangeduidAlsOverledenZonderPersoonsgegevens KszSyncHeeftVertegenwoordigerAangeduidAlsOverledenZonderPersoonsgegevens { get; set; }

    private CommandMetadata Metadata;

    public VertegenwoordigerZonderPersoonsgegevensOnVZERScenario()
    {
    }

    public async Task<KeyValuePair<string, IEvent[]>[]> GivenEvents(IVCodeService service)
    {
        var fixture = new Fixture().CustomizeAdminApi();


        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens = fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens>() with
        {
            VCode = await service.GetNext(),
            Bankrekeningnummers = [],
        };

        VertegenwoordigerWerdToegevoegdZonderPersoonsgegevens = fixture.Create<VertegenwoordigerWerdToegevoegdZonderPersoonsgegevens>();

        VertegenwoordigerWerdGewijzigdZonderPersoonsgegevens = fixture.Create<VertegenwoordigerWerdGewijzigdZonderPersoonsgegevens>() with
        {
            VertegenwoordigerId = VertegenwoordigerWerdToegevoegdZonderPersoonsgegevens.VertegenwoordigerId,
        };

        VertegenwoordigerWerdVerwijderdZonderPersoonsgegevens = fixture.Create<VertegenwoordigerWerdVerwijderdZonderPersoonsgegevens>() with
        {
            VertegenwoordigerId = VertegenwoordigerWerdToegevoegdZonderPersoonsgegevens.VertegenwoordigerId,
        };

        VertegenwoordigerWerdToegevoegdVoorKszNietGekend = fixture.Create<VertegenwoordigerWerdToegevoegdZonderPersoonsgegevens>();
        KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekendZonderPersoonsgegevens = fixture.Create<KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekendZonderPersoonsgegevens>() with
        {
            VertegenwoordigerId = VertegenwoordigerWerdToegevoegdVoorKszNietGekend.VertegenwoordigerId,
        };

        VertegenwoordigerWerdToegevoegdVoorKszOverledenZonderPersoonsgegevens = fixture.Create<VertegenwoordigerWerdToegevoegdZonderPersoonsgegevens>();
        KszSyncHeeftVertegenwoordigerAangeduidAlsOverledenZonderPersoonsgegevens = fixture.Create<KszSyncHeeftVertegenwoordigerAangeduidAlsOverledenZonderPersoonsgegevens>() with
        {
            VertegenwoordigerId = VertegenwoordigerWerdToegevoegdVoorKszOverledenZonderPersoonsgegevens.VertegenwoordigerId,
        };


        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };

        return
        [
            new(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens.VCode,
            [
                VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens,
                VertegenwoordigerWerdToegevoegdZonderPersoonsgegevens,
                VertegenwoordigerWerdGewijzigdZonderPersoonsgegevens,
                VertegenwoordigerWerdVerwijderdZonderPersoonsgegevens,
                VertegenwoordigerWerdToegevoegdVoorKszNietGekend,
                KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekendZonderPersoonsgegevens,
                VertegenwoordigerWerdToegevoegdVoorKszOverledenZonderPersoonsgegevens,
                KszSyncHeeftVertegenwoordigerAangeduidAlsOverledenZonderPersoonsgegevens,
            ]),
        ];
    }

    public StreamActionResult Result { get; set; } = null!;

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
