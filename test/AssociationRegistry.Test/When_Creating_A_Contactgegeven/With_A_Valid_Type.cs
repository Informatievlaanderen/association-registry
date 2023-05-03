namespace AssociationRegistry.Test.When_Creating_A_Contactgegeven;

using AutoFixture;
using FluentAssertions;
using Vereniging;
using Vereniging.Emails;
using Vereniging.SocialMedias;
using Vereniging.TelefoonNummers;
using Vereniging.Websites;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_Valid_Type
{
    [Theory]
    [InlineData("email", "test@example.org")]
    [InlineData("WeBsIte", "https://www.example.org")]
    [InlineData("SOCIALMEDIA", "https://www.example.org")]
    [InlineData("Telefoon", "0000112233")]
    public void Then_it_Returns_A_Contactgegeven(string type, string waarde)
    {
        var fixture = new Fixture();
        var contactgegeven = Contactgegeven.Create(type, waarde, fixture.Create<string>(), false);

        contactgegeven.Type.Should().Be(ContactgegevenType.Parse(type));
        contactgegeven.Waarde.Should().Be(waarde);
    }
}
