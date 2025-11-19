namespace AssociationRegistry.Test.E2E.Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;

using Admin.Schema.Persoonsgegevens;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;
using EventStore;
using Framework.TestClasses;
using MartenDb.Store;
using Vereniging;

public class VertegenwoordigerWerdToegevoegdScenario : IScenario
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario BaseScenario = new();
    public VertegenwoordigerWerdToegevoegd VertegenwoordigerWerdToegevoegd { get; set; }
    public VertegenwoordigerPersoonsgegevensDocument VertegenwoordigerPersoonsgegevensDocument { get; set; }

    private CommandMetadata Metadata;

    public VertegenwoordigerWerdToegevoegdScenario()
    {
    }

    public async Task<KeyValuePair<string, IEvent[]>[]> GivenEvents(IVCodeService service)
    {
        var fixture = new Fixture().CustomizeAdminApi();
        await BaseScenario.GivenEvents(service);
        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
        var refId = fixture.Create<Guid>();

        VertegenwoordigerWerdToegevoegd = fixture.Create<VertegenwoordigerWerdToegevoegd>() with
        {
            RefId = refId,
        };

        VertegenwoordigerPersoonsgegevensDocument = fixture.Create<VertegenwoordigerPersoonsgegevensDocument>() with
        {
            VCode = BaseScenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode,
            RefId = refId,
            VertegenwoordigerId = VertegenwoordigerWerdToegevoegd.VertegenwoordigerId,
        };

        return
        [
            new(BaseScenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode, [BaseScenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd, VertegenwoordigerWerdToegevoegd]),
        ];
    }

    public VertegenwoordigerPersoonsgegevensDocument[] GivenVertegenwoordigerPersoonsgegevens()
        => BaseScenario.GivenVertegenwoordigerPersoonsgegevens().Append(VertegenwoordigerPersoonsgegevensDocument).ToArray();

    public StreamActionResult Result { get; set; } = null!;

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
