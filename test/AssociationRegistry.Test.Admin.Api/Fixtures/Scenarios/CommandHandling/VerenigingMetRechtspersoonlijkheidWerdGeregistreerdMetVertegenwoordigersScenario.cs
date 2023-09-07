namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.CommandHandling;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using Framework;
using Vereniging;

public class VerenigingMetRechtspersoonlijkheidWerdGeregistreerdMetVertegenwoordigersScenario : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009003");
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;
    public readonly VertegenwoordigerWerdOvergenomenUitKBO VertegenwoordigerWerdOvergenomenUitKbo;

    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerdMetVertegenwoordigersScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with { VCode = VCode };

        VertegenwoordigerWerdOvergenomenUitKbo = new VertegenwoordigerWerdOvergenomenUitKBO(
            VertegenwoordigerId: 1,
            fixture.Create<Insz>(),
            fixture.Create<Voornaam>(),
            fixture.Create<Achternaam>());
    }

    public override IEnumerable<IEvent> Events()
        => new IEvent[]
        {
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            VertegenwoordigerWerdOvergenomenUitKbo,
        };
}
