﻿namespace AssociationRegistry.Test.Projections.Publiek.Detail;

using Public.Schema.Detail;

[Collection(nameof(ProjectionContext))]
public class Given_LidmaatschapWerdToegevoegd(LidmaatschapWerdToegevoegdFixture fixture) : IClassFixture<LidmaatschapWerdToegevoegdFixture>
{
    [Fact]
    public void Document_Is_Updated()
        => fixture.Result
                  .Lidmaatschappen[0]
                  .Should()
                  .BeEquivalentTo(
                       new PubliekVerenigingDetailDocument.Lidmaatschap(
                           JsonLdMetadata: null,
                           fixture.Scenario.LidmaatschapWerdToegevoegd.Lidmaatschap.LidmaatschapId,
                           fixture.Scenario.LidmaatschapWerdToegevoegd.Lidmaatschap.AndereVereniging,
                           fixture.Scenario.LidmaatschapWerdToegevoegd.Lidmaatschap.DatumVan,
                           fixture.Scenario.LidmaatschapWerdToegevoegd.Lidmaatschap.DatumTot,
                           fixture.Scenario.LidmaatschapWerdToegevoegd.Lidmaatschap.Identificatie,
                           fixture.Scenario.LidmaatschapWerdToegevoegd.Lidmaatschap.Beschrijving
                       ),
                       config: options => options.Excluding(x => x.JsonLdMetadata));
}
