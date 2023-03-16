namespace AssociationRegistry.Test.When_Creating_A_ContactInfo;

using ContactInfo;
using ContactInfo.Exceptions;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_Contactnaam_Is_Null
{
    [Fact]
    public void Then_it_throws_an_NoContactnaamException()
    {
        var factory = () => ContactInfo.CreateInstance(null!, "info@something.be", null, null, null, false);
        factory.Should().Throw<NoContactnaam>();
    }
}
