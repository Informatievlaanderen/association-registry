namespace AssociationRegistry.Test.When_Creating_A_ContactInfo;

using ContactInfo;
using ContactInfo.Exceptions;
using FluentAssertions;
using Xunit;

public class Given_Only_A_Name
{
    [Fact]
    public void Then_it_throws_an_NoContactInfoException()
    {
        var factory = () => ContactInfo.CreateInstance(null, null, null, null, null);
        factory.Should().Throw<NoContactInfo>();
    }
}
