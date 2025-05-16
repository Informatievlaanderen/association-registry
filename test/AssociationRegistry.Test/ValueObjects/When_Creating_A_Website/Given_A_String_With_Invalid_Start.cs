namespace AssociationRegistry.Test.ValueObjects.When_Creating_A_Website;

using AssociationRegistry.Vereniging.Websites;
using AssociationRegistry.Vereniging.Websites.Exceptions;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_String_With_Invalid_Start
{
    [Theory]
    [InlineData("bla.bla.bla")]
    [InlineData("http:/oeps.com")]
    [InlineData("www.hello.me")]
    public void Then_it_throws_InvalidWebsiteStartException(string? invalidWebsiteString)
    {
        var ctor = () => Website.Create(invalidWebsiteString);

        ctor.Should().Throw<WebsiteMoetStartenMetHttps>();
    }
}
