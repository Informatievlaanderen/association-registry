namespace AssociationRegistry.Test.Projections.Scenario.Adressen;

using Events;
using AutoFixture;

public class AdresWerdOpnieuwOvergenomenUitAdressenregisterScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingWerdGeregistreerd { get; }
    public AdresWerdOvergenomenUitAdressenregister AdresWerdOvergenomenUitAdressenregister { get; }
    public LocatieWerdGewijzigd LocatieWerdGewijzigd { get; }
    public AdresWerdOvergenomenUitAdressenregister AdresWerdOpnieuwOvergenomenUitAdressenregister { get; }

    public AdresWerdOpnieuwOvergenomenUitAdressenregisterScenario()
    {
        VerenigingWerdGeregistreerd = AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();
        AdresWerdOvergenomenUitAdressenregister = AutoFixture.Create<AdresWerdOvergenomenUitAdressenregister>() with
        {
            VCode = VerenigingWerdGeregistreerd.VCode,
            LocatieId = VerenigingWerdGeregistreerd.Locaties.First().LocatieId,
        };
        LocatieWerdGewijzigd = AutoFixture.Create<LocatieWerdGewijzigd>() with
        {
            Locatie = AutoFixture.Create<Registratiedata.Locatie>() with
            {
                LocatieId = VerenigingWerdGeregistreerd.Locaties.First().LocatieId,
            }
        };
        AdresWerdOpnieuwOvergenomenUitAdressenregister = AutoFixture.Create<AdresWerdOvergenomenUitAdressenregister>() with
        {
            VCode = VerenigingWerdGeregistreerd.VCode,
            LocatieId = VerenigingWerdGeregistreerd.Locaties.First().LocatieId,
        };
    }

    public override string VCode => VerenigingWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VCode, VerenigingWerdGeregistreerd, AdresWerdOvergenomenUitAdressenregister, LocatieWerdGewijzigd, AdresWerdOpnieuwOvergenomenUitAdressenregister),
    ];
}
