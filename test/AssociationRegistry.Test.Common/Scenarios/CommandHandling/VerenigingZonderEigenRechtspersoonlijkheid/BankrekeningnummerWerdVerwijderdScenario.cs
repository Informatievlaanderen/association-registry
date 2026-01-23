namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;

using AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;
using global::AutoFixture;

public class BankrekeningnummerWerdVerwijderdScenario : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009002");
    public readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd;
    public readonly BankrekeningnummerWerdToegevoegd BankrekeningnummerWerdToegevoegd;
    public readonly BankrekeningnummerWerdVerwijderd BankrekeningnummerWerdVerwijderd;

    public BankrekeningnummerWerdVerwijderdScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd = fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with { VCode = VCode };

        BankrekeningnummerWerdToegevoegd = fixture.Create<BankrekeningnummerWerdToegevoegd>() with
        {
            Iban = "BE68539007547034",
        };

        BankrekeningnummerWerdVerwijderd = fixture.Create<BankrekeningnummerWerdVerwijderd>() with
        {
            BankrekeningnummerId = BankrekeningnummerWerdToegevoegd.BankrekeningnummerId,
            Iban = "BE68539007547034",
        };
    }

    public override IEnumerable<IEvent> Events()
        => new IEvent[]
        {
            VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
            BankrekeningnummerWerdToegevoegd,
            BankrekeningnummerWerdVerwijderd,
        };
}
