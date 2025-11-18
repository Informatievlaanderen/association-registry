namespace AssociationRegistry.Test.E2E.Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;

using Admin.Schema.Persoonsgegevens;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;
using EventStore;
using Framework.Mappers;
using Framework.TestClasses;

public class VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario : IScenario
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get;
        set;
    }
    private CommandMetadata Metadata;
    public readonly VertegenwoordigerPersoonsgegevensDocument[] VertegenwoordigerPersoonsgegevens;

    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var geregistreerdeVerenigingMetPersoongegevens = EventMapper.CreateVzerGeregistreerdMetPersoonsgegevens();
        var geregistreerdEvent = geregistreerdeVerenigingMetPersoongegevens.GeregistreerdEvent;
        var vertegenwoordigerPersoonsgegevensDocuments = geregistreerdeVerenigingMetPersoongegevens.PersoonsgegevensDocumenten;
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd = geregistreerdEvent;
        VertegenwoordigerPersoonsgegevens = vertegenwoordigerPersoonsgegevensDocuments
           .ToArray();

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public async Task<KeyValuePair<string, IEvent[]>[]> GivenEvents(IVCodeService service)
        =>
        [
            new(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode, [VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd]),
        ];

    public StreamActionResult Result { get; set; } = null!;

    public VertegenwoordigerPersoonsgegevensDocument[] GivenVertegenwoordigerPersoonsgegevens()
        => VertegenwoordigerPersoonsgegevens;

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
