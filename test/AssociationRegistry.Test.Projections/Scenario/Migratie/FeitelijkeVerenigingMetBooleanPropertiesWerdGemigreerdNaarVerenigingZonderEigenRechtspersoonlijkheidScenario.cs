namespace AssociationRegistry.Test.Projections.Scenario.Migratie;

using Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;

/// <summary>
/// zie bug: or-2749
/// </summary>
public class FeitelijkeVerenigingMetBooleanPropertiesWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheidScenario : InszScenarioBase
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly VerenigingWerdGemarkeerdAlsDubbelVan VerenigingWerdGemarkeerdAlsDubbelVan;
    public readonly VerenigingWerdVerwijderd VerenigingWerdVerwijderd;
    public readonly VerenigingWerdUitgeschrevenUitPubliekeDatastroom VerenigingWerdUitgeschrevenUitPubliekeDatastroom;

    public readonly FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid
        FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid;

    public FeitelijkeVerenigingMetBooleanPropertiesWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheidScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();

        VerenigingWerdGemarkeerdAlsDubbelVan = fixture.Create<VerenigingWerdGemarkeerdAlsDubbelVan>() with
        {
            VCode = FeitelijkeVerenigingWerdGeregistreerd.VCode
        };

        VerenigingWerdVerwijderd = fixture.Create<VerenigingWerdVerwijderd>() with
        {
            VCode = FeitelijkeVerenigingWerdGeregistreerd.VCode
        };

        VerenigingWerdUitgeschrevenUitPubliekeDatastroom = fixture.Create<VerenigingWerdUitgeschrevenUitPubliekeDatastroom>();


        FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid =
            new FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(
                VCode: FeitelijkeVerenigingWerdGeregistreerd.VCode);
    }

    public override string VCode => FeitelijkeVerenigingWerdGeregistreerd.VCode;
    public override string Insz => FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordigers.First().Insz;

    public override EventsPerVCode[] Events =>
    [
        new(VCode,
            FeitelijkeVerenigingWerdGeregistreerd,
            VerenigingWerdUitgeschrevenUitPubliekeDatastroom,
            VerenigingWerdGemarkeerdAlsDubbelVan,
            VerenigingWerdVerwijderd,
            FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid)
    ];
}
