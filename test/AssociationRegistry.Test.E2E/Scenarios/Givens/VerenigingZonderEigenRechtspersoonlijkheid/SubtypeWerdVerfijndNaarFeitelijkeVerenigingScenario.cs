namespace AssociationRegistry.Test.E2E.Scenarios.Givens.FeitelijkeVereniging;

using AutoFixture;
using Common.AutoFixture;
using Events;
using EventStore;
using Framework.TestClasses;
using MartenDb.Store;
using Vereniging;

public class SubtypeWerdVerfijndNaarFeitelijkeVerenigingScenario : IScenario
{
    public string NaamVereniging { get; set; }
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get;
        set;
    }
    public VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging { get; set; }

    public SubtypeWerdVerfijndNaarFeitelijkeVerenigingScenario()
    {
    }

    public async Task<KeyValuePair<string, IEvent[]>[]> GivenEvents(IVCodeService service)
    {
        var fixture = new Fixture().CustomizeAdminApi();

        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd = fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
        {
            VCode = await service.GetNext(),
        };

        VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging = new VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging(
            VCode: VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode);

        return
        [
            new(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode, [VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd, VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging]),
        ];
    }

    public StreamActionResult Result { get; set; } = null!;

}
