namespace AssociationRegistry.Test.Projections.Scenario.Dubbels;

using Admin.Schema.Persoonsgegevens;
using Events;
using AutoFixture;
using DecentraalBeheer.Vereniging;

public class VerenigingWerdGemarkeerdAlsDubbelVanScenario : InszScenarioBase
{
    public FeitelijkeVerenigingWerdGeregistreerd DubbeleVerenging { get; set; }
    public VertegenwoordigerPersoonsgegevensDocument DubbeleVerenigingPersoonsGegevens { get; set; }
    public FeitelijkeVerenigingWerdGeregistreerd AuthentiekeVereniging { get; set; }
    public VertegenwoordigerPersoonsgegevensDocument AuthentiekeVerenigingPersoonsGegevens { get; set; }
    public VerenigingWerdGemarkeerdAlsDubbelVan VerenigingWerdGemarkeerdAlsDubbelVan { get; set; }
    public VerenigingAanvaarddeDubbeleVereniging VerenigingAanvaarddeDubbeleVereniging { get; set; }

    private string _insz { get; }
    public VerenigingWerdGemarkeerdAlsDubbelVanScenario()
    {

        var vCodeDubbeleVereniging = AutoFixture.Create<VCode>();
        var refIdDubbeleVereniging = Guid.NewGuid();
        DubbeleVerenigingPersoonsGegevens = AutoFixture.Create<VertegenwoordigerPersoonsgegevensDocument>() with
        {
            VCode = vCodeDubbeleVereniging,
            RefId = refIdDubbeleVereniging,
        };
        DubbeleVerenging = AutoFixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
        {
            VCode = vCodeDubbeleVereniging,
            Vertegenwoordigers = new []{new Registratiedata.Vertegenwoordiger(refIdDubbeleVereniging, DubbeleVerenigingPersoonsGegevens.VertegenwoordigerId, false)}
        };

        var vCodeAuthentiekeVereniging = AutoFixture.Create<VCode>();
        var refIdAuthentiekeVereniging = Guid.NewGuid();
        AuthentiekeVerenigingPersoonsGegevens = AutoFixture.Create<VertegenwoordigerPersoonsgegevensDocument>() with
        {
            VCode = vCodeAuthentiekeVereniging,
            RefId = refIdAuthentiekeVereniging,
        };
        AuthentiekeVereniging = AutoFixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
        {
            VCode = vCodeAuthentiekeVereniging,
            Vertegenwoordigers = new []{new Registratiedata.Vertegenwoordiger(refIdAuthentiekeVereniging, AuthentiekeVerenigingPersoonsGegevens.VertegenwoordigerId, false)}
        };

        _insz = AuthentiekeVerenigingPersoonsGegevens.Insz;

        VerenigingWerdGemarkeerdAlsDubbelVan = AutoFixture.Create<VerenigingWerdGemarkeerdAlsDubbelVan>() with
        {
            VCode = DubbeleVerenging.VCode,
            VCodeAuthentiekeVereniging = AuthentiekeVereniging.VCode,
        };

        VerenigingAanvaarddeDubbeleVereniging = AutoFixture.Create<VerenigingAanvaarddeDubbeleVereniging>() with
        {
            VCode = AuthentiekeVereniging.VCode,
            VCodeDubbeleVereniging = DubbeleVerenging.VCode,
        };
    }

    public override string VCode => DubbeleVerenging.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VCode, DubbeleVerenging, VerenigingWerdGemarkeerdAlsDubbelVan),
        new(AuthentiekeVereniging.VCode, AuthentiekeVereniging, VerenigingAanvaarddeDubbeleVereniging),
    ];

    public override string Insz => _insz;
}
