namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling;

using AutoFixture;
using Events;
using global::AutoFixture;
using Vereniging;

public class LidmaatschapWerdGewijzigdScenario : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009002");
    public readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd;
    public LidmaatschapWerdToegevoegd LidmaatschapWerdToegevoegd { get; }
    public LidmaatschapWerdGewijzigd LidmaatschapWerdGewijzigd { get; set; }

    public LidmaatschapWerdGewijzigdScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd = fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with { VCode = VCode };

        var lidmaatschap = fixture.Create<Registratiedata.Lidmaatschap>()
            with
            {
                LidmaatschapId = new LidmaatschapId(1),
            };

        LidmaatschapWerdToegevoegd =
            new LidmaatschapWerdToegevoegd(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode, lidmaatschap);

        LidmaatschapWerdGewijzigd =
            new LidmaatschapWerdGewijzigd(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode, lidmaatschap with
            {
                Identificatie = fixture.Create<LidmaatschapIdentificatie>(),
            });
    }

    public override IEnumerable<IEvent> Events()
        =>
        [
            VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
            LidmaatschapWerdToegevoegd,
            LidmaatschapWerdGewijzigd,
        ];
}
