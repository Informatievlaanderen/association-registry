namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using global::AutoFixture;
using Vereniging;

public class VerenigingWerdGemarkeerdAlsDubbelVanEnVerwijderdScenario : CommandhandlerScenarioBase
{
    public FeitelijkeVerenigingWerdGeregistreerd DubbeleVerenigingWerdGeregistreerd { get; }
    public VerenigingWerdGemarkeerdAlsDubbelVan VerenigingWerdGemarkeerdAlsDubbelVan { get; set; }
    public VerenigingWerdVerwijderd VerenigingWerdVerwijderd { get; set; }

    public VerenigingWerdGemarkeerdAlsDubbelVanEnVerwijderdScenario()
    {
        var fixture = new Fixture().CustomizeDomain();
        DubbeleVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();

        VerenigingWerdGemarkeerdAlsDubbelVan = fixture.Create<VerenigingWerdGemarkeerdAlsDubbelVan>() with
        {
            VCode = DubbeleVerenigingWerdGeregistreerd.VCode,
            VCodeAuthentiekeVereniging = fixture.Create<VCode>(),
        };

        VerenigingWerdVerwijderd = fixture.Create<VerenigingWerdVerwijderd>() with
        {
            VCode = DubbeleVerenigingWerdGeregistreerd.VCode,
        };
    }

    public override VCode VCode => VCode.Create(DubbeleVerenigingWerdGeregistreerd.VCode);

    public override IEnumerable<IEvent> Events()
        =>
        [
            DubbeleVerenigingWerdGeregistreerd,
            VerenigingWerdGemarkeerdAlsDubbelVan,
            VerenigingWerdVerwijderd,
        ];
}
