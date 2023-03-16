namespace AssociationRegistry.Test.When_Creating_A_Startdatum;

using FluentAssertions;
using Framework;
using Startdatums;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Startdatum_Equal_To_Today
{
    private static readonly ClockStub ClockStub = new(new DateTime(2022, 12, 31));
    [Fact]
    public void Then_it_returns_a_startdatum()
    {
        var startdatum = Startdatum.Create(ClockStub, ClockStub.Today)!;

        startdatum.Value.Should().Be(ClockStub.Today);
    }
}
