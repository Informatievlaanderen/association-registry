namespace AssociationRegistry.Test.E2E.Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;

using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;
using EventStore;
using Framework.TestClasses;

public class BankrekeningnummerWerdToegevoegdScenario : IScenario
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; set; }
    public BankrekeningnummerValidatieWerdOngedaanGemaaktDoorWijzigingTitularis BankrekeningnummerValidatieWerdOngedaanGemaaktDoorWijzigingTitularis { get; set; }
    public BankrekeningnummerWerdToegevoegd BankrekeningnummerWerdToegevoegd { get; set; }

    private CommandMetadata Metadata;

    public async Task<KeyValuePair<string, IEvent[]>[]> GivenEvents(IVCodeService service)
    {
        var fixture = new Fixture().CustomizeAdminApi();

        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd = fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
        {
            VCode = await service.GetNext(),
        };

        // To test if State works without applying this event
        BankrekeningnummerValidatieWerdOngedaanGemaaktDoorWijzigingTitularis =
            fixture.Create<BankrekeningnummerValidatieWerdOngedaanGemaaktDoorWijzigingTitularis>() with
            {
                BankrekeningnummerId = VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Bankrekeningnummers.First().BankrekeningnummerId
            };

        BankrekeningnummerWerdToegevoegd = fixture.Create<BankrekeningnummerWerdToegevoegd>();

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };

        return
        [
            new(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode, [VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd, BankrekeningnummerWerdToegevoegd]),
        ];
    }

    public StreamActionResult Result { get; set; } = null!;

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
