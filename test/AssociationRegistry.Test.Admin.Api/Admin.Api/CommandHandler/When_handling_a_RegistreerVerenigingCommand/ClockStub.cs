namespace AssociationRegistry.Test.Admin.Api.Admin.Api.CommandHandler.When_handling_a_RegistreerVerenigingCommand;

using AssociationRegistry.Framework;

public class ClockStub : IClock
{
    private readonly DateTime _now;

    public ClockStub(DateTime now)
    {
        _now = now;
    }

    public DateOnly Today
        => DateOnly.FromDateTime(_now);
}
