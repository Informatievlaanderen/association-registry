namespace AssociationRegistry.Test.E2E.Scenarios.Givens.FeitelijkeVereniging.VertegenwoordigerPersoonsgegevens;

using Events;
using EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using Vereniging;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using MartenDb.Store;

public class VertegenwoordigerPersoonsgegevensOnFVScenario : Framework.TestClasses.IScenario
{
    public FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd { get; set; }
    public VertegenwoordigerWerdToegevoegd VertegenwoordigerWerdToegevoegd { get; set; }
    public VertegenwoordigerWerdGewijzigd VertegenwoordigerWerdGewijzigd { get; set; }
    public VertegenwoordigerWerdVerwijderd VertegenwoordigerWerdVerwijderd { get; set; }

    private CommandMetadata Metadata;

    public VertegenwoordigerPersoonsgegevensOnFVScenario()
    {
    }

    public async Task<KeyValuePair<string, IEvent[]>[]> GivenEvents(IVCodeService service)
    {
        var fixture = new Fixture().CustomizeAdminApi();

        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
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


        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };

        return
        [
            new(FeitelijkeVerenigingWerdGeregistreerd.VCode,
            [
                FeitelijkeVerenigingWerdGeregistreerd,
                new FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(
                    FeitelijkeVerenigingWerdGeregistreerd.VCode),
                VertegenwoordigerWerdToegevoegd,
                VertegenwoordigerWerdGewijzigd,
                VertegenwoordigerWerdVerwijderd,
            ]),
        ];
    }

    public StreamActionResult Result { get; set; } = null!;

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
