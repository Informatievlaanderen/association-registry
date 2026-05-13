namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;

using AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;
using global::AutoFixture;

public class VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningenScenario : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009002");

    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd
        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;

    public readonly ErkenningWerdGeregistreerd ErkenningWerdGeregistreerd1;
    public readonly ErkenningWerdGeregistreerd ErkenningWerdGeregistreerd2;

    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningenScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                VCode = VCode,
            };

        ErkenningWerdGeregistreerd1 = fixture.Create<ErkenningWerdGeregistreerd>();
        var startDatumVoorErkenning2 = ErkenningWerdGeregistreerd1.Einddatum.Value.AddDays(fixture.Create<int>());
        var hernieuwingsDatumVoorErkenning2 = startDatumVoorErkenning2.AddDays(fixture.Create<int>());
        var eindDatumVoorErkenning2 = hernieuwingsDatumVoorErkenning2.AddDays(fixture.Create<int>());

        ErkenningWerdGeregistreerd2 = fixture.Create<ErkenningWerdGeregistreerd>() with
        {
            Startdatum = startDatumVoorErkenning2,
            Einddatum = eindDatumVoorErkenning2,
            Hernieuwingsdatum = hernieuwingsDatumVoorErkenning2,
            IpdcProduct = ErkenningWerdGeregistreerd1.IpdcProduct,
            GeregistreerdDoor = ErkenningWerdGeregistreerd1.GeregistreerdDoor,
        };
    }

    public override IEnumerable<IEvent> Events() =>
        new IEvent[]
        {
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            ErkenningWerdGeregistreerd1,
            ErkenningWerdGeregistreerd2,
        };
}
