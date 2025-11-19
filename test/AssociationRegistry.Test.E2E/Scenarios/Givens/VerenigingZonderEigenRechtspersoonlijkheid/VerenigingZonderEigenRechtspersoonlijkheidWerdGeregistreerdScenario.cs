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
    public VertegenwoordigerPersoonsgegevensDocument[] VertegenwoordigerPersoonsgegevens;

    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario()
    {

    }

    public async Task<KeyValuePair<string, IEvent[]>[]> GivenEvents(IVCodeService service)
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var geregistreerdeVerenigingMetPersoongegevens = await EventMapper.CreateVzerGeregistreerdMetPersoonsgegevens(service);
        var geregistreerdEvent = geregistreerdeVerenigingMetPersoongegevens.GeregistreerdEvent;
        var vertegenwoordigerPersoonsgegevensDocuments = geregistreerdeVerenigingMetPersoongegevens.PersoonsgegevensDocumenten;
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd = geregistreerdEvent;
        VertegenwoordigerPersoonsgegevens = vertegenwoordigerPersoonsgegevensDocuments
           .ToArray();

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
        return
        [
            new(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode,
                [VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd]),
        ];
    }

    public StreamActionResult Result { get; set; } = null!;

    public VertegenwoordigerPersoonsgegevensDocument[] GivenVertegenwoordigerPersoonsgegevens()
        => VertegenwoordigerPersoonsgegevens;

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
