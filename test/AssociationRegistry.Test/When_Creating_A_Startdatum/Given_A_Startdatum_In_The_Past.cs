namespace AssociationRegistry.Test.When_Creating_A_Startdatum;

using Startdatums;
using Framework;
using FluentAssertions;
using Xunit;

public class Given_A_Startdatum_In_The_Past
{
    private static readonly ClockStub ClockStub = new(new DateTime(2022, 12, 31));
    [Fact]
    public void Then_it_returns_a_startdatum()
    {
        var expectedStartdatum = DateOnly.FromDateTime(new DateTime(2022, 10, 04));
        var startdatum = Startdatum.Create(ClockStub, expectedStartdatum)!;

        startdatum.Value.Should().Be(expectedStartdatum);
    }
}
