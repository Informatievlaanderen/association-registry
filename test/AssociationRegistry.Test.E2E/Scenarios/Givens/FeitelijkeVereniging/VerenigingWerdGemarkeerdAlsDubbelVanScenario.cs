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
    public MultipleWerdGeregistreerdScenario MultipleWerdGeregistreerdScenario = new ();

    public VerenigingWerdGemarkeerdAlsDubbelVan VerenigingWerdGemarkeerdAlsDubbelVan { get; set; }
    public VerenigingAanvaarddeDubbeleVereniging VerenigingAanvaarddeDubbeleVereniging { get; set; }
    private CommandMetadata Metadata;

    public VerenigingWerdGemarkeerdAlsDubbelVanScenario()
    {
    }

    public async Task<KeyValuePair<string, IEvent[]>[]> GivenEvents(IVCodeService service)
    {
        var fixture = new Fixture().CustomizeAdminApi();
        await MultipleWerdGeregistreerdScenario.GivenEvents(service);
        VerenigingWerdGemarkeerdAlsDubbelVan = fixture.Create<VerenigingWerdGemarkeerdAlsDubbelVan>() with
        {
            VCode = MultipleWerdGeregistreerdScenario.FeitelijkeVerenigingWerdGeregistreerd.VCode,
            VCodeAuthentiekeVereniging = MultipleWerdGeregistreerdScenario.AndereFeitelijkeVerenigingWerdGeregistreerd.VCode,
        };

        VerenigingAanvaarddeDubbeleVereniging = fixture.Create<VerenigingAanvaarddeDubbeleVereniging>() with
        {
            VCode = MultipleWerdGeregistreerdScenario.AndereFeitelijkeVerenigingWerdGeregistreerd.VCode,
            VCodeDubbeleVereniging = MultipleWerdGeregistreerdScenario.FeitelijkeVerenigingWerdGeregistreerd.VCode,
        };

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };

        return
        [
            new(MultipleWerdGeregistreerdScenario.FeitelijkeVerenigingWerdGeregistreerd.VCode, [MultipleWerdGeregistreerdScenario.FeitelijkeVerenigingWerdGeregistreerd, VerenigingWerdGemarkeerdAlsDubbelVan, new FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(MultipleWerdGeregistreerdScenario.FeitelijkeVerenigingWerdGeregistreerd.VCode)]),
            new(MultipleWerdGeregistreerdScenario.AndereFeitelijkeVerenigingWerdGeregistreerd.VCode, [MultipleWerdGeregistreerdScenario.AndereFeitelijkeVerenigingWerdGeregistreerd, VerenigingAanvaarddeDubbeleVereniging,new FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(MultipleWerdGeregistreerdScenario.AndereFeitelijkeVerenigingWerdGeregistreerd.VCode)]),
        ];
    }

    public StreamActionResult Result { get; set; } = null!;

    public CommandMetadata GetCommandMetadata()
        => Metadata;

    public VertegenwoordigerPersoonsgegevensDocument[] GivenVertegenwoordigerPersoonsgegevens()
        => MultipleWerdGeregistreerdScenario.VertegenwoordigerPersoonsgegevens.Concat(MultipleWerdGeregistreerdScenario.AndereVertegenwoordigerPersoonsgegevens).ToArray();
}
