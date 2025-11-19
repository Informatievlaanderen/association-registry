namespace AssociationRegistry.Test.E2E.Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;

using Admin.Schema.Persoonsgegevens;
using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;
using Events.Enriched;
using EventStore;
using Framework.Mappers;
using Framework.TestClasses;

public class SubtypeWerdVerfijndNaarFeitelijkeVerenigingScenario : IScenario
{
    public string NaamVereniging { get; set; }
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario BaseScenario = new();
    public VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging { get; set; }

    public SubtypeWerdVerfijndNaarFeitelijkeVerenigingScenario()
    {

    }

    public async Task<KeyValuePair<string, IEvent[]>[]> GivenEvents(IVCodeService service)
    {
        await BaseScenario.GivenEvents(service);
        VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging = new VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging(
            VCode: BaseScenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode);
        return
        [
            new(BaseScenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode,
            [
                BaseScenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
                VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging
            ]),
        ];
    }

    public VertegenwoordigerPersoonsgegevensDocument[] GivenVertegenwoordigerPersoonsgegevens()
        => BaseScenario.GivenVertegenwoordigerPersoonsgegevens();

    public StreamActionResult Result { get; set; } = null!;

}
