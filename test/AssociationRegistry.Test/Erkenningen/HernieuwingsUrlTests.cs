namespace AssociationRegistry.Test.Erkenningen;

using DecentraalBeheer.Vereniging.Erkenningen;
using DecentraalBeheer.Vereniging.Websites.Exceptions;
using FluentAssertions;
using Xunit;

public class HernieuwingsUrlTests
{
    [Theory]
    [InlineData("http:/oeps.com")]
    [InlineData("http//oeps.com")]
    [InlineData("http.")]
    [InlineData("http://")]
    [InlineData("https://")]
    [InlineData("ftp://example.com")]
    [InlineData("mailto:someone@example.com")]
    [InlineData("/relative/path")]
    [InlineData("   ")]
    [InlineData(".com")]
    [InlineData("example.")]
    [InlineData("http://.com")]
    [InlineData("http://example.")]
    [InlineData("exa mple.com")]
    [InlineData("http://awebsitewithoutperiods")]
    [InlineData("https://gibberish")]
    [InlineData("gibberish")]
    [InlineData("     ")]
    public void Given_Relative_Url_Then_Throws_WebsiteMoetStartenMetHttps(string url)
    {
        Assert.Throws<OngeldigUrl>(() => HernieuwingsUrl.Create(url));
    }

    [Theory]
    [InlineData("http://www.my-domain.com")]
    [InlineData("https://www.my-domain.com")]
    [InlineData("https://www.my-other-domain.be")]
    [InlineData("https://www.sub.domain.be")]
    [InlineData("https://sub.domain.be")]
    [InlineData("https://domain.be")]
    [InlineData("HTTPS://DOMAIN.BE")]
    [InlineData("www.hello.me")]
    [InlineData("google.com")]
    [InlineData("bla.bla.bla")]
    [InlineData("example.com/path?x=1")]
    [InlineData("www.example.com/path?x=1")]
    [InlineData("https://example.com/path?x=1#frag")]
    [InlineData("https://example.com:8080")]
    public void Given_Valid_Url_Then_Create_Succeeds(string url)
    {
        var result = HernieuwingsUrl.Create(url);

        result.Value.Should().Be(url);
    }
}
