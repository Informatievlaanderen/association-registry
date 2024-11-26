namespace AssociationRegistry.Test.Projections.Beheer.Detail;

using Admin.Schema.Detail;
using FluentAssertions;
using Framework;
using Framework.Fixtures;
using Scenarios;
using Xunit;

[Collection(nameof(ProjectionContext))]
public class Given_LidmaatschapWerdToegevoegd : IClassFixture<BeheerDetailClassFixture<LidmaatschapWerdToegevoegdScenario>>
{
    private readonly BeheerDetailClassFixture<LidmaatschapWerdToegevoegdScenario> _fixture;

    public Given_LidmaatschapWerdToegevoegd(
        BeheerDetailClassFixture<LidmaatschapWerdToegevoegdScenario> fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Metadata_Is_Updated()
        => _fixture.Document.Metadata.Version.Should().Be(2);

    [Fact]
    public void Document_Is_Updated()
    {
        _fixture.Document.Lidmaatschappen[0]
                .Should()
                .BeEquivalentTo(
                     new Lidmaatschap(
                         JsonLdMetadata: null,
                         _fixture.Scenario.LidmaatschapWerdToegevoegd.Lidmaatschap.LidmaatschapId,
                         _fixture.Scenario.LidmaatschapWerdToegevoegd.Lidmaatschap.AndereVereniging,
                         _fixture.Scenario.LidmaatschapWerdToegevoegd.Lidmaatschap.DatumVan,
                         _fixture.Scenario.LidmaatschapWerdToegevoegd.Lidmaatschap.DatumTot,
                         _fixture.Scenario.LidmaatschapWerdToegevoegd.Lidmaatschap.Identificatie,
                         _fixture.Scenario.LidmaatschapWerdToegevoegd.Lidmaatschap.Beschrijving
                     ),
                     config: options => options.Excluding(x => x.JsonLdMetadata));
    }
}
