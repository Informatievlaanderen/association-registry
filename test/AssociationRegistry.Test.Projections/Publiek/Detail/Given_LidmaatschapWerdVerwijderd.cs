namespace AssociationRegistry.Test.Projections.Publiek.Detail;

using FluentAssertions;
using Framework;
using Xunit;

[Collection(nameof(ProjectionContext))]
public class Given_LidmaatschapWerdVerwijderd(LidmaatschapWerdVerwijderdFixture fixture) : IClassFixture<LidmaatschapWerdVerwijderdFixture>
{
    [Fact]
    public void Document_Is_Updated()
        => fixture.Result
                  .Lidmaatschappen.Should().BeEmpty();
}
