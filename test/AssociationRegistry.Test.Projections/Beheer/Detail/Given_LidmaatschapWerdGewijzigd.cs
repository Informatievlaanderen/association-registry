namespace AssociationRegistry.Test.Projections.Beheer.Detail;

using Admin.Schema.Detail;
using FluentAssertions;
using Framework;
using Framework.Fixtures;
using Scenarios;
using Xunit;

[Collection(nameof(ProjectionContext))]
public class Given_LidmaatschapWerdGewijzigd : IClassFixture<BeheerDetailClassFixture<LidmaatschapWerdGewijzigdScenario>>
{
    private readonly BeheerDetailClassFixture<LidmaatschapWerdGewijzigdScenario> _fixture;

    public Given_LidmaatschapWerdGewijzigd(
        BeheerDetailClassFixture<LidmaatschapWerdGewijzigdScenario> fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Metadata_Is_Updated()
        => _fixture.Document.Metadata.Version.Should().Be(3);

    [Fact]
    public void Document_Is_Updated()
    {
       _fixture.Document.Lidmaatschappen[0]
                .Should()
                .BeEquivalentTo(
                     new Lidmaatschap(
                         JsonLdMetadata: null,
                         _fixture.Scenario.LidmaatschapWerdGewijzigd.Lidmaatschap.LidmaatschapId,
                         _fixture.Scenario.LidmaatschapWerdGewijzigd.Lidmaatschap.AndereVereniging,
                         _fixture.Scenario.LidmaatschapWerdGewijzigd.Lidmaatschap.DatumVan,
                         _fixture.Scenario.LidmaatschapWerdGewijzigd.Lidmaatschap.DatumTot,
                         _fixture.Scenario.LidmaatschapWerdGewijzigd.Lidmaatschap.Identificatie,
                         _fixture.Scenario.LidmaatschapWerdGewijzigd.Lidmaatschap.Beschrijving
                     ),
                     config: options => options.Excluding(x => x.JsonLdMetadata));
    }
}
