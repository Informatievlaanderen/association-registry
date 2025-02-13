namespace AssociationRegistry.Test.Public.Api.When_Searching.PubliekVerenigingenZoekFilterTests;


using AssociationRegistry.Public.Api.Queries;
using AssociationRegistry.Public.Api.Verenigingen.Search.RequestModels;
using FluentAssertions;
using Xunit;

public class With_Verenigingtype
{
    [Theory]
    [InlineData("verenigingstype.code:FV", "(verenigingstype.code:VZER OR verenigingstype.code:FV)")]
    [InlineData("verenigingstype.code:fv", "(verenigingstype.code:VZER OR verenigingstype.code:FV)")]
    [InlineData("verenigingstype.code: fv ", "(verenigingstype.code:VZER OR verenigingstype.code:FV) ")]
    [InlineData("verenigingstype.code: fv AND verenigingstype.code: vzer",
                "(verenigingstype.code:VZER OR verenigingstype.code:FV) AND (verenigingstype.code:VZER OR verenigingstype.code:FV)")]
    [InlineData("verenigingstype.code: fv AND naam:de grote vereniging AND verenigingstype.code: vzer",
                "(verenigingstype.code:VZER OR verenigingstype.code:FV) AND naam:de grote vereniging AND (verenigingstype.code:VZER OR verenigingstype.code:FV)")]
    [InlineData("naam:de grote vereniging AND verenigingstype.code: fv AND verenigingstype.code: vzer",
                "naam:de grote vereniging AND (verenigingstype.code:VZER OR verenigingstype.code:FV) AND (verenigingstype.code:VZER OR verenigingstype.code:FV)")]
    public void Replaces_Verenigingstype(string input, string expectedOutput)
    {
        var sut = new PubliekVerenigingenZoekFilter(input, null, [], new PaginationQueryParams());

        sut.Query.Should().Be(expectedOutput);
    }
}
