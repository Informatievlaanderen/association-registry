namespace AssociationRegistry.Test.E2E.Scenarios.Givens.Relatieaanvragen;

using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;
using Framework.TestClasses;

/// <summary>
///     A koepel (vereniging met rechtspersoonlijkheid, identified as koepel) and a VZER, plus another koepel for the
///     rules that look at the relatieaanvragen with every koepel.
/// </summary>
public class KoepelEnVerenigingScenario : IScenario
{
    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd KoepelWerdGeregistreerd { get; private set; } = null!;
    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd AndereKoepelWerdGeregistreerd { get; private set; } =
        null!;
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingWerdGeregistreerd
    {
        get;
        private set;
    } = null!;

    public string KoepelVCode => KoepelWerdGeregistreerd.VCode;
    public string AndereKoepelVCode => AndereKoepelWerdGeregistreerd.VCode;
    public string VerenigingVCode => VerenigingWerdGeregistreerd.VCode;

    private CommandMetadata Metadata = null!;

    public async Task<KeyValuePair<string, IEvent[]>[]> GivenEvents(IVCodeService service)
    {
        var fixture = new Fixture().CustomizeAdminApi();

        KoepelWerdGeregistreerd = fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
        {
            VCode = await service.GetNext(),
            Rechtsvorm = Verenigingstype.VZW.Code,
        };

        AndereKoepelWerdGeregistreerd = fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
        {
            VCode = await service.GetNext(),
            Rechtsvorm = Verenigingstype.VZW.Code,
        };

        VerenigingWerdGeregistreerd = fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
        {
            VCode = await service.GetNext(),
        };

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };

        return
        [
            new(KoepelVCode, [KoepelWerdGeregistreerd, new VerenigingWerdGeidentificeerdAlsKoepel(KoepelVCode)]),
            new(
                AndereKoepelVCode,
                [AndereKoepelWerdGeregistreerd, new VerenigingWerdGeidentificeerdAlsKoepel(AndereKoepelVCode)]
            ),
            new(VerenigingVCode, [VerenigingWerdGeregistreerd]),
        ];
    }

    public CommandMetadata GetCommandMetadata() => Metadata;
}
