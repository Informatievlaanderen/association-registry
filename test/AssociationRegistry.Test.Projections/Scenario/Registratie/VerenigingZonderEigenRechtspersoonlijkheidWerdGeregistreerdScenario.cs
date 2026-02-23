namespace AssociationRegistry.Test.Projections.Scenario.Registratie;

using AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;

public class VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; }

    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();
    }

    public override string AggregateId => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
        [
            new(
                VCode: VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode,
                VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd
            ),
        ];
}

public class VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdNaEersteKszEventScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; }

    public VCode KszEventVCode { get; set; }

    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdNaEersteKszEventScenario()
    {
        KszEventVCode = AutoFixture.Create<VCode>();

        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();
    }

    public override string AggregateId => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events
    {
        get
        {
            var verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
                AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
                {
                    VCode = KszEventVCode,
                };

            return
            [
                new EventsPerVCode(
                    VCode: KszEventVCode,
                    verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
                    AutoFixture.Create<KszSyncHeeftVertegenwoordigerBevestigd>() with
                    {
                        VertegenwoordigerId = verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd
                            .Vertegenwoordigers.First()
                            .VertegenwoordigerId,
                    }
                ),
                new EventsPerVCode(
                    VCode: VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode,
                    VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd
                ),
            ];
        }
    }
}
