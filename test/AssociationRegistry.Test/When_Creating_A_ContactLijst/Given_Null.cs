namespace AssociationRegistry.Test.When_Creating_A_ContactLijst;

using ContactInfo;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_Null
{
    [Fact]
    public void Then_It_Returns_An_Empty_ContactLijst()
    {
        var contactLijst = ContactLijst.Create((ContactInfo[]?)null);
        contactLijst.Should().BeEmpty();
    }
}
