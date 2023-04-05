namespace AssociationRegistry.Test.When_Creating_A_Contactgegeven;

using AutoFixture;
using ContactGegevens;
using ContactGegevens.Exceptions;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_Invalid_Type
{
    [Fact]
    public void Then_it_Throws_An_InvalidContactTypeException()
    {
        var fixture = new Fixture();
        var createCall = () => Contactgegeven.Create(fixture.Create<string>(), fixture.Create<string>(), fixture.Create<string>(), false);

        createCall.Should().Throw<InvalidContactType>();
    }
}
