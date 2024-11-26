namespace AssociationRegistry.Test.Projections.Beheer.Zoeken;

using FluentAssertions;
using Framework;
using Xunit;

[Collection(nameof(ProjectionContext))]
public class Given_WerkingsgebiedenWerdenNietBepaald(WerkingsgebiedenWerdenNietBepaaldFixture fixture)
    : IClassFixture<WerkingsgebiedenWerdenNietBepaaldFixture>
{
    [Fact]
    public void Document_Is_Updated()
        => fixture.Result
                  .Werkingsgebieden
                  .Should().BeEmpty();
}
