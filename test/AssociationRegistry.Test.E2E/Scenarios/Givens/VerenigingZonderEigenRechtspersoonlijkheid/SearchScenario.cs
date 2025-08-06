namespace AssociationRegistry.Test.E2E.Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;

using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Events;
using EventStore;
using Framework.TestClasses;
using MartenDb.Store;
using Vereniging;

public class SearchScenario : IScenario
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd1 { get; set; }
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd2 { get; set; }
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd3 { get; set; }
    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd1 { get; set; }
    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd2 { get; set; }


    private CommandMetadata Metadata;

    public SearchScenario()
    {
    }

    public async Task<KeyValuePair<string, IEvent[]>[]> GivenEvents(IVCodeService service)
    {
        var fixture = new Fixture().CustomizeAdminApi();

        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd1 = fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
        {
            VCode = await service.GetNext(),
        };
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd2 = fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
        {
            VCode = await service.GetNext(),
        };
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd3 = fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
        {
            VCode = await service.GetNext(),
        };
        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd1 = fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
        {
            VCode = await service.GetNext(),
        };
        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd2 = fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
        {
            VCode = await service.GetNext(),
        };

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };

        return
        [
            new(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd1.VCode, [VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd1]),
            new(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd1.VCode, [VerenigingMetRechtspersoonlijkheidWerdGeregistreerd1]),
            new(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd2.VCode, [VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd2]),
            new(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd2.VCode, [VerenigingMetRechtspersoonlijkheidWerdGeregistreerd2]),
            new(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd3.VCode, [VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd3]),
        ];
    }

    public StreamActionResult Result { get; set; } = null!;

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
