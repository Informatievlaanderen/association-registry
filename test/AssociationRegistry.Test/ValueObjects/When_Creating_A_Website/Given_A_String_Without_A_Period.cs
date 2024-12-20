namespace AssociationRegistry.Test.ValueObjects.When_Creating_A_Website;

using AssociationRegistry.Vereniging.Websites;
using AssociationRegistry.Vereniging.Websites.Exceptions;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_String_Without_A_Period
{
    [Theory]
    [InlineData("http://awebsitewithoutperiods")]
    [InlineData("https://gibberish")]
    public void Then_it_throws_WebsiteMissingPeriodException(string? invalidWebsiteString)
    {
        var ctor = () => Website.Create(invalidWebsiteString);

        ctor.Should().Throw<WebsiteMoetMinstensEenPuntBevatten>();
    }
}
