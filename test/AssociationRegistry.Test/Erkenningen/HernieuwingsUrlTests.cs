namespace AssociationRegistry.Test.Erkenningen;

using DecentraalBeheer.Vereniging.Erkenningen;
using DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using DecentraalBeheer.Vereniging.Websites.Exceptions;
using FluentAssertions;
using Xunit;

public class HernieuwingsUrlTests
{
    [Fact]
    public void Given_Null_Url_Then_Create_Returns_Empty_String()
    {
        string? url = null;

        var result = HernieuwingsUrl.Create(url);

        result.Value.Should().Be(string.Empty);
    }

    [Fact]
    public void Given_Empty_Url_Then_Create_Returns_Empty_String()
    {
        var url = string.Empty;

        var result = HernieuwingsUrl.Create(url);

        result.Value.Should().Be(string.Empty);
    }

    [Fact]
    public void Given_Http_Url_Then_Create_Succeeds()
    {
        var url = "http://example.com";

        var result = HernieuwingsUrl.Create(url);

        result.Value.Should().Be(url);
    }

    [Fact]
    public void Given_Https_Url_Then_Create_Succeeds()
    {
        var url = "https://example.com";

        var result = HernieuwingsUrl.Create(url);

        result.Value.Should().Be(url);
    }

    [Fact]
    public void Given_Http_Url_With_Path_Then_Create_Succeeds()
    {
        var url = "http://example.com/path";

        var result = HernieuwingsUrl.Create(url);

        result.Value.Should().Be(url);
    }

    [Fact]
    public void Given_Https_Url_With_Query_Then_Create_Succeeds()
    {
        var url = "https://example.com/path?x=1";

        var result = HernieuwingsUrl.Create(url);

        result.Value.Should().Be(url);
    }

    [Fact]
    public void Given_Www_Url_With_Query_Then_Throws_WebsiteMoetStartenMetHttps()
    {
        var url = "www.example.com/path?x=1";

        Assert.Throws<WebsiteMoetStartenMetHttps>(() => HernieuwingsUrl.Create(url));
    }

    [Fact]
    public void Given_Url_With_Ftp_Scheme_Then_Throws_WebsiteMoetStartenMetHttps()
    {
        var url = "ftp://example.com";

        Assert.Throws<WebsiteMoetStartenMetHttps>(() => HernieuwingsUrl.Create(url));
    }

    [Fact]
    public void Given_Url_Without_Scheme_Then_Throws_WebsiteMoetStartenMetHttps()
    {
        var url = "example.com";

        Assert.Throws<WebsiteMoetStartenMetHttps>(() => HernieuwingsUrl.Create(url));
    }

    [Fact]
    public void Given_Url_With_Only_Https_Scheme_Then_Throws_OngeldigUrl()
    {
        var url = "https://";

        Assert.Throws<OngeldigUrl>(() => HernieuwingsUrl.Create(url));
    }

    [Fact]
    public void Given_Url_With_Only_Http_Scheme_Then_Throws_OngeldigUrl()
    {
        var url = "http://";

        Assert.Throws<OngeldigUrl>(() => HernieuwingsUrl.Create(url));
    }


    [Fact]
    public void Given_Url_With_Only_HttpDot_Scheme_Then_Throws_OngeldigUrl()
    {
        var url = "http.";

        Assert.Throws<WebsiteMoetStartenMetHttps>(() => HernieuwingsUrl.Create(url));
    }

    [Fact]
    public void Given_Relative_Url_Then_Throws_WebsiteMoetStartenMetHttps()
    {
        var url = "/relative/path";

        Assert.Throws<WebsiteMoetStartenMetHttps>(() => HernieuwingsUrl.Create(url));
    }

    [Fact]
    public void Given_Whitespace_Url_Then_Throws_WebsiteMoetStartenMetHttps()
    {
        var url = "      ";

        Assert.Throws<WebsiteMoetStartenMetHttps>(() => HernieuwingsUrl.Create(url));
    }
}
