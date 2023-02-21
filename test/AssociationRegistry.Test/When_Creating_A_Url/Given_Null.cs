namespace AssociationRegistry.Test.When_Creating_A_Url;

using AssociationRegistry.ContactInfo.Urls;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_Null
{
    [Fact]
    public void Then_it_returns_Null()
    {
        var website = Url.Create(null!);
        website.Should().BeNull();
    }
}
