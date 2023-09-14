namespace AssociationRegistry.Test.When_Creating_An_Email;

using FluentAssertions;
using Vereniging.Emails;
using Vereniging.Emails.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_An_InvalidValid_EmailAddres
{
    [Theory]
    [InlineData("brol")]
    [InlineData("http://ongeldig.als.email")]
    [InlineData("jos@club")]
    [InlineData("jos@club.")]
    [InlineData("jos(at)club.be")]
    [InlineData("jos.@punt.be")]
    [InlineData(".jos@club.be")]
    [InlineData("stoereVent@onderde/.be")]
    [InlineData("ikke@club@ginder.nl")]
    [InlineData("@club.be")]
    [InlineData("+32 477 24 55 00")]
    [InlineData("www.vereniging.be")]
    [InlineData("https://jos@club.be")]
    [InlineData("fons de spons@club.be")]
    [InlineData("http://@ongeldig.als")]
    [InlineData("conan@barbarian#sonya.red.com")]
    [InlineData("conan@barbarian-sonya.c")]
    public void Then_it_throws_InvalidEmailFormatException(string emailString)
    {
        var ctor = () => Email.Create(emailString);
        ctor.Should().Throw<EmailHeeftEenOngeldigFormaat>();
    }
}
