namespace AssociationRegistry.Test.E2E.Scenarios.Givens.MetRechtspersoonlijkheid.VertegenwoordigerPersoonsgegevens;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Events;
using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using AutoFixture;

public class VertegenwoordigerZonderPersoonsgegevensOnKBOScenario : IScenario
{
    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd { get; set; }
    public VertegenwoordigerWerdToegevoegdVanuitKBOZonderPersoonsgegevens VertegenwoordigerWerdToegevoegdVanuitKBOZonderPersoonsgegevensToKeep { get; set; }
    public VertegenwoordigerWerdOvergenomenUitKBOZonderPersoonsgegevens VertegenwoordigerWerdOvergenomenUitKBOZonderPersoonsgegevensToChangeAndDelete { get; set; }
    public VertegenwoordigerWerdGewijzigdInKBOZonderPersoonsgegevens VertegenwoordigerWerdGewijzigdInKBOZonderPersoonsgegevens { get; set; }
    public VertegenwoordigerWerdVerwijderdUitKBOZonderPersoonsgegevens VertegenwoordigerWerdVerwijderdUitKBOZonderPersoonsgegevens { get; set; }

    private CommandMetadata Metadata;

    public VertegenwoordigerZonderPersoonsgegevensOnKBOScenario()
    {
    }

    public async Task<KeyValuePair<string, IEvent[]>[]> GivenEvents(IVCodeService service)
    {
        var fixture = new Fixture().CustomizeAdminApi();


        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd = fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
        {
            VCode = await service.GetNext(),
        };

        VertegenwoordigerWerdToegevoegdVanuitKBOZonderPersoonsgegevensToKeep = fixture.Create<VertegenwoordigerWerdToegevoegdVanuitKBOZonderPersoonsgegevens>();

        VertegenwoordigerWerdOvergenomenUitKBOZonderPersoonsgegevensToChangeAndDelete = fixture.Create<VertegenwoordigerWerdOvergenomenUitKBOZonderPersoonsgegevens>();

        VertegenwoordigerWerdGewijzigdInKBOZonderPersoonsgegevens = fixture.Create<VertegenwoordigerWerdGewijzigdInKBOZonderPersoonsgegevens>() with
        {
            VertegenwoordigerId = VertegenwoordigerWerdOvergenomenUitKBOZonderPersoonsgegevensToChangeAndDelete.VertegenwoordigerId,
        };

        VertegenwoordigerWerdVerwijderdUitKBOZonderPersoonsgegevens = fixture.Create<VertegenwoordigerWerdVerwijderdUitKBOZonderPersoonsgegevens>() with
        {
            VertegenwoordigerId = VertegenwoordigerWerdOvergenomenUitKBOZonderPersoonsgegevensToChangeAndDelete.VertegenwoordigerId,
        };


        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };

        return
        [
            new(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode,
            [
                VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
                VertegenwoordigerWerdToegevoegdVanuitKBOZonderPersoonsgegevensToKeep,
                VertegenwoordigerWerdOvergenomenUitKBOZonderPersoonsgegevensToChangeAndDelete,
                VertegenwoordigerWerdGewijzigdInKBOZonderPersoonsgegevens,
                VertegenwoordigerWerdVerwijderdUitKBOZonderPersoonsgegevens,
            ]),
        ];
    }

    public StreamActionResult Result { get; set; } = null!;

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
