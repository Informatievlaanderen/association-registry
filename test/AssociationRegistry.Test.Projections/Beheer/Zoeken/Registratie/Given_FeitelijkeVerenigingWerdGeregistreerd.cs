namespace AssociationRegistry.Test.Projections.Beheer.Zoeken.Registratie;

using Admin.Schema;
using Admin.Schema.Search;
using Contracts.JsonLdContext;
using Scenario.Werkingsgebieden;

[Collection(nameof(ProjectionContext))]
public class Given_FeitelijkeVerenigingWerdGeregistreerd(BeheerZoekenScenarioFixture<WerkingsgebiedenWerdenBepaaldScenario> fixture)
    : BeheerZoekenScenarioClassFixture<WerkingsgebiedenWerdenBepaaldScenario>
{



    [Fact]
    public void Document_Is_Updated()
        => fixture.Result
                  .Werkingsgebieden
                  .Should().BeEquivalentTo(fixture.Scenario.WerkingsgebiedenWerdenBepaald.Werkingsgebieden.Select(
                                               s => new VerenigingZoekDocument.Types.Werkingsgebied
                                               {
                                                   JsonLdMetadata = new JsonLdMetadata(
                                                       JsonLdType.Werkingsgebied.CreateWithIdValues(s.Code),
                                                       JsonLdType.Werkingsgebied.Type),
                                                   Code = s.Code,
                                                   Naam = s.Naam,
                                               }));
}
