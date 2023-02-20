namespace AssociationRegistry.Test.When_Creating_An_Email;

using ContactInfo.Emails;
using ContactInfo.Emails.Exceptions;
using FluentAssertions;
using Xunit;

public class Given_A_Valid_EmailAddres
{
    [Theory]
    [InlineData("email@vlaanderen.be")]
    public void Then_it_returns_a_new_Email(string emailString)
    {
        var email = Email.Create(emailString);
        email.ToString().Should().BeEquivalentTo(emailString);
    }
}

public class Given_An_InvalidValid_EmailAddres
{
    [Theory]
    [InlineData("brol")]
    [InlineData("http://ongeldig.als.email")]
    public void Then_it_throws_InvalidEmailFormatException(string emailString)
    {
        var ctor = () => Email.Create(emailString);
        ctor.Should().Throw<InvalidEmailFormat>();
    }
}
