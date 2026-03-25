namespace AssociationRegistry.Test.E2E.Scenarios.Givens.FeitelijkeVereniging.VertegenwoordigerPersoonsgegevens;

using Events;
using EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using Vereniging;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using MartenDb.Store;

public class VertegenwoordigerZonderPersoonsgegevensOnFVScenario : Framework.TestClasses.IScenario
{
    public FeitelijkeVerenigingWerdGeregistreerdZonderPersoonsgegevens FeitelijkeVerenigingWerdGeregistreerdZonderPersoonsgegevens { get; set; }
    public VertegenwoordigerWerdToegevoegdZonderPersoonsgegevens VertegenwoordigerWerdToegevoegdZonderPersoonsgegevens { get; set; }
    public VertegenwoordigerWerdGewijzigdZonderPersoonsgegevens VertegenwoordigerWerdGewijzigdZonderPersoonsgegevens { get; set; }
    public VertegenwoordigerWerdVerwijderdZonderPersoonsgegevens VertegenwoordigerWerdVerwijderdZonderPersoonsgegevens { get; set; }

    private CommandMetadata Metadata;

    public VertegenwoordigerZonderPersoonsgegevensOnFVScenario()
    {
    }

    public async Task<KeyValuePair<string, IEvent[]>[]> GivenEvents(IVCodeService service)
    {
        var fixture = new Fixture().CustomizeAdminApi();

        FeitelijkeVerenigingWerdGeregistreerdZonderPersoonsgegevens = fixture.Create<FeitelijkeVerenigingWerdGeregistreerdZonderPersoonsgegevens>() with
        {
            VCode = await service.GetNext(),
        };

        VertegenwoordigerWerdToegevoegdZonderPersoonsgegevens = fixture.Create<VertegenwoordigerWerdToegevoegdZonderPersoonsgegevens>() with
        {

        };

        VertegenwoordigerWerdGewijzigdZonderPersoonsgegevens = fixture.Create<VertegenwoordigerWerdGewijzigdZonderPersoonsgegevens>() with
        {
            VertegenwoordigerId = VertegenwoordigerWerdToegevoegdZonderPersoonsgegevens.VertegenwoordigerId,
        };

        VertegenwoordigerWerdVerwijderdZonderPersoonsgegevens = fixture.Create<VertegenwoordigerWerdVerwijderdZonderPersoonsgegevens>() with
        {
            VertegenwoordigerId = VertegenwoordigerWerdToegevoegdZonderPersoonsgegevens.VertegenwoordigerId,
        };


        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };

        return
        [
            new(FeitelijkeVerenigingWerdGeregistreerdZonderPersoonsgegevens.VCode,
            [
                FeitelijkeVerenigingWerdGeregistreerdZonderPersoonsgegevens,
                VertegenwoordigerWerdToegevoegdZonderPersoonsgegevens,
                VertegenwoordigerWerdGewijzigdZonderPersoonsgegevens,
                VertegenwoordigerWerdVerwijderdZonderPersoonsgegevens,
            ]),
        ];
    }

    public StreamActionResult Result { get; set; } = null!;

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
