namespace AssociationRegistry.Test.Public.Api.When_Searching.PubliekVerenigingenZoekFilterTests;

using AssociationRegistry.Public.Api.Queries;
using AssociationRegistry.Public.Api.WebApi.Verenigingen.Search.RequestModels;
using AutoFixture;
using FluentAssertions;
using Xunit;

public class When_Verenigingstype_Is_Non_KBO
{
    private const string ExpandedVerenigingstype = "(verenigingstype.code:VZER OR verenigingstype.code:FV)";

    [Theory]
    [InlineData("verenigingstype.code:FV")]
    [InlineData("verenigingstype.code:fv")]
    [InlineData("verenigingstype.code:Fv")]
    [InlineData("verenigingstype.code:fV")]
    public void Expands_VerenigingsType_With_VZERandFV_With_CaseInsensitivity(string queryInput)
    {
        var expected = ExpandedVerenigingstype;
        var sut = new PubliekVerenigingenZoekFilter(queryInput, null, [], new PaginationQueryParams());
        sut.Query.Should().Be(expected);
    }


    [Theory]
    [InlineData("verenigingstype.code: fv")]
    [InlineData("verenigingstype.code:  fv")]
    public void Expands_VerenigingsType_With_VZERandFV_TrimsSpacesBeforeCode(string queryInput)
    {
        var expected = ExpandedVerenigingstype;
        var sut = new PubliekVerenigingenZoekFilter(queryInput, null, [], new PaginationQueryParams());
        sut.Query.Should().Be(expected);
    }

    [Theory]
    [InlineData("verenigingstype.code:fv ", $"{ExpandedVerenigingstype} ")]
    [InlineData("verenigingstype.code:fv  ", $"{ExpandedVerenigingstype}  ")]
    [InlineData("verenigingstype.code:fv  AND x:y", $"{ExpandedVerenigingstype}  AND x:y")]
    public void Expands_VerenigingsType_With_VZERandFV_DoesNotTrimSpacesAfterCode(string queryInput, string expected)
    {
        var sut = new PubliekVerenigingenZoekFilter(queryInput, null, [], new PaginationQueryParams());
        sut.Query.Should().Be(expected);
    }

    [Fact]
    public void Expands_VerenigingsType_With_VZERandFV_For_Each_Time_It_Occurs()
    {
        var input = "verenigingstype.code: fv AND verenigingstype.code: vzer AND verenigingstype.code: fv";
        var expected = $"{ExpandedVerenigingstype} AND {ExpandedVerenigingstype} AND {ExpandedVerenigingstype}";
        var sut = new PubliekVerenigingenZoekFilter(input, null, [], new PaginationQueryParams());
        sut.Query.Should().Be(expected);
    }

    [Fact]
    public void Expands_VerenigingsType_With_VZERandFV_Without_Touching_Other_QueryParts()
    {
        var fixture = new Fixture();
        var prefix = fixture.Create<string>();
        var postfix = fixture.Create<string>();
        var infix = fixture.Create<string>();
        var input = $"{prefix} AND verenigingstype.code: fv AND {infix} AND verenigingstype.code: vzer AND {postfix}";

        var expected =
            $"{prefix} AND {ExpandedVerenigingstype} AND {infix} AND {ExpandedVerenigingstype} AND {postfix}";

        var sut = new PubliekVerenigingenZoekFilter(input, null, [], new PaginationQueryParams());
        sut.Query.Should().Be(expected);
    }
}
