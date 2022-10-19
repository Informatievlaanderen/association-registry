namespace AssociationRegistry.Test.Public.Api.IntegrationTests.Fixtures;

using AssociationRegistry.Public.Api.Projections;

public class BrolFeederStub : IVerenigingBrolFeeder
{
    public string KorteNaam
        => "De korte naam";

    public string Hoofdlocatie
        => "De hoofdlocatie";

    public string AndereLocaties
        => "andere locaties";

    public string Hoofdactiviteit
        => "Buurtwerking";

    public string Doelgroep
        => "+18";
}
