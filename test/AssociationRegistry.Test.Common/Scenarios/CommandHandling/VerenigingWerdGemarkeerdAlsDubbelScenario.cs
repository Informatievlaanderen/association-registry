namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using global::AutoFixture;
using Vereniging;

public class VerenigingWerdGemarkeerdAlsDubbelVanScenario : CommandhandlerScenarioBase
{
    public FeitelijkeVerenigingWerdGeregistreerd DubbeleVerenigingWerdGeregistreerd { get; }
    // public FeitelijkeVerenigingWerdGeregistreerd AuthentiekeVerenigingWerdGeregistreerd { get; }
    public VerenigingWerdGemarkeerdAlsDubbelVan VerenigingWerdGemarkeerdAlsDubbelVan { get; set; }
    // public VerenigingAanvaarddeDubbeleVereniging VerenigingAanvaarddeDubbeleVereniging { get; set; }

    public VerenigingWerdGemarkeerdAlsDubbelVanScenario()
    {
        var fixture = new Fixture().CustomizeDomain();
        DubbeleVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();

        // AuthentiekeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>()
        //     with
        //     {
        //         Vertegenwoordigers = DubbeleVerenigingWerdGeregistreerd.Vertegenwoordigers,
        //     };

        VerenigingWerdGemarkeerdAlsDubbelVan = fixture.Create<VerenigingWerdGemarkeerdAlsDubbelVan>() with
        {
            VCode = DubbeleVerenigingWerdGeregistreerd.VCode,
            VCodeAuthentiekeVereniging = fixture.Create<VCode>(),
        };

        // VerenigingAanvaarddeDubbeleVereniging = fixture.Create<VerenigingAanvaarddeDubbeleVereniging>() with
        // {
        //     VCode = AuthentiekeVerenigingWerdGeregistreerd.VCode,
        //     VCodeDubbeleVereniging = DubbeleVerenigingWerdGeregistreerd.VCode,
        // };
    }

    public override VCode VCode => VCode.Create(DubbeleVerenigingWerdGeregistreerd.VCode);

    public override IEnumerable<IEvent> Events()
        =>
        [
            DubbeleVerenigingWerdGeregistreerd,
            VerenigingWerdGemarkeerdAlsDubbelVan,
        ];
}
