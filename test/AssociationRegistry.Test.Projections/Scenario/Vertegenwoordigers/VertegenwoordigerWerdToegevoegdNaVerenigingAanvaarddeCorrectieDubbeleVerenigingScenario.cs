namespace AssociationRegistry.Test.Projections.Scenario.Vertegenwoordigers;

using Events;
using AutoFixture;

public class VertegenwoordigerWerdToegevoegdNaVerenigingAanvaarddeCorrectieDubbeleVerenigingScenario : InszScenarioBase
{
    public FeitelijkeVerenigingWerdGeregistreerd DubbeleVerenigingOmTeCorrigeren { get; }
    public FeitelijkeVerenigingWerdGeregistreerd DubbeleVerengingOmTeHouden { get; }
    public FeitelijkeVerenigingWerdGeregistreerd AuthentiekeVereniging { get; }
    public VerenigingWerdGemarkeerdAlsDubbelVan DubbeleVerenigingOmTeCorrigerenWerdGemarkeerdAlsDubbelVan { get; set; }
    public VerenigingAanvaarddeDubbeleVereniging AuthentiekeVerenigingAanvaarddeTeCorrigerenDubbeleVereniging { get; set; }
    public VerenigingAanvaarddeDubbeleVereniging AuthentiekeVerenigingAanvaarddeTeHoudenDubbeleVereniging { get; set; }
    public MarkeringDubbeleVerengingWerdGecorrigeerd MarkeringDubbeleVerengingWerdGecorrigeerd { get; set; }
    public VerenigingAanvaarddeCorrectieDubbeleVereniging VerenigingAanvaarddeCorrectieDubbeleVereniging { get; set; }
    public VertegenwoordigerWerdToegevoegd VertegenwoordigerWerdToegevoegd { get; set; }


    private string _insz { get; }
    public VertegenwoordigerWerdToegevoegdNaVerenigingAanvaarddeCorrectieDubbeleVerenigingScenario()
    {

        DubbeleVerengingOmTeHouden = AutoFixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();
        DubbeleVerenigingOmTeCorrigeren = AutoFixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();

        AuthentiekeVereniging = AutoFixture.Create<FeitelijkeVerenigingWerdGeregistreerd>()
            with
            {
                Vertegenwoordigers = DubbeleVerenigingOmTeCorrigeren.Vertegenwoordigers,
            };

        _insz = AuthentiekeVereniging.Vertegenwoordigers[0].Insz;

        DubbeleVerenigingOmTeCorrigerenWerdGemarkeerdAlsDubbelVan = AutoFixture.Create<VerenigingWerdGemarkeerdAlsDubbelVan>() with
        {
            VCode = DubbeleVerenigingOmTeCorrigeren.VCode,
            VCodeAuthentiekeVereniging = AuthentiekeVereniging.VCode,
        };

        MarkeringDubbeleVerengingWerdGecorrigeerd = AutoFixture.Create<MarkeringDubbeleVerengingWerdGecorrigeerd>() with
        {
            VCode = DubbeleVerenigingOmTeCorrigeren.VCode,
            VCodeAuthentiekeVereniging = AuthentiekeVereniging.VCode,
        };

        AuthentiekeVerenigingAanvaarddeTeCorrigerenDubbeleVereniging = AutoFixture.Create<VerenigingAanvaarddeDubbeleVereniging>() with
        {
            VCode = AuthentiekeVereniging.VCode,
            VCodeDubbeleVereniging = DubbeleVerenigingOmTeCorrigeren.VCode,
        };

        VerenigingAanvaarddeCorrectieDubbeleVereniging = AutoFixture.Create<VerenigingAanvaarddeCorrectieDubbeleVereniging>() with
        {
            VCode = AuthentiekeVereniging.VCode,
            VCodeDubbeleVereniging = DubbeleVerenigingOmTeCorrigeren.VCode,
        };

        AuthentiekeVerenigingAanvaarddeTeHoudenDubbeleVereniging = AutoFixture.Create<VerenigingAanvaarddeDubbeleVereniging>() with
        {
            VCode = AuthentiekeVereniging.VCode,
            VCodeDubbeleVereniging = DubbeleVerengingOmTeHouden.VCode,
        };

        VertegenwoordigerWerdToegevoegd = AutoFixture.Create<VertegenwoordigerWerdToegevoegd>();
        _insz = VertegenwoordigerWerdToegevoegd.Insz;
    }

    public override string VCode => AuthentiekeVereniging.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(DubbeleVerengingOmTeHouden.VCode, DubbeleVerengingOmTeHouden),
        new(DubbeleVerenigingOmTeCorrigeren.VCode, DubbeleVerenigingOmTeCorrigeren, DubbeleVerenigingOmTeCorrigerenWerdGemarkeerdAlsDubbelVan, MarkeringDubbeleVerengingWerdGecorrigeerd),
        new(AuthentiekeVereniging.VCode, AuthentiekeVereniging, AuthentiekeVerenigingAanvaarddeTeCorrigerenDubbeleVereniging, VerenigingAanvaarddeCorrectieDubbeleVereniging, VertegenwoordigerWerdToegevoegd, AuthentiekeVerenigingAanvaarddeTeHoudenDubbeleVereniging),
    ];

    public override string Insz => _insz;
}
