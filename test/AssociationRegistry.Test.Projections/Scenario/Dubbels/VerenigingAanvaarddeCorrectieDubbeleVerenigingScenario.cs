namespace AssociationRegistry.Test.Projections.Scenario.Dubbels;

using Events;
using AutoFixture;

public class VerenigingAanvaarddeCorrectieDubbeleVerenigingScenario : InszScenarioBase
{
    public FeitelijkeVerenigingWerdGeregistreerd DubbeleVerenigingWerdGeregistreerd { get; }
    public FeitelijkeVerenigingWerdGeregistreerd AuthentiekeVerenigingWerdGeregistreerd { get; }
    public VerenigingWerdGemarkeerdAlsDubbelVan VerenigingWerdGemarkeerdAlsDubbelVan { get; set; }
    public VerenigingAanvaarddeDubbeleVereniging VerenigingAanvaarddeDubbeleVereniging { get; set; }
    public MarkeringDubbeleVerengingWerdGecorrigeerd MarkeringDubbeleVerengingWerdGecorrigeerd { get; set; }
    public VerenigingAanvaarddeCorrectieDubbeleVereniging VerenigingAanvaarddeCorrectieDubbeleVereniging { get; set; }

    private string _insz { get; }
    public VerenigingAanvaarddeCorrectieDubbeleVerenigingScenario()
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

        MarkeringDubbeleVerengingWerdGecorrigeerd = AutoFixture.Create<MarkeringDubbeleVerengingWerdGecorrigeerd>() with
        {
            VCode = DubbeleVerenigingWerdGeregistreerd.VCode,
            VCodeAuthentiekeVereniging = AuthentiekeVerenigingWerdGeregistreerd.VCode,
        };

        VerenigingAanvaarddeDubbeleVereniging = AutoFixture.Create<VerenigingAanvaarddeDubbeleVereniging>() with
        {
            VCode = AuthentiekeVerenigingWerdGeregistreerd.VCode,
            VCodeDubbeleVereniging = DubbeleVerenigingWerdGeregistreerd.VCode,
        };

        VerenigingAanvaarddeCorrectieDubbeleVereniging = AutoFixture.Create<VerenigingAanvaarddeCorrectieDubbeleVereniging>() with
        {
            VCode = AuthentiekeVerenigingWerdGeregistreerd.VCode,
            VCodeDubbeleVereniging = DubbeleVerenigingWerdGeregistreerd.VCode,
        };
    }

    public override string AggregateId => AuthentiekeVerenigingWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(DubbeleVerenigingWerdGeregistreerd.VCode, DubbeleVerenigingWerdGeregistreerd, VerenigingWerdGemarkeerdAlsDubbelVan, MarkeringDubbeleVerengingWerdGecorrigeerd),
        new(AuthentiekeVerenigingWerdGeregistreerd.VCode, AuthentiekeVerenigingWerdGeregistreerd, VerenigingAanvaarddeDubbeleVereniging, VerenigingAanvaarddeCorrectieDubbeleVereniging),
    ];

    public override string Insz => _insz;
}
