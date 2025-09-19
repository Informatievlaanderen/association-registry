namespace AssociationRegistry.Test.ValueObjects.When_Creating_A_Contactgegeven;

using AutoFixture;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Exceptions;
using FluentAssertions;
using Xunit;

public class With_A_Invalid_Type
{
    [Fact]
    public void Then_it_Throws_An_InvalidContactTypeException()
    {
        var fixture = new Fixture();

        var createCall = ()
            => Contactgegeven.Create(fixture.Create<string>(), fixture.Create<string>(), fixture.Create<string>(), isPrimair: false);

        createCall.Should().Throw<ContactTypeIsOngeldig>();
    }
}
