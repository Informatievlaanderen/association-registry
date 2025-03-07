namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using global::AutoFixture;

public class SubtypeWerdVerfijndNaarFeitelijkeVerenigingScenario : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009008");
    public readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd;
    public readonly SubtypeWerdVerfijndNaarFeitelijkeVereniging SubtypeWerdVerfijndNaarFeitelijkeVereniging;
    public SubtypeWerdVerfijndNaarFeitelijkeVerenigingScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd = fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with { VCode = VCode };

        SubtypeWerdVerfijndNaarFeitelijkeVereniging = fixture.Create<SubtypeWerdVerfijndNaarFeitelijkeVereniging>() with
        {
            VCode = VCode,
            Subtype = new Registratiedata.Subtype(Verenigingssubtype.FeitelijkeVereniging.Code, Verenigingssubtype.FeitelijkeVereniging.Naam)
        };
    }

    public override IEnumerable<IEvent> Events()
        => new IEvent[]
        {
            VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,SubtypeWerdVerfijndNaarFeitelijkeVereniging
        };
}
