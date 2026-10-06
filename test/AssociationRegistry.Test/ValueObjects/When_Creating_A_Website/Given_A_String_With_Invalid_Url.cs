namespace AssociationRegistry.Test.ValueObjects.When_Creating_A_Website;

using DecentraalBeheer.Vereniging.Websites;
using DecentraalBeheer.Vereniging.Websites.Exceptions;
using FluentAssertions;
using Xunit;

public class Given_A_String_With_Invalid_Url
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
    public void Then_it_throws_OngeldigUrl(string? invalidWebsiteString)
    {
        var ctor = () => Website.Create(invalidWebsiteString);

        ctor.Should().Throw<OngeldigUrl>();
    }
}
