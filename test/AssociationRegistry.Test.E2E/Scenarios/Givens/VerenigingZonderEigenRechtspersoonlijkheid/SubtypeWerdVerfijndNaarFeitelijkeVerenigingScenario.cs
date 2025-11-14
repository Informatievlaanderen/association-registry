namespace AssociationRegistry.Test.E2E.Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;

using Admin.Schema.Persoonsgegevens;
using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;
using Events.Enriched;
using EventStore;
using Framework.Mappers;
using Framework.TestClasses;

public class SubtypeWerdVerfijndNaarFeitelijkeVerenigingScenario : IScenario
{
    private VertegenwoordigerPersoonsgegevensDocument[] _vertegenwoordigerPersoonsgegevens;
    public string NaamVereniging { get; set; }
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdMetPersoonsgegevens VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdMetPersoonsgegevens { get;
        set;
    }
    public VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging { get; set; }

    public SubtypeWerdVerfijndNaarFeitelijkeVerenigingScenario()
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

        VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging = new VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging(
            VCode: verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode);

        return
        [
            new(verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode, [verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd, VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging]),
        ];
    }

    public VertegenwoordigerPersoonsgegevensDocument[] GivenVertegenwoordigerPersoonsgegevens()
        => _vertegenwoordigerPersoonsgegevens;

    public StreamActionResult Result { get; set; } = null!;

}
