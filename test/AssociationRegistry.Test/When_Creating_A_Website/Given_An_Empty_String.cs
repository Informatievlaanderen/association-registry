namespace AssociationRegistry.Test.When_Creating_A_Website;

using ContactInfo.Websites;
using FluentAssertions;
using Xunit;

public class Given_An_Empty_String
{
    [Fact]
    public void Then_it_returns_Null()
    {
        var website = Website.Create(string.Empty);
        website.Should().BeNull();
    }
}
