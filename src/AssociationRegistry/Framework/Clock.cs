namespace AssociationRegistry.Framework;

public class Clock : IClock
{
    public DateOnly Today
        => DateOnly.FromDateTime(DateTime.Today);
}
