﻿namespace AssociationRegistry.Test.Projections.Scenario;

using AutoFixture;
using Events;

public class WerkingsgebiedenWerdenBepaaldScenario : ScenarioBase
{
    public FeitelijkeVerenigingWerdGeregistreerd VerenigingWerdGeregistreerd { get; }
    public WerkingsgebiedenWerdenBepaald WerkingsgebiedenWerdenBepaald { get; }

    public WerkingsgebiedenWerdenBepaaldScenario()
    {
        VerenigingWerdGeregistreerd = AutoFixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();

        WerkingsgebiedenWerdenBepaald = new WerkingsgebiedenWerdenBepaald(VerenigingWerdGeregistreerd.VCode, [
            new Registratiedata.Werkingsgebied(Code: "BE25535002", Naam: "Bredene"),
            new Registratiedata.Werkingsgebied(Code: "BE25535005", Naam: "Gistel"),
        ]);
    }

    public override string VCode => VerenigingWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VCode, VerenigingWerdGeregistreerd, WerkingsgebiedenWerdenBepaald),
    ];
}
