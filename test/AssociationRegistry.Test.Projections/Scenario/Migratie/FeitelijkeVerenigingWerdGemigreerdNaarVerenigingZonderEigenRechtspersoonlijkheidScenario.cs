namespace AssociationRegistry.Test.Projections.Scenario.Migratie;

using Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;

public class FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheidScenario : InszScenarioBase
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid;

    public FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheidScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();

        FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid = new FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(VCode: FeitelijkeVerenigingWerdGeregistreerd.VCode);
    }

    public override string VCode => FeitelijkeVerenigingWerdGeregistreerd.VCode;
    public override string Insz => FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordigers.First().Insz;

    public override EventsPerVCode[] Events =>
    [
        new(VCode, [FeitelijkeVerenigingWerdGeregistreerd, FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid])
    ];
}
