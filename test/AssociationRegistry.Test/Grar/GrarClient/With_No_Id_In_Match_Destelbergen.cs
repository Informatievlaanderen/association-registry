namespace AssociationRegistry.Test.Grar.GrarClient;

using Fixtures;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_No_Id_In_Match_Destelbergen : IClassFixture<WithNoIdInMatchDestelbergenFixture>
{
    private readonly WithNoIdInMatchDestelbergenFixture _fixture;

    public With_No_Id_In_Match_Destelbergen(WithNoIdInMatchDestelbergenFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Then_It_Returns_No_Matches()
    {
        _fixture.Result.Should().BeEmpty();
    }
}
