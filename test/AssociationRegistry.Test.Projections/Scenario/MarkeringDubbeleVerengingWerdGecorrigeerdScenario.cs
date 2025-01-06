namespace AssociationRegistry.Test.Projections.Scenario;

using AutoFixture;
using Events;

public class MarkeringDubbeleVerengingWerdGecorrigeerdScenario : InszScenarioBase
{
    public FeitelijkeVerenigingWerdGeregistreerd DubbeleVerenigingWerdGeregistreerd { get; }
    public FeitelijkeVerenigingWerdGeregistreerd AuthentiekeVerenigingWerdGeregistreerd { get; }
    public VerenigingWerdGemarkeerdAlsDubbelVan VerenigingWerdGemarkeerdAlsDubbelVan { get; set; }
    public VerenigingAanvaarddeDubbeleVereniging VerenigingAanvaarddeDubbeleVereniging { get; set; }
    public MarkeringDubbeleVerengingWerdGecorrigeerd MarkeringDubbeleVerengingWerdGecorrigeerd { get; set; }

    private string _insz { get; }
    public MarkeringDubbeleVerengingWerdGecorrigeerdScenario()
    {

        DubbeleVerenigingWerdGeregistreerd = AutoFixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();

        AuthentiekeVerenigingWerdGeregistreerd = AutoFixture.Create<FeitelijkeVerenigingWerdGeregistreerd>()
            with
            {
                Vertegenwoordigers = DubbeleVerenigingWerdGeregistreerd.Vertegenwoordigers,
            };

        _insz = AuthentiekeVerenigingWerdGeregistreerd.Vertegenwoordigers[0].Insz;

        VerenigingWerdGemarkeerdAlsDubbelVan = AutoFixture.Create<VerenigingWerdGemarkeerdAlsDubbelVan>() with
        {
            VCode = DubbeleVerenigingWerdGeregistreerd.VCode,
            VCodeAuthentiekeVereniging = AuthentiekeVerenigingWerdGeregistreerd.VCode,
        };

        VerenigingAanvaarddeDubbeleVereniging = AutoFixture.Create<VerenigingAanvaarddeDubbeleVereniging>() with
        {
            VCode = AuthentiekeVerenigingWerdGeregistreerd.VCode,
            VCodeDubbeleVereniging = DubbeleVerenigingWerdGeregistreerd.VCode,
        };

        MarkeringDubbeleVerengingWerdGecorrigeerd = AutoFixture.Create<MarkeringDubbeleVerengingWerdGecorrigeerd>() with
        {
            VCode = DubbeleVerenigingWerdGeregistreerd.VCode,
            VCodeAuthentiekeVereniging = AuthentiekeVerenigingWerdGeregistreerd.VCode,
        };
    }

    public override string VCode => DubbeleVerenigingWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VCode, DubbeleVerenigingWerdGeregistreerd, VerenigingWerdGemarkeerdAlsDubbelVan, MarkeringDubbeleVerengingWerdGecorrigeerd),
        new(AuthentiekeVerenigingWerdGeregistreerd.VCode, AuthentiekeVerenigingWerdGeregistreerd, VerenigingAanvaarddeDubbeleVereniging),
    ];

    public override string Insz => _insz;
}
