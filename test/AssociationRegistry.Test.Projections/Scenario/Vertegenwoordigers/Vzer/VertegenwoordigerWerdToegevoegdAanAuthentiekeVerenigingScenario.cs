namespace AssociationRegistry.Test.Projections.Scenario.Vertegenwoordigers.Vzer;

using Events;
using AutoFixture;

public class VertegenwoordigerWerdToegevoegdAanAuthentiekeVerenigingScenario : InszScenarioBase
{
    public FeitelijkeVerenigingWerdGeregistreerd DubbeleVerenigingWerdGeregistreerd { get; }
    public FeitelijkeVerenigingWerdGeregistreerd AuthentiekeVerenigingWerdGeregistreerd { get; }
    public VerenigingWerdGemarkeerdAlsDubbelVan VerenigingWerdGemarkeerdAlsDubbelVan { get; set; }
    public VerenigingAanvaarddeDubbeleVereniging VerenigingAanvaarddeDubbeleVereniging { get; set; }
    public VertegenwoordigerWerdToegevoegdMetPersoonsgegevens VertegenwoordigerWerdToegevoegdMetPersoonsgegevens { get; set; }

    private string _insz { get; }
    public VertegenwoordigerWerdToegevoegdAanAuthentiekeVerenigingScenario()
    {

        DubbeleVerenigingWerdGeregistreerd = AutoFixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();

        AuthentiekeVerenigingWerdGeregistreerd = AutoFixture.Create<FeitelijkeVerenigingWerdGeregistreerd>()
            with
            {
                Vertegenwoordigers = DubbeleVerenigingWerdGeregistreerd.Vertegenwoordigers,
            };



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

        VertegenwoordigerWerdToegevoegdMetPersoonsgegevens = AutoFixture.Create<VertegenwoordigerWerdToegevoegdMetPersoonsgegevens>();
        _insz = VertegenwoordigerWerdToegevoegdMetPersoonsgegevens.VertegenwoordigerPersoonsgegevens.Insz;
    }

    public override string VCode => AuthentiekeVerenigingWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(DubbeleVerenigingWerdGeregistreerd.VCode, DubbeleVerenigingWerdGeregistreerd, VerenigingWerdGemarkeerdAlsDubbelVan),
        new(AuthentiekeVerenigingWerdGeregistreerd.VCode, AuthentiekeVerenigingWerdGeregistreerd, VerenigingAanvaarddeDubbeleVereniging, VertegenwoordigerWerdToegevoegdMetPersoonsgegevens),
    ];

    public override string Insz => _insz;
}
