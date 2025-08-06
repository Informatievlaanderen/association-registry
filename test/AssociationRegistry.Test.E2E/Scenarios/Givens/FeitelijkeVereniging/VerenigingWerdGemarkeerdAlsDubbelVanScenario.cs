namespace AssociationRegistry.Test.E2E.Scenarios.Givens.FeitelijkeVereniging;

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
    public FeitelijkeVerenigingWerdGeregistreerd AuthentiekeVereniging { get; set; }
    public VerenigingWerdGemarkeerdAlsDubbelVan VerenigingWerdGemarkeerdAlsDubbelVan { get; set; }
    public VerenigingAanvaarddeDubbeleVereniging VerenigingAanvaarddeDubbeleVereniging { get; set; }
    private CommandMetadata Metadata;

    public VerenigingWerdGemarkeerdAlsDubbelVanScenario()
    {
    }

    public async Task<KeyValuePair<string, IEvent[]>[]> GivenEvents(IVCodeService service)
    {
        var fixture = new Fixture().CustomizeAdminApi();

        DubbeleVerenging = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
        {
            VCode = await service.GetNext(),
        };
        AuthentiekeVereniging = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
        {
            VCode = await service.GetNext(),
            Vertegenwoordigers = DubbeleVerenging.Vertegenwoordigers,
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
}
