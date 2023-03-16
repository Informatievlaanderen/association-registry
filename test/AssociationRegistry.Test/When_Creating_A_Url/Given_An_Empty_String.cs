namespace AssociationRegistry.Test.When_Creating_A_Url;

using ContactInfo.Urls;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_An_Empty_String
{
    [Fact]
    public void Then_it_returns_Null()
    {
        var website = Url.Create(string.Empty);
        website.Should().BeNull();
    }
}
