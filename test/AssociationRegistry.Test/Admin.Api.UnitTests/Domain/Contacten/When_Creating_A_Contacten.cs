namespace AssociationRegistry.Test.Admin.Api.UnitTests.Domain.Contacten;

using AssociationRegistry.Contacten;
using FluentAssertions;
using Xunit;

public class When_Creating_A_Contacten
{
    public class Given_A_List_Of_ContactInfo
    {
        [Fact]
        public void Then_It_Returns_A_Filled_Contacten()
        {
            var listOfContactInfo = new List<ContactInfo>
            {
                ContactInfo.CreateInstance("De router", "ip@adress.com", "255.255.255.0", "127.0.0.1", "#home"),
            };

            var contactLijst = Contacten.Create(listOfContactInfo);

            contactLijst.Should().HaveCount(1);
            contactLijst[0].Contactnaam.Should().Be("De router");
            contactLijst[0].Email.Should().Be("ip@adress.com");
            contactLijst[0].Telefoon.Should().Be("255.255.255.0");
            contactLijst[0].Website.Should().Be("127.0.0.1");
        }
    }

    public class Given_An_Empty_List
    {
        [Fact]
        public void Then_It_Returns_An_Empty_ContactLijst()
        {
            var listOfContactInfo = Array.Empty<ContactInfo>();
            var contactLijst = Contacten.Create(listOfContactInfo);
            contactLijst.Should().BeEmpty();
        }
    }

    public class Given_Null
    {
        [Fact]
        public void Then_It_Returns_An_Empty_ContactLijst()
        {
            var contactLijst = Contacten.Create((ContactInfo[]?)null);
            contactLijst.Should().BeEmpty();
        }
    }
}
