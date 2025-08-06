namespace AssociationRegistry.Test.E2E.Scenarios.Givens.MetRechtspersoonlijkheid;

using Events;
using EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using Vereniging;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using MartenDb.Store;
using Requests.VerenigingMetRechtspersoonlijkheid;


public class VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario : IVerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario,
                                                                           Framework.TestClasses.IScenario
{
    private readonly bool _isUitgeschreven;
    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd { get; set; }
    private CommandMetadata Metadata;
    public VCode VCode { get; private set; }

    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario(bool isUitgeschreven = false)
    {
        _isUitgeschreven = isUitgeschreven;
    }

    public async Task<KeyValuePair<string, IEvent[]>[]> GivenEvents(IVCodeService service)
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VCode = await service.GetNext();

        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerd(
            VCode,
            KboNummer: "0554790609",
            Rechtsvorm: Verenigingstype.VZW.Code,
            Naam: "Abstrkt Events",
            KorteNaam: "Abstrkt",
            Startdatum: DateOnly.FromDateTime(DateTime.Now)
        );

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };

        return [
            new(VCode, [VerenigingMetRechtspersoonlijkheidWerdGeregistreerd])
        ];
    }

    public IEvent[] GivenEvents()
        => [VerenigingMetRechtspersoonlijkheidWerdGeregistreerd];

    public StreamActionResult Result { get; set; } = null!;

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
