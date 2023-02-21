namespace AssociationRegistry.Test.When_Creating_An_Email;

using ContactInfo.Emails;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Valid_EmailAddres
{
    [Theory]
    [InlineData("email@vlaanderen.be")]
    [InlineData("conan@barbarian.history.com")]
    [InlineData("conan@barbarian-sonya.red.com")]
    [InlineData("conan@barbarian-sonya.com")]
    [InlineData("conan%sonya@barbarian.com")]
    [InlineData("conan#sonya@barbarian.com")]
    [InlineData("conan?sonya@barbarian.com")]
    [InlineData("conan_sonya@barbarian.com")]
    [InlineData("conan-sonya@barbarian.com")]
    [InlineData("conan!sonya@barbarian.com")]
    [InlineData("conan'sonya@barbarian.com")]
    [InlineData("conan*sonya@barbarian.com")]
    [InlineData("conan+sonya@barbarian.com")]
    [InlineData("conan^sonya@barbarian.com")]
    [InlineData("conan{sonya@barbarian.com")]
    [InlineData("conan}sonya@barbarian.com")]
    [InlineData("conan|sonya@barbarian.com")]
    [InlineData("conan~sonya@barbarian.com")]
    [InlineData("conan`sonya@barbarian.com")]
    [InlineData("conan`sonya@barbarian.vlaanderen")]
    [InlineData("aa@digitaal.vlaanderen")]
    [InlineData("de-vrolijke-vissers@sport.vlaanderen")]
    [InlineData("fons.de.spons@club.ac.be")]
    [InlineData("fons@spons.be")]
    [InlineData("a@b.cd")]
    [InlineData("Jane@do.com")]
    public void Then_it_returns_a_new_Email(string emailString)
    {
        var email = Email.Create(emailString);
        email.ToString().Should().BeEquivalentTo(emailString);
    }
}
