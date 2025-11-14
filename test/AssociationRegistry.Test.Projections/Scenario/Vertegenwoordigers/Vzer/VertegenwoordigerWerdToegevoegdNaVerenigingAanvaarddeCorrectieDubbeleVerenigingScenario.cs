namespace AssociationRegistry.Test.Projections.Scenario.Vertegenwoordigers.Vzer;

using Events;
using AutoFixture;
using Events.Enriched;

public class VertegenwoordigerWerdToegevoegdNaVerenigingAanvaarddeCorrectieDubbeleVerenigingScenario : InszScenarioBase
{
    public FeitelijkeVerenigingMetPersoonsgegevensGeregistreerd DubbeleVerenigingOmTeCorrigeren { get; }
    public FeitelijkeVerenigingMetPersoonsgegevensGeregistreerd DubbeleVerengingOmTeHouden { get; }
    public FeitelijkeVerenigingMetPersoonsgegevensGeregistreerd AuthentiekeVereniging { get; }
    public VerenigingWerdGemarkeerdAlsDubbelVan DubbeleVerenigingOmTeCorrigerenWerdGemarkeerdAlsDubbelVan { get; set; }
    public VerenigingAanvaarddeDubbeleVereniging AuthentiekeVerenigingAanvaarddeTeCorrigerenDubbeleVereniging { get; set; }
    public VerenigingAanvaarddeDubbeleVereniging AuthentiekeVerenigingAanvaarddeTeHoudenDubbeleVereniging { get; set; }
    public MarkeringDubbeleVerengingWerdGecorrigeerd MarkeringDubbeleVerengingWerdGecorrigeerd { get; set; }
    public VerenigingAanvaarddeCorrectieDubbeleVereniging VerenigingAanvaarddeCorrectieDubbeleVereniging { get; set; }
    public VertegenwoordigerWerdToegevoegdMetPersoonsgegevens VertegenwoordigerWerdToegevoegdMetPersoonsgegevens { get; set; }


    private string _insz { get; }
    public VertegenwoordigerWerdToegevoegdNaVerenigingAanvaarddeCorrectieDubbeleVerenigingScenario()
    {

        DubbeleVerengingOmTeHouden = AutoFixture.Create<FeitelijkeVerenigingMetPersoonsgegevensGeregistreerd>();
        DubbeleVerenigingOmTeCorrigeren = AutoFixture.Create<FeitelijkeVerenigingMetPersoonsgegevensGeregistreerd>();

        AuthentiekeVereniging = AutoFixture.Create<FeitelijkeVerenigingMetPersoonsgegevensGeregistreerd>()
            with
            {
                Vertegenwoordigers = DubbeleVerenigingOmTeCorrigeren.Vertegenwoordigers,
            };

        _insz = AuthentiekeVereniging.Vertegenwoordigers[0].VertegenwoordigerPersoonsgegevens.Insz;

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

        VertegenwoordigerWerdToegevoegdMetPersoonsgegevens = AutoFixture.Create<VertegenwoordigerWerdToegevoegdMetPersoonsgegevens>();
        _insz = VertegenwoordigerWerdToegevoegdMetPersoonsgegevens.VertegenwoordigerPersoonsgegevens.Insz;
    }

    public override string VCode => AuthentiekeVereniging.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(DubbeleVerengingOmTeHouden.VCode, DubbeleVerengingOmTeHouden),
        new(DubbeleVerenigingOmTeCorrigeren.VCode, DubbeleVerenigingOmTeCorrigeren, DubbeleVerenigingOmTeCorrigerenWerdGemarkeerdAlsDubbelVan, MarkeringDubbeleVerengingWerdGecorrigeerd),
        new(AuthentiekeVereniging.VCode, AuthentiekeVereniging, AuthentiekeVerenigingAanvaarddeTeCorrigerenDubbeleVereniging, VerenigingAanvaarddeCorrectieDubbeleVereniging, VertegenwoordigerWerdToegevoegdMetPersoonsgegevens, AuthentiekeVerenigingAanvaarddeTeHoudenDubbeleVereniging),
    ];

    public override string Insz => _insz;
}
