namespace AssociationRegistry.Test.Grar.UrlBuilder;

using AssociationRegistry.Integrations.Grar.Clients;
using Xunit;

public class UrlBuilderTests
{
    [Theory]
    [InlineData("/v2/postinfo", "", "", "/v2/postinfo")]
    [InlineData("/v2/postinfo", null, null, "/v2/postinfo")]
    [InlineData("/v2/postinfo", "100", null, "/v2/postinfo?offset=100")]
    [InlineData("/v2/postinfo", null, "100", "/v2/postinfo?limit=100")]
    [InlineData("/v2/postinfo", "100", "100", "/v2/postinfo?offset=100&limit=100")]
    public void Returns_Correctly_Builded_Urls(string baseUrl, string offset, string limit, string expectedUrl)
    {
        var actual = UrlBuilder.Build(baseUrl, offset, limit);
        Assert.Equal(expectedUrl, actual);
    }
}
