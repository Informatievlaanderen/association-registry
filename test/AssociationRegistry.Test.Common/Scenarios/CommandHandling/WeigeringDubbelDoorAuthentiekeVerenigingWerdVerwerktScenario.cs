namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using global::AutoFixture;
using Vereniging;

public class WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerktScenario : CommandhandlerScenarioBase
{
    public FeitelijkeVerenigingWerdGeregistreerd DubbeleVerenigingWerdGeregistreerd { get; }
    public VerenigingWerdGemarkeerdAlsDubbelVan VerenigingWerdGemarkeerdAlsDubbelVan { get; set; }
    public WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt { get; set; }

    public WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerktScenario()
    {
        var fixture = new Fixture().CustomizeDomain();
        DubbeleVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();

        VerenigingWerdGemarkeerdAlsDubbelVan = fixture.Create<VerenigingWerdGemarkeerdAlsDubbelVan>() with
        {
            VCode = DubbeleVerenigingWerdGeregistreerd.VCode,
            VCodeAuthentiekeVereniging = fixture.Create<VCode>(),
        };

        WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt = fixture.Create<WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt>() with
        {
            VCode = DubbeleVerenigingWerdGeregistreerd.VCode,
            VorigeStatus = VerenigingStatus.Actief.Naam,
        };
    }

    public override VCode VCode => VCode.Create(DubbeleVerenigingWerdGeregistreerd.VCode);

    public override IEnumerable<IEvent> Events()
        =>
        [
            DubbeleVerenigingWerdGeregistreerd,
            VerenigingWerdGemarkeerdAlsDubbelVan,
            WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt
        ];
}
