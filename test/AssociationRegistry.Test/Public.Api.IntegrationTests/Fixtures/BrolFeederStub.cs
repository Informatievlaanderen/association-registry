namespace AssociationRegistry.Test.Public.Api.IntegrationTests.Fixtures;

using System.Collections.Immutable;
using AssociationRegistry.Public.Api.Extensions;
using AssociationRegistry.Public.Api.Projections;

public class BrolFeederStub : IVerenigingBrolFeeder
{
    public string KorteNaam
        => "De korte naam";

    public string Hoofdlocatie
        => "De hoofdlocatie";

    public ImmutableArray<string> Locaties
        => new[] { Hoofdlocatie, "andere locatie" }
            .ToImmutableArray();

    public string[] Hoofdactiviteiten
        => "Buurtwerking".ObjectToArray();

    public string Doelgroep
        => "+18";

    public ImmutableArray<string> Activiteiten
        => new[] { "Basketbal", "Tennis", "Padel" }
            .ToImmutableArray();
}
