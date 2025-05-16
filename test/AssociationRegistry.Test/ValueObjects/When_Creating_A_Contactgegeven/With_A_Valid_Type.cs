namespace AssociationRegistry.Test.ValueObjects.When_Creating_A_Contactgegeven;

using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Emails;
using AssociationRegistry.Vereniging.SocialMedias;
using AssociationRegistry.Vereniging.TelefoonNummers;
using AssociationRegistry.Vereniging.Websites;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_Valid_Type
{
    [Theory]
    [InlineData("e-mail", "test@example.org", typeof(Email))]
    [InlineData("WeBsIte", "https://www.example.org", typeof(Website))]
    [InlineData("SOCIALMEDIA", "https://www.example.org", typeof(SocialMedia))]
    [InlineData("Telefoon", "0000112233", typeof(TelefoonNummer))]
    public void Then_it_Returns_A_Contactgegeven(string type, string waarde, Type expectedType)
    {
        var fixture = new Fixture();
        var contactgegeven = Contactgegeven.Create(type, waarde, fixture.Create<string>(), isPrimair: false);

        contactgegeven.Should().BeOfType(expectedType);
    }
}
