namespace AssociationRegistry.Test.Grar.GrarClient;

using Fixtures;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Exact_Match : IClassFixture<WithExactMatchFixture>
{
    private readonly WithExactMatchFixture _fixture;

    public With_Exact_Match(WithExactMatchFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Then_It_Returns_A_SuccessResult()
    {
        _fixture.Result.Should().NotBeEmpty();
        _fixture.Result.Max(r => r.Score).Should().Be(100);
    }
}
