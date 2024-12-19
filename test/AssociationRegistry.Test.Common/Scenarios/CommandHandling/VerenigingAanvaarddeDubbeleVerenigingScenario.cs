namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using global::AutoFixture;
using Vereniging;

public class VerenigingAanvaarddeDubbeleVerenigingScenario : CommandhandlerScenarioBase
{
    public FeitelijkeVerenigingWerdGeregistreerd AuthentiekeVerenigingWerdGeregistreerd { get; }
    public VerenigingAanvaarddeDubbeleVereniging VerenigingAanvaarddeDubbeleVereniging { get; set; }

    public VerenigingAanvaarddeDubbeleVerenigingScenario()
    {
        var fixture = new Fixture().CustomizeDomain();

        AuthentiekeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();

        VerenigingAanvaarddeDubbeleVereniging = fixture.Create<VerenigingAanvaarddeDubbeleVereniging>() with
        {
            VCode = AuthentiekeVerenigingWerdGeregistreerd.VCode,
        };
    }

    public override VCode VCode => VCode.Create(AuthentiekeVerenigingWerdGeregistreerd.VCode);

    public override IEnumerable<IEvent> Events()
        =>
        [
            AuthentiekeVerenigingWerdGeregistreerd,
            VerenigingAanvaarddeDubbeleVereniging,
        ];
}
