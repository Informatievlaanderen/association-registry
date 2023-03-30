namespace AssociationRegistry.Framework;

public interface IClock
{
    DateOnly Today { get; }
}
