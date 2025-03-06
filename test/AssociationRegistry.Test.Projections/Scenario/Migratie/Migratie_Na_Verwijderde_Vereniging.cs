namespace AssociationRegistry.Test.Projections.Scenario.Migratie;

using Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using System.Linq;

public class Migratie_Na_Verwijderde_Vereniging : InszScenarioBase
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly VerenigingWerdVerwijderd VerenigingWerdVerwijderd;
    public readonly FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid;

    public Migratie_Na_Verwijderde_Vereniging()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();
        VerenigingWerdVerwijderd = fixture.Create<VerenigingWerdVerwijderd>();

        FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid = new FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(VCode: FeitelijkeVerenigingWerdGeregistreerd.VCode);
    }

    public override string VCode => FeitelijkeVerenigingWerdGeregistreerd.VCode;
    public override string Insz => FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordigers.First().Insz;

    public override EventsPerVCode[] Events =>
    [
        new(VCode, [FeitelijkeVerenigingWerdGeregistreerd,VerenigingWerdVerwijderd, FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid])
    ];
}

