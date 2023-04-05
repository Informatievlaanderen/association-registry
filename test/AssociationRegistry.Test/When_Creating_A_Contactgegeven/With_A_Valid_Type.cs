namespace AssociationRegistry.Test.When_Creating_A_Contactgegeven;

using AutoFixture;
using ContactGegevens;
using ContactGegevens.Emails;
using ContactGegevens.SocialMedias;
using ContactGegevens.TelefoonNummers;
using ContactGegevens.Websites;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_Valid_Type
{
    [Theory]
    [InlineData("email", "test@example.org", typeof(Email))]
    [InlineData("WeBsIte", "https://www.example.org", typeof(Website))]
    [InlineData("SOCIALMEDIA", "https://www.example.org", typeof(SocialMedia))]
    [InlineData("Telefoon", "0000112233", typeof(TelefoonNummer))]
    public void Then_it_Returns_A_Contactgegeven(string type, string waarde, Type expectedType)
    {
        var fixture = new Fixture();
        var contactgegeven = Contactgegeven.Create(type, waarde, fixture.Create<string>(), false);

        contactgegeven.Should().BeOfType(expectedType);
    }
}
