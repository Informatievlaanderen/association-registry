namespace AssociationRegistry.Test.E2E.Scenarios.Givens.FeitelijkeVereniging;

using Admin.Schema.Persoonsgegevens;
using Events;
using EventStore;
using AssociationRegistry.Test.Common.AutoFixture;
using Vereniging;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using MartenDb.Store;

public class LidmaatschapWerdToegevoegdScenario : Framework.TestClasses.IScenario
{
    public readonly MultipleWerdGeregistreerdScenario BaseScenario = new();
    public string NaamVereniging { get; set; }
    public LidmaatschapWerdToegevoegd LidmaatschapWerdToegevoegd { get; set; }

    public LidmaatschapWerdToegevoegdScenario()
    {
    }

    public async Task<KeyValuePair<string, IEvent[]>[]> GivenEvents(IVCodeService service)
    {
        var fixture = new Fixture().CustomizeAdminApi();
        await BaseScenario.GivenEvents(service);

        NaamVereniging = BaseScenario.FeitelijkeVerenigingWerdGeregistreerd.Naam;

        LidmaatschapWerdToegevoegd = new LidmaatschapWerdToegevoegd(
            VCode: BaseScenario.FeitelijkeVerenigingWerdGeregistreerd.VCode,
            Lidmaatschap: fixture.Create<Registratiedata.Lidmaatschap>() with
            {
                AndereVereniging = BaseScenario.AndereFeitelijkeVerenigingWerdGeregistreerd.VCode,
                AndereVerenigingNaam = BaseScenario.AndereFeitelijkeVerenigingWerdGeregistreerd.Naam,
            });

        return
        [
            new(BaseScenario.AndereFeitelijkeVerenigingWerdGeregistreerd.VCode, [BaseScenario.AndereFeitelijkeVerenigingWerdGeregistreerd, new FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(BaseScenario.AndereFeitelijkeVerenigingWerdGeregistreerd.VCode)]),
            new(BaseScenario.FeitelijkeVerenigingWerdGeregistreerd.VCode, [BaseScenario.FeitelijkeVerenigingWerdGeregistreerd, new FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(BaseScenario.FeitelijkeVerenigingWerdGeregistreerd.VCode), LidmaatschapWerdToegevoegd]),
        ];
    }

    public VertegenwoordigerPersoonsgegevensDocument[] GivenVertegenwoordigerPersoonsgegevens()
        => BaseScenario.GivenVertegenwoordigerPersoonsgegevens();

    public StreamActionResult Result { get; set; } = null!;
}
