namespace AssociationRegistry.Test.E2E.Scenarios.Givens.FeitelijkeVereniging;

using Events;
using EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using Vereniging;
using AutoFixture;

public class MultipleWerdGeregistreerdScenario : Framework.TestClasses.IScenario
{
    public FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd { get; set; }
    public FeitelijkeVerenigingWerdGeregistreerd AndereFeitelijkeVerenigingWerdGeregistreerd { get; set; }
    private CommandMetadata Metadata;

    public MultipleWerdGeregistreerdScenario()
    {
    }

    public async Task<KeyValuePair<string, IEvent[]>[]> GivenEvents(IVCodeService service)
    {
        var fixture = new Fixture().CustomizeAdminApi();

        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
        {
            VCode = await service.GetNext(),
        };
        AndereFeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
        {
            VCode = await service.GetNext(),
            Vertegenwoordigers = FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordigers,
        };

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };

        return
        [
            new(FeitelijkeVerenigingWerdGeregistreerd.VCode, [FeitelijkeVerenigingWerdGeregistreerd, new FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(FeitelijkeVerenigingWerdGeregistreerd.VCode)]),
            new(AndereFeitelijkeVerenigingWerdGeregistreerd.VCode, [AndereFeitelijkeVerenigingWerdGeregistreerd, new FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(AndereFeitelijkeVerenigingWerdGeregistreerd.VCode)]),
        ];
    }

    public StreamActionResult Result { get; set; } = null!;

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
