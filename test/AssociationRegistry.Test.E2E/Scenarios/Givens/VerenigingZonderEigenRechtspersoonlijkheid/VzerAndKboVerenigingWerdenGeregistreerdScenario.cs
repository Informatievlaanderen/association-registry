namespace AssociationRegistry.Test.E2E.Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;

using Admin.Schema.Persoonsgegevens;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;
using Events.Enriched;
using EventStore;
using Framework.Mappers;
using Framework.TestClasses;
using MartenDb.Store;
using Vereniging;

public class VzerAndKboVerenigingWerdenGeregistreerdScenario : IScenario
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdMetPersoonsgegevens VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdMetPersoonsgegevens { get; set; }
    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd { get; set; }

    private CommandMetadata Metadata;
    private VertegenwoordigerPersoonsgegevensDocument[] _vertegenwoordigerPersoonsgegevens;

    public VzerAndKboVerenigingWerdenGeregistreerdScenario()
    {
    }

    public async Task<KeyValuePair<string, IEvent[]>[]> GivenEvents(IVCodeService service)
    {
        var fixture = new Fixture().CustomizeAdminApi();

        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdMetPersoonsgegevens = fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdMetPersoonsgegevens>() with
        {
            VCode = await service.GetNext(),
        };
        var (verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd, persoonsgegevensDocuments) = VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdMetPersoonsgegevens.MapDomainWithPersoonsgegevens();
        _vertegenwoordigerPersoonsgegevens = persoonsgegevensDocuments
                                            .ToArray();

        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd = fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
        {
            VCode = await service.GetNext(),
            Naam = "Vereniging met rechtspersoonlijkheid",
        };

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };

        return
        [
            new(verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode, [verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd]),
            new(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode, [VerenigingMetRechtspersoonlijkheidWerdGeregistreerd]),
        ];
    }

    public VertegenwoordigerPersoonsgegevensDocument[] GivenVertegenwoordigerPersoonsgegevens()
        => _vertegenwoordigerPersoonsgegevens;

    public StreamActionResult Result { get; set; } = null!;

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
