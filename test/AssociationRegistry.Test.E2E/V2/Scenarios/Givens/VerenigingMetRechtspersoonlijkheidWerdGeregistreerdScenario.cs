namespace AssociationRegistry.Test.E2E.V2.Scenarios.Givens;

using AssociationRegistry.Events;
using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Requests;

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

    public async Task<Dictionary<string, IEvent[]>> GivenEvents(IVCodeService service)
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VCode = await service.GetNext();

        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerd(
            VCode,
            KboNummer: "",
            Rechtsvorm: "",
            Naam: "Feestcommittee Oudenaarde",
            KorteNaam: "FOud",
            Startdatum: DateOnly.FromDateTime(new DateTime(year: 2022, month: 11, day: 9))
        );

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };

        return new Dictionary<string, IEvent[]>()
        {
            { VCode, [VerenigingMetRechtspersoonlijkheidWerdGeregistreerd] },
        };
    }

    public IEvent[] GivenEvents()
        => [VerenigingMetRechtspersoonlijkheidWerdGeregistreerd];

    public StreamActionResult Result { get; set; } = null!;

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
