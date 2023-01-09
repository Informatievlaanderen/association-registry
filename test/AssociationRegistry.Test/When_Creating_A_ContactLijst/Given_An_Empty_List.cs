namespace AssociationRegistry.Test.When_Creating_A_ContactLijst;

using ContactInfo;
using FluentAssertions;
using Xunit;

public class Given_An_Empty_List
{
    [Fact]
    public void Then_It_Returns_An_Empty_ContactLijst()
    {
        var listOfContactInfo = Array.Empty<ContactInfo>();
        var contactLijst = ContactLijst.Create(listOfContactInfo);
        contactLijst.Should().BeEmpty();
    }
}
