namespace AssociationRegistry.Test.When_Creating_An_Email;

using ContactInfo.Emails;
using ContactInfo.Emails.Exceptions;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_An_InvalidValid_EmailAddres
{
    [Theory]
    [InlineData("brol")]
    [InlineData("http://ongeldig.als.email")]
    [InlineData("http://ongeldig.@als")]
    [InlineData("http://@ongeldig.als")]
    [InlineData("conan@barbarian#sonya.red.com")]
    [InlineData("conan@barbarian-sonya.r.com")]
    [InlineData("conan@barbarian-sonya.c")]
    public void Then_it_throws_InvalidEmailFormatException(string emailString)
    {
        var ctor = () => Email.Create(emailString);
        ctor.Should().Throw<InvalidEmailFormat>();
    }
}
