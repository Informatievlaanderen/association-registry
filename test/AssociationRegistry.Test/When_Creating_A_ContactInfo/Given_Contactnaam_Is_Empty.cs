namespace AssociationRegistry.Test.When_Creating_A_ContactInfo;

using ContactInfo;
using ContactInfo.Exceptions;
using FluentAssertions;
using Xunit;

public class Given_Contactnaam_Is_Empty
{
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Then_it_throws_an_NoContactnaamException(string contactNaam)
    {
        var factory = () => ContactInfo.CreateInstance(contactNaam, "info@something.be", null, null, null, false);
        factory.Should().Throw<NoContactnaam>();
    }
}
