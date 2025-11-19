namespace AssociationRegistry.Test.E2E.Scenarios.Givens.MetRechtspersoonlijkheid;

using Events;
using EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using Vereniging;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using MartenDb.Store;
using Requests.VerenigingMetRechtspersoonlijkheid;


public class VertegenwoordigerWerdToegevoegdVanuitKBOScenario : IVerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario,
                                                                Framework.TestClasses.IScenario
{
    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd { get; set; }
    public VertegenwoordigerWerdToegevoegdVanuitKBO VertegenwoordigerWerdToegevoegdVanuitKBO { get; set; }
    private CommandMetadata Metadata;
    public VCode VCode { get; private set; }

    public VertegenwoordigerWerdToegevoegdVanuitKBOScenario()
    { }

    public async Task<KeyValuePair<string, IEvent[]>[]> GivenEvents(IVCodeService service)
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VCode = await service.GetNext();

        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd = fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                VCode = VCode,
            };

        VertegenwoordigerWerdToegevoegdVanuitKBO = fixture.Create<VertegenwoordigerWerdToegevoegdVanuitKBO>();

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };

        return [
            new(VCode, [VerenigingMetRechtspersoonlijkheidWerdGeregistreerd, VertegenwoordigerWerdToegevoegdVanuitKBO])
        ];
    }

    public StreamActionResult Result { get; set; } = null!;

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
