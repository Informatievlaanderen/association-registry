namespace AssociationRegistry.Test.E2E.Scenarios.Givens.FeitelijkeVereniging;

using Admin.Schema.Persoonsgegevens;
using Events;
using EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using Vereniging;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using MartenDb.Store;

public class VerenigingWerdGemarkeerdAlsDubbelVanScenario : Framework.TestClasses.IScenario
{
    public FeitelijkeVerenigingWerdGeregistreerd DubbeleVerenging { get; set; }
    public VertegenwoordigerPersoonsgegevensDocument DubbeleVerenigingPersoonsGegevens { get; set; }
    public FeitelijkeVerenigingWerdGeregistreerd AuthentiekeVereniging { get; set; }
    public VertegenwoordigerPersoonsgegevensDocument AuthentiekeVerenigingPersoonsGegevens { get; set; }

    public VerenigingWerdGemarkeerdAlsDubbelVan VerenigingWerdGemarkeerdAlsDubbelVan { get; set; }
    public VerenigingAanvaarddeDubbeleVereniging VerenigingAanvaarddeDubbeleVereniging { get; set; }
    private CommandMetadata Metadata;

    public VerenigingWerdGemarkeerdAlsDubbelVanScenario()
    {
    }

    public async Task<KeyValuePair<string, IEvent[]>[]> GivenEvents(IVCodeService service)
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var vCodeDubbeleVereniging = await service.GetNext();
        var refIdDubbeleVereniging = Guid.NewGuid();
        DubbeleVerenigingPersoonsGegevens = fixture.Create<VertegenwoordigerPersoonsgegevensDocument>() with
        {
            VCode = vCodeDubbeleVereniging,
            RefId = refIdDubbeleVereniging,
        };
        DubbeleVerenging = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
        {
            VCode = vCodeDubbeleVereniging,
            Vertegenwoordigers = new []{new Registratiedata.Vertegenwoordiger(refIdDubbeleVereniging, DubbeleVerenigingPersoonsGegevens.VertegenwoordigerId, false)}
        };

        var vCodeAuthentiekeVereniging = await service.GetNext();
        var refIdAuthentiekeVereniging = Guid.NewGuid();
        AuthentiekeVerenigingPersoonsGegevens = fixture.Create<VertegenwoordigerPersoonsgegevensDocument>() with
        {
            VCode = vCodeAuthentiekeVereniging,
            RefId = refIdAuthentiekeVereniging,
        };
        AuthentiekeVereniging = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
        {
            VCode = vCodeAuthentiekeVereniging,
            Vertegenwoordigers = new []{new Registratiedata.Vertegenwoordiger(refIdAuthentiekeVereniging, AuthentiekeVerenigingPersoonsGegevens.VertegenwoordigerId, false)}
        };

        VerenigingWerdGemarkeerdAlsDubbelVan = fixture.Create<VerenigingWerdGemarkeerdAlsDubbelVan>() with
        {
            VCode = DubbeleVerenging.VCode,
            VCodeAuthentiekeVereniging = AuthentiekeVereniging.VCode,
        };

        VerenigingAanvaarddeDubbeleVereniging = fixture.Create<VerenigingAanvaarddeDubbeleVereniging>() with
        {
            VCode = AuthentiekeVereniging.VCode,
            VCodeDubbeleVereniging = DubbeleVerenging.VCode,
        };


        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };

        return
        [
            new(DubbeleVerenging.VCode, [DubbeleVerenging, VerenigingWerdGemarkeerdAlsDubbelVan, new FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(DubbeleVerenging.VCode)]),
            new(AuthentiekeVereniging.VCode, [AuthentiekeVereniging, VerenigingAanvaarddeDubbeleVereniging,new FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(AuthentiekeVereniging.VCode)]),
        ];
    }

    public StreamActionResult Result { get; set; } = null!;

    public CommandMetadata GetCommandMetadata()
        => Metadata;

    public VertegenwoordigerPersoonsgegevensDocument[] GivenVertegenwoordigerPersoonsgegevens()
        => [DubbeleVerenigingPersoonsGegevens, AuthentiekeVerenigingPersoonsGegevens];
}
