namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.CommandHandling;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using Framework;
using Vereniging;

public class VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_AllFields_Scenario : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009002");
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;
    public readonly MaatschappelijkeZetelWerdOvergenomenUitKbo MaatschappelijkeZetelWerdOvergenomenUitKbo;

    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_AllFields_Scenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with { VCode = VCode };

        MaatschappelijkeZetelWerdOvergenomenUitKbo = fixture.Create<MaatschappelijkeZetelWerdOvergenomenUitKbo>();
    }

    public override IEnumerable<IEvent> Events()
        => new IEvent[]
        {
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            MaatschappelijkeZetelWerdOvergenomenUitKbo,
        };
}
