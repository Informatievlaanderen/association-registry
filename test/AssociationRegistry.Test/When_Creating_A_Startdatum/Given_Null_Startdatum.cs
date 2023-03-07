namespace AssociationRegistry.Test.When_Creating_A_Startdatum;

using FluentAssertions;
using Framework;
using Primitives;
using Startdatums;
using Xunit;

public class Given_Null_Startdatum
{
    private static readonly ClockStub ClockStub = new(new DateTime(2022, 12, 31));

    [Fact]
    public void Then_it_returns_null()
    {
        var startdatum = Startdatum.Create(ClockStub, NullOrEmpty<DateOnly>.Null);
        startdatum.Should().BeNull();
    }
}
