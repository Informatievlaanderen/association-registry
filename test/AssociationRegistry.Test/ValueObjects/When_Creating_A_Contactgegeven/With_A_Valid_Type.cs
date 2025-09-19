namespace AssociationRegistry.Test.ValueObjects.When_Creating_A_Contactgegeven;

using AutoFixture;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Emails;
using DecentraalBeheer.Vereniging.SocialMedias;
using DecentraalBeheer.Vereniging.TelefoonNummers;
using DecentraalBeheer.Vereniging.Websites;
using FluentAssertions;
using Xunit;

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
