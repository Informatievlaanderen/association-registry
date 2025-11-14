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

public class VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario : IScenario
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdMetPersoonsgegevens VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdMetPersoonsgegevens { get; set; }
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; set; }

    private CommandMetadata Metadata;
    private VertegenwoordigerPersoonsgegevensDocument[] _vertegenwoordigerPersoonsgegevens;

    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdMetPersoonsgegevens = fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdMetPersoonsgegevens>() with
        {
            VCode = fixture.Create<VCode>(),
        };

        var (verenigingZonderEigenRechtspersoonWerdGeregistreerd, persoonsgegevensDocuments) = VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdMetPersoonsgegevens.MapDomainWithPersoonsgegevens();
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd = verenigingZonderEigenRechtspersoonWerdGeregistreerd;
        _vertegenwoordigerPersoonsgegevens = persoonsgegevensDocuments
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
        => _vertegenwoordigerPersoonsgegevens;

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
