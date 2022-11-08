namespace AssociationRegistry.Test.Admin.Api.UnitTests.CommandHandlerTests.When_a_RegistreerVerenigingCommand_is_received;

using AssociationRegistry.Admin.Api;

public class ClockStub : IClock
{
    private readonly DateTime _now;

    public ClockStub(DateTime now)
    {
        _now = now;
    }

    public DateOnly Today
        => DateOnly.FromDateTime(TodayAsDateTime);

    public DateTime TodayAsDateTime
        => _now.Date;
}
