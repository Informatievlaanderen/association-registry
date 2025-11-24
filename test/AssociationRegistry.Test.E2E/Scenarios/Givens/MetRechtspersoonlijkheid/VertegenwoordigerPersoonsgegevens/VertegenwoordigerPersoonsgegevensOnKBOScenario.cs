namespace AssociationRegistry.Test.E2E.Scenarios.Givens.MetRechtspersoonlijkheid.VertegenwoordigerPersoonsgegevens;

using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;
using EventStore;
using Framework.TestClasses;

public class VertegenwoordigerPersoonsgegevensOnKBOScenario : IScenario
{
    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd { get; set; }
    public VertegenwoordigerWerdToegevoegdVanuitKBO VertegenwoordigerWerdToegevoegdVanuitKBO { get; set; }
    public VertegenwoordigerWerdOvergenomenUitKBO VertegenwoordigerWerdOvergenomenUitKBO { get; set; }
    public VertegenwoordigerWerdGewijzigdInKBO VertegenwoordigerWerdGewijzigdInKBO { get; set; }
    public VertegenwoordigerWerdVerwijderdUitKBO VertegenwoordigerWerdVerwijderdUitKBO { get; set; }

    private CommandMetadata Metadata;

    public VertegenwoordigerPersoonsgegevensOnKBOScenario()
    {
    }

    public async Task<KeyValuePair<string, IEvent[]>[]> GivenEvents(IVCodeService service)
    {
        var fixture = new Fixture().CustomizeAdminApi();

        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd = fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
        {
            VCode = await service.GetNext(),
        };

        VertegenwoordigerWerdToegevoegdVanuitKBO = fixture.Create<VertegenwoordigerWerdToegevoegdVanuitKBO>();
        VertegenwoordigerWerdOvergenomenUitKBO = fixture.Create<VertegenwoordigerWerdOvergenomenUitKBO>();

        VertegenwoordigerWerdGewijzigdInKBO = fixture.Create<VertegenwoordigerWerdGewijzigdInKBO>() with
        {
            VertegenwoordigerId = VertegenwoordigerWerdToegevoegdVanuitKBO.VertegenwoordigerId,
            Insz = VertegenwoordigerWerdToegevoegdVanuitKBO.Insz,
        };

        VertegenwoordigerWerdVerwijderdUitKBO = fixture.Create<VertegenwoordigerWerdVerwijderdUitKBO>() with
        {
            VertegenwoordigerId = VertegenwoordigerWerdGewijzigdInKBO.VertegenwoordigerId,
            Voornaam = VertegenwoordigerWerdGewijzigdInKBO.Voornaam,
            Achternaam = VertegenwoordigerWerdGewijzigdInKBO.Achternaam,
            Insz = VertegenwoordigerWerdGewijzigdInKBO.Insz,
        };


        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };

        return
        [
            new(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode,
            [
                VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
                VertegenwoordigerWerdToegevoegdVanuitKBO,
                VertegenwoordigerWerdOvergenomenUitKBO,
                VertegenwoordigerWerdGewijzigdInKBO,
                VertegenwoordigerWerdVerwijderdUitKBO,
            ]),
        ];
    }

    public StreamActionResult Result { get; set; } = null!;

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
