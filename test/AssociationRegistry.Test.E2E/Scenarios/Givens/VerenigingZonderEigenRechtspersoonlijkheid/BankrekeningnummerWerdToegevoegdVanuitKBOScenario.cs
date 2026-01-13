namespace AssociationRegistry.Test.E2E.Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;

using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;
using EventStore;
using Framework.TestClasses;

public class BankrekeningnummerWerdToegevoegdVanuitKBOScenario : IScenario
{
    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd { get; set; }
    public BankrekeningnummerWerdToegevoegdVanuitKBO BankrekeningnummerWerdToegevoegdVanuitKBO { get; set; }

    private CommandMetadata Metadata;

    public async Task<KeyValuePair<string, IEvent[]>[]> GivenEvents(IVCodeService service)
    {
        var fixture = new Fixture().CustomizeAdminApi();

        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd = fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
        {
            VCode = await service.GetNext(),
        };

        BankrekeningnummerWerdToegevoegdVanuitKBO = fixture.Create<BankrekeningnummerWerdToegevoegdVanuitKBO>();

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };

        return
        [
            new(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode, [VerenigingMetRechtspersoonlijkheidWerdGeregistreerd, BankrekeningnummerWerdToegevoegdVanuitKBO]),
        ];
    }

    public StreamActionResult Result { get; set; } = null!;

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
