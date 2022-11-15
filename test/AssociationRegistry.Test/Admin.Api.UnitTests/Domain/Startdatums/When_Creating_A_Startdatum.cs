namespace AssociationRegistry.Test.Admin.Api.UnitTests.Domain.Startdatums;

using AssociationRegistry.Startdatums;
using AssociationRegistry.Startdatums.Exceptions;
using CommandHandlerTests.When_a_RegistreerVerenigingCommand_is_received;
using FluentAssertions;
using Xunit;

public class When_Creating_A_Startdatum
{
    private static readonly ClockStub ClockStub = new(new DateTime(2022, 12, 31));

    public class Given_A_Startdatum_In_The_Past
    {
        [Fact]
        public void Then_it_returns_a_startdatum()
        {
            var expectedStartdatum = DateOnly.FromDateTime(new DateTime(2022, 10, 04));
            var startdatum = Startdatum.Create(ClockStub, expectedStartdatum)!;

            startdatum.Value.Should().Be(expectedStartdatum);
        }
    }

    public class Given_A_Startdatum_Equal_To_Today
    {
        [Fact]
        public void Then_it_returns_a_startdatum()
        {
            var startdatum = Startdatum.Create(ClockStub, ClockStub.Today)!;

            startdatum.Value.Should().Be(ClockStub.Today);
        }
    }

    public class Given_Null_Startdatum
    {
        [Fact]
        public void Then_it_returns_null()
        {
            var startdatum = Startdatum.Create(ClockStub, null);

            startdatum.Should().BeNull();
        }
    }

    public class Given_A_Startdatum_In_The_Future
    {
        [Fact]
        public void Then_it_throws_an_InvalidStartdatumException()
        {
            var startdatum = DateOnly.FromDateTime(new DateTime(3022, 10, 04));
            var factory = () => Startdatum.Create(ClockStub, startdatum);

            factory.Should().Throw<InvalidStartdatum>();
        }
    }
}
