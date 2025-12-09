namespace AssociationRegistry.Test.E2E.Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid.VertegenwoordigerPersoonsgegevens;

using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;
using EventStore;
using Framework.TestClasses;

public class VertegenwoordigerPersoonsgegevensOnVZERScenario : IScenario
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; set; }
    public VertegenwoordigerWerdToegevoegd VertegenwoordigerWerdToegevoegd { get; set; }
    public VertegenwoordigerWerdGewijzigd VertegenwoordigerWerdGewijzigd { get; set; }
    public VertegenwoordigerWerdVerwijderd VertegenwoordigerWerdVerwijderd { get; set; }

    public VertegenwoordigerWerdToegevoegd VertegenwoordigerWerdToegevoegdVoorKszNietGekend { get; set; }
    public KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend { get; set; }
    public VertegenwoordigerWerdToegevoegd VertegenwoordigerWerdToegevoegdVoorKszOverleden { get; set; }

    public KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden { get; set; }

    private CommandMetadata Metadata;

    public VertegenwoordigerPersoonsgegevensOnVZERScenario()
    {
    }

    public async Task<KeyValuePair<string, IEvent[]>[]> GivenEvents(IVCodeService service)
    {
        var fixture = new Fixture().CustomizeAdminApi();

        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd = fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
        {
            VCode = await service.GetNext(),
        };

        VertegenwoordigerWerdToegevoegd = fixture.Create<VertegenwoordigerWerdToegevoegd>();

        VertegenwoordigerWerdGewijzigd = fixture.Create<VertegenwoordigerWerdGewijzigd>() with
        {
            VertegenwoordigerId = VertegenwoordigerWerdToegevoegd.VertegenwoordigerId,
        };

        VertegenwoordigerWerdVerwijderd = fixture.Create<VertegenwoordigerWerdVerwijderd>() with
        {
            VertegenwoordigerId = VertegenwoordigerWerdToegevoegd.VertegenwoordigerId,
            Voornaam = VertegenwoordigerWerdGewijzigd.Voornaam,
            Achternaam = VertegenwoordigerWerdGewijzigd.Achternaam,
        };

        VertegenwoordigerWerdToegevoegdVoorKszNietGekend = fixture.Create<VertegenwoordigerWerdToegevoegd>();
        KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend = fixture.Create<KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend>() with
        {
            VertegenwoordigerId = VertegenwoordigerWerdToegevoegdVoorKszNietGekend.VertegenwoordigerId,
        };

        VertegenwoordigerWerdToegevoegdVoorKszOverleden = fixture.Create<VertegenwoordigerWerdToegevoegd>();
        KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden = fixture.Create<KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden>() with
        {
            VertegenwoordigerId = VertegenwoordigerWerdToegevoegdVoorKszNietGekend.VertegenwoordigerId,
        };


        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };

        return
        [
            new(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode,
            [
                VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
                VertegenwoordigerWerdToegevoegd,
                VertegenwoordigerWerdGewijzigd,
                VertegenwoordigerWerdVerwijderd,
                VertegenwoordigerWerdToegevoegdVoorKszNietGekend,
                KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend,
                VertegenwoordigerWerdToegevoegdVoorKszOverleden,
                KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden,
            ]),
        ];
    }

    public StreamActionResult Result { get; set; } = null!;

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
