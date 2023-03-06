namespace AssociationRegistry.Test.When_Creating_A_Startdatum;

using FluentAssertions;
using Framework;
using Startdatums;
using Startdatums.Exceptions;
using Xunit;

public class Given_A_Startdatum_In_The_Future
{
    private static readonly ClockStub ClockStub = new(new DateTime(2022, 12, 31));
    [Fact]
    public void Then_it_throws_an_InvalidStartdatumException()
    {
        var startdatum = DateOnly.FromDateTime(new DateTime(3022, 10, 04));
        var factory = () => StartDatum.Create(ClockStub, startdatum);

        factory.Should().Throw<InvalidStartdatumFuture>();
    }
}
