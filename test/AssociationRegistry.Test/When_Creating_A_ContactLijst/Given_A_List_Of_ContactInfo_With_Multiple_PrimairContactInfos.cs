namespace AssociationRegistry.Test.When_Creating_A_ContactLijst;

using ContactInfo;
using ContactInfo.Exceptions;
using FluentAssertions;
using Xunit;

public class Given_A_List_Of_ContactInfo_With_Multiple_PirmairContactInfos
{
    [Fact]
    public void Then_It_Thows_A_MultiplePrimaryContactInfosException()
    {
        var listOfContactInfo = new List<ContactInfo>
        {
            ContactInfo.CreateInstance("De router", "ip@adress.com", "255.255.255.0", "127.0.0.1", "#home", true),
            ContactInfo.CreateInstance("De switch", null, "255.255.255.128", "127.0.0.2", "#switch", true),
        };

        var ctor = () => ContactLijst.Create(listOfContactInfo);
        ctor.Should().Throw<MultiplePrimaryContactInfos>();
    }
}
