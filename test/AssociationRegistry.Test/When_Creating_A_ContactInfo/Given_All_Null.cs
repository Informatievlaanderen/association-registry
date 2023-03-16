namespace AssociationRegistry.Test.When_Creating_A_ContactInfo;

using ContactInfo;
using ContactInfo.Exceptions;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_All_Null
{
    [Fact]
    public void Then_it_throws_an_NoContactInfoException()
    {
        var factory = () => ContactInfo.CreateInstance(null!, null, null, null, null, false);
        factory.Should().Throw<NoContactInfo>();
    }
}
