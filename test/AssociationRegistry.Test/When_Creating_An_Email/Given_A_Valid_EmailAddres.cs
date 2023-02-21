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
    public void Then_it_returns_a_new_Email(string emailString)
    {
        var email = Email.Create(emailString);
        email.ToString().Should().BeEquivalentTo(emailString);
    }
}
