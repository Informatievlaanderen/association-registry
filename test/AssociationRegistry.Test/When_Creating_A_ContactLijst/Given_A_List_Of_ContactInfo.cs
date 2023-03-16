namespace AssociationRegistry.Test.When_Creating_A_ContactLijst;

using ContactInfo;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_List_Of_ContactInfo
{
    [Fact]
    public void Then_It_Returns_A_Filled_ContactLijst()
    {
        var listOfContactInfo = new List<ContactInfo>
        {
            ContactInfo.CreateInstance("De router", "ip@adress.com", "255.255.255.0", "http://127.0.0.1", "http://127.0.0.1#home", true),
        };

        var contactLijst = ContactLijst.Create(listOfContactInfo);

        contactLijst.Should().HaveCount(1);
        contactLijst[0].Contactnaam.Should().Be("De router");
        contactLijst[0].Email?.ToString().Should().Be("ip@adress.com");
        contactLijst[0].Telefoon?.ToString().Should().Be("255.255.255.0");
        contactLijst[0].Website?.ToString().Should().Be("http://127.0.0.1");
        contactLijst[0].SocialMedia?.ToString().Should().Be("http://127.0.0.1#home");
        contactLijst[0].PrimairContactInfo.Should().Be(true);
    }
}
