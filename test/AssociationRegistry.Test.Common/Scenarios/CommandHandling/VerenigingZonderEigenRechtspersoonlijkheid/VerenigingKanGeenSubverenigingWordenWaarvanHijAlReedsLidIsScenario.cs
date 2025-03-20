namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;

using AutoFixture;
using Events;
using global::AutoFixture;
using Vereniging;

/// <summary>
/// This is a scenario to check if andere vereniging is already a lid to the same vereniging
/// in your command, the andereVereniging will be the VerenigingMetRechtspersoonlijkheidWerdGeregistreerd
/// </summary>
public class VerenigingKanGeenSubverenigingWordenWaarvanHijAlReedsLidIsScenario
{
    public static VCode VCodeVereniging => VCode.Create("V0009002");
    public static VCode VCodeAndereVereniging => VCode.Create("V0019002");

    public class VoorTeVerfijenSubverenigingScenario : CommandhandlerScenarioBase
    {
        public override VCode VCode => VCodeVereniging;
        public readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd;
        public readonly LidmaatschapWerdToegevoegd LidmaatschapWerdToegevoegd;

        public VoorTeVerfijenSubverenigingScenario()
        {
            var fixture = new Fixture().CustomizeAdminApi();
            VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd = fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with { VCode = VCode };

            LidmaatschapWerdToegevoegd =
                new LidmaatschapWerdToegevoegd(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode, fixture.Create<Registratiedata.Lidmaatschap>() with
                {
                    AndereVereniging = VCodeAndereVereniging,
                });
        }

        public override IEnumerable<IEvent> Events()
            =>
            [
                VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
                LidmaatschapWerdToegevoegd,
            ];
    }

    public class VoorTeWijzigenSubverenigingScenario : CommandhandlerScenarioBase
    {
        public override VCode VCode => VCodeVereniging;
        public readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd;
        public readonly VerenigingssubtypeWerdVerfijndNaarSubvereniging VerenigingssubtypeWerdVerfijndNaarSubvereniging;
        public readonly LidmaatschapWerdToegevoegd LidmaatschapWerdToegevoegd;

        public VoorTeWijzigenSubverenigingScenario()
        {
            var fixture = new Fixture().CustomizeAdminApi();
            VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd = fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with { VCode = VCode };

            VerenigingssubtypeWerdVerfijndNaarSubvereniging = fixture.Create<VerenigingssubtypeWerdVerfijndNaarSubvereniging>() with
            {
                VCode = VCode,
                SubverenigingVan = fixture.Create<Registratiedata.SubverenigingVan>() with
                {
                    AndereVereniging = fixture.Create<VCode>(), // we want to change it later to VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario
                }
            };

            LidmaatschapWerdToegevoegd =
                new LidmaatschapWerdToegevoegd(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode, fixture.Create<Registratiedata.Lidmaatschap>() with
                {
                    AndereVereniging = VCodeAndereVereniging,
                });
        }

        public override IEnumerable<IEvent> Events()
            =>
            [
                VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
                VerenigingssubtypeWerdVerfijndNaarSubvereniging,
                LidmaatschapWerdToegevoegd,
            ];
    }

    public class VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario : CommandhandlerScenarioBase
    {
        public override VCode VCode => VCodeAndereVereniging;
        public KboNummer KboNummer => KboNummer.Create(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.KboNummer);
        public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;

        public VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario()
        {
            var fixture = new Fixture().CustomizeAdminApi();

            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
                fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with { VCode = VCode };
        }

        public override IEnumerable<IEvent> Events()
            => new IEvent[]
            {
                VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            };
    }
}
