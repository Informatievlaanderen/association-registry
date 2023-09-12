namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.CommandHandling;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using Framework;
using Vereniging;

public class VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_Contactgegeven_Scenario : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009002");
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;
    public readonly ContactgegevenWerdOvergenomenUitKBO ContactgegevenWerdOvergenomenUitKBO;

    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_Contactgegeven_Scenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with { VCode = VCode };

        ContactgegevenWerdOvergenomenUitKBO = fixture.Create<ContactgegevenWerdOvergenomenUitKBO>();
    }

    public override IEnumerable<IEvent> Events()
        => new IEvent[]
        {
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            ContactgegevenWerdOvergenomenUitKBO,
        };
}
