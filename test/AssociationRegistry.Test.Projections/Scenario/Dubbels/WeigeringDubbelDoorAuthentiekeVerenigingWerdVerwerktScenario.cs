namespace AssociationRegistry.Test.Projections.Scenario.Dubbels;

using Events;
using Vereniging;
using AutoFixture;
using DecentraalBeheer.Vereniging;

public class WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerktScenario : InszScenarioBase
{
    public FeitelijkeVerenigingWerdGeregistreerd DubbeleVerenigingWerdGeregistreerd { get; }
    public FeitelijkeVerenigingWerdGeregistreerd AuthentiekeVerenigingWerdGeregistreerd { get; }
    public VerenigingWerdGemarkeerdAlsDubbelVan VerenigingWerdGemarkeerdAlsDubbelVan { get; set; }
    public VerenigingAanvaarddeDubbeleVereniging VerenigingAanvaarddeDubbeleVereniging { get; set; }
    public WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt { get; set; }

    private string _insz { get; }
    public WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerktScenario()
    {

        DubbeleVerenigingWerdGeregistreerd = AutoFixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();

        AuthentiekeVerenigingWerdGeregistreerd = AutoFixture.Create<FeitelijkeVerenigingWerdGeregistreerd>()
            with
            {
                Vertegenwoordigers = DubbeleVerenigingWerdGeregistreerd.Vertegenwoordigers,
            };

        _insz = AuthentiekeVerenigingWerdGeregistreerd.Vertegenwoordigers[0].Insz;

        VerenigingWerdGemarkeerdAlsDubbelVan = AutoFixture.Create<VerenigingWerdGemarkeerdAlsDubbelVan>() with
        {
            VCode = DubbeleVerenigingWerdGeregistreerd.VCode,
            VCodeAuthentiekeVereniging = AuthentiekeVerenigingWerdGeregistreerd.VCode,
        };

        VerenigingAanvaarddeDubbeleVereniging = AutoFixture.Create<VerenigingAanvaarddeDubbeleVereniging>() with
        {
            VCode = AuthentiekeVerenigingWerdGeregistreerd.VCode,
            VCodeDubbeleVereniging = DubbeleVerenigingWerdGeregistreerd.VCode,
        };

        WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt =
            AutoFixture.Create<WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt>() with
            {
                VCode = DubbeleVerenigingWerdGeregistreerd.VCode,
                VCodeAuthentiekeVereniging = AuthentiekeVerenigingWerdGeregistreerd.VCode,
                VorigeStatus = VerenigingStatus.Actief.StatusNaam,
            };
    }

    public override string VCode => DubbeleVerenigingWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VCode, DubbeleVerenigingWerdGeregistreerd, VerenigingWerdGemarkeerdAlsDubbelVan, WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt),
        new(AuthentiekeVerenigingWerdGeregistreerd.VCode, AuthentiekeVerenigingWerdGeregistreerd, VerenigingAanvaarddeDubbeleVereniging),
    ];

    public override string Insz => _insz;
}
