namespace AssociationRegistry.Test.When_Creating_A_ContactLijst;

using ContactInfo;
using ContactInfo.Exceptions;
using FluentAssertions;
using Xunit;

public class Given_A_List_Of_ContactInfo_With_Duplicate_Contactnaam
{
    [Fact]
    public void Then_It_Thows_A_DuplicateContactnaamException()
    {
        const string contactnaam = "De router";
        var listOfContactInfo = new List<ContactInfo>
        {
            ContactInfo.CreateInstance(contactnaam, "ip@adress.com", "255.255.255.0", "http://127.0.0.1", "http://127.0.0.1#home", true),
            ContactInfo.CreateInstance(contactnaam, null, "255.255.255.128", "http://127.0.0.2", "http://127.0.0.1#switch", false),
        };

        var ctor = () => ContactLijst.Create(listOfContactInfo);
        ctor.Should().Throw<DuplicateContactnaam>();
    }
}
